using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace iUtils
{
	public class iPipe
	{
		#region Delegates

		public delegate void ConnectCallBack();

		public delegate void DataReadCallBack(int a_bytesRead);

		public delegate void DataWriteCallBack();

		#endregion

		#region PipeIO enum

		public enum PipeIo
		{
			Asynchronous,
			Blocking
		}

		#endregion

		#region PipeState enum

		public enum PipeState
		{
			Disconnected,
			Connecting,
			Connected,
			WaitingToRecv,
			WaitingToSend,
			Broken
		}

		#endregion

		#region PipeType enum

		public enum PipeType
		{
			Client,
			Server
		}

		#endregion

		private int _blockingTimeout;

		private ConnectCallBack _connectCallback;

		/// <summary>
		/// Pipe stuff
		/// </summary>
		private PipeStream _pipeHandle;

		private string _pipeName = "";

		// this is the buffer that is currently being read into
		private byte[] _pipeReadBackBuffer;
		private byte[] _pipeReadFrontBuffer;

		// the number of valid bytes that have been read from the client
		// the number of bytes waiting in the backbuffer
		private int _pipeReadValidBackBufferBytes;
		private int _pipeReadValidFrontBufferBytes;

		private string _pipeServer = ".";
		private byte[] _pipeWriteBackBuffer;
		private byte[] _pipeWriteFrontBuffer;
		private DataReadCallBack _readCallback;
		private EventWaitHandle _waitHandle;
		private DataWriteCallBack _writeCallback;

		/// <summary>
		/// Initialise a pipe object. This object can be used to connect to a server
		/// or a client.  All client connections are synchronous and all server
		/// connections are asynchronous.
		/// </summary>
		public iPipe()
		{
			LogicalPipeState = PipeState.Disconnected;

			PipeReadBufferSizeKb = 4;
			PipeWritBufferSizeKb = 4;

			_blockingTimeout = Timeout.Infinite;
		}

		/// <summary>
		/// The name of the server to connect to. Defaults to "." i.e. the local machine.
		/// Only applies to client pipes.
		/// </summary>
		public string PipeServer
		{
			get => _pipeServer;
			set { _pipeServer = value; }
		}

		/// <summary>
		/// returns the name of the pipe that was created/connected to.
		/// </summary>
		public string PipeName => _pipeName;

		/// <summary>
		/// how long to wait for blocking reads & writes
		/// </summary>
		public int BlockingTimeout
		{
			get => _blockingTimeout;
			set { _blockingTimeout = value; }
		}

		/// <summary>
		/// reflects the class state of the pipe. this does not necessarily accurately
		/// reflect the actual state of the pipe
		/// </summary>
		public PipeState LogicalPipeState { get; internal set; }

		/// <summary>
		/// sets the maximum size of the _pipeHandle read buffer. This
		/// can only be changed when the _pipeHandle is not connected
		/// </summary>
		public int PipeReadBufferSizeKb
		{
			get => _pipeReadFrontBuffer.Length;
			set
			{
				if (LogicalPipeState != PipeState.Disconnected)
					throw new ApplicationException("Can only change buffer size when _pipeHandle is disconnected");

				int buffSize = Math.Max(value, 4)*1024;

				_pipeReadFrontBuffer = new byte[buffSize];
				_pipeReadBackBuffer = new byte[buffSize];

				_pipeReadValidFrontBufferBytes = 0;
				_pipeReadValidBackBufferBytes = 0;
			}
		}

		/// <summary>
		/// sets the maximum size of the _pipeHandle write buffer. This
		/// can only be changed when the _pipeHandle is not connected
		/// </summary>
		public int PipeWritBufferSizeKb
		{
			get => _pipeWriteFrontBuffer.Length;
			set
			{
				if (LogicalPipeState != PipeState.Disconnected)
					throw new ApplicationException("Can only change buffer size when _pipeHandle is disconnected");

				int buffSize = Math.Max(value, 4)*1024;

				_pipeWriteFrontBuffer = new byte[buffSize];
				_pipeWriteBackBuffer = new byte[buffSize];
			}
		}

		/// <summary>
		/// Do not cahche this handle - it changes every
		/// time SendDataToServer is called as well
		/// as when the write buffer is re-sized
		/// </summary>
		public byte[] PipeWriteBuffer => _pipeWriteFrontBuffer;

		/// <summary>
		/// Do not cache this handle - it changes every
		/// time RecvDataFromServer() is callled as well
		/// as when the read buffer is re-sized
		/// </summary>
		public byte[] PipeReadBuffer => _pipeReadFrontBuffer;

		/// <summary>
		/// returns the number of bytes in the read buffer that have
		/// been read from the client
		/// </summary>
		public int ReadDataLength => _pipeReadValidFrontBufferBytes;

		public override string ToString()
		{
			return _pipeName + " : " + LogicalPipeState.ToString();
		}

		/// <summary>
		/// Connect to a server pipe as a client - synchronous
		/// </summary>
		/// <param name="a_pipeName">the name of the pipe to connect to</param>
		public void Connect(string a_pipeName)
		{
			Connect(a_pipeName, PipeType.Client, null);
		}

		/// <summary>
		/// Connect to/Create a pipe
		/// </summary>
		/// <param name="a_pipeName">the name of the pipe</param>
		/// <param name="a_connectionType">Specify Client or Server</param>
		public void Connect(string a_pipeName, PipeType a_connectionType)
		{
			Connect(a_pipeName, a_connectionType, null);
		}

		/// <summary>
		/// Create a server pipe asynchronously
		/// </summary>
		/// <param name="a_pipeName">the name of the pipe</param>
		/// <param name="a_connectCallback">a function to call after the pipe is created</param>
		public void Connect(string a_pipeName, ConnectCallBack a_connectCallback)
		{
			Connect(a_pipeName, PipeType.Server, a_connectCallback);
		}

		/// <summary>
		/// Connect to/Create a pipe
		/// </summary>
		/// <param name="a_pipeName">the name of the pipe</param>
		/// <param name="a_connectionType">specify client or server</param>
		/// <param name="a_connectCallback">function top callback when the connection is complete</param>
		public void Connect(string a_pipeName, PipeType a_connectionType, ConnectCallBack a_connectCallback)
		{
			if (LogicalPipeState == PipeState.Connecting)
				return;

			if (LogicalPipeState != PipeState.Disconnected)
				Disconnect();

			_waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset);

			_connectCallback = a_connectCallback;

			LogicalPipeState = PipeState.Connecting;

			_pipeName = a_pipeName;

			if (a_connectionType == PipeType.Server)
			{
				_pipeHandle = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte,
				                                        PipeOptions.Asynchronous);

				(_pipeHandle as NamedPipeServerStream).BeginWaitForConnection(ConnectCompleteCb, this);
			}
			else if (a_connectionType == PipeType.Client)
			{
				_pipeHandle = new NamedPipeClientStream(_pipeServer, _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

				// this is a blocking operation
				(_pipeHandle as NamedPipeClientStream).Connect();

				// simulate a connect callback
				InnerConnectComplete(this);
			}
		}

		// this can block forever if the server keeps the
		// _pipeHandle open but never reads from it
		public void Flush()
		{
			if (_pipeHandle != null && _pipeHandle.IsConnected)
			{
				try
				{
					_pipeHandle.WaitForPipeDrain();
				}
				catch
				{
				} // ignore all errors, tra-la
			}
		}

		public virtual void Disconnect()
		{
			if (_pipeHandle != null)
			{
				if (_pipeHandle is NamedPipeServerStream)
				{
					try
					{
						(_pipeHandle as NamedPipeServerStream).Disconnect();
					}
					catch (Exception)
					{
					}
				}

				_pipeHandle.Dispose();

				_pipeHandle = null;
			}

			LogicalPipeState = PipeState.Disconnected;
		}

		private bool SendDataInternal(int a_bytesToWrite, PipeIo a_pipeIoMode)
		{
			int offset = 0;

			try
			{
				if (_pipeHandle.CanWrite)
				{
					if (a_pipeIoMode == PipeIo.Blocking)
					{
						_pipeHandle.Write(_pipeWriteFrontBuffer, offset, a_bytesToWrite);
					}
					else if (a_pipeIoMode == PipeIo.Asynchronous)
					{
						LogicalPipeState = PipeState.WaitingToSend;

						_pipeHandle.BeginWrite(_pipeWriteFrontBuffer, offset, a_bytesToWrite, WriteCompleteCb, this);
					}

					return true;
				}
			}
			catch (ArgumentException ex)
			{
				Console.WriteLine("Error writing _pipeHandle - Arguments : {0}", ex.Message);
				LogicalPipeState = PipeState.Broken;
				throw;
			}
			catch (ObjectDisposedException ex)
			{
				Console.WriteLine("Error writing _pipeHandle - Pipe is closed : {0}", ex.Message);
				LogicalPipeState = PipeState.Broken;
			}
			catch (IOException ex)
			{
				Console.WriteLine("Error writing _pipeHandle - IO : {0}", ex.Message);
				LogicalPipeState = PipeState.Broken;
			}
			return false;
		}

		private bool RecvDataInternal(PipeIo a_pipeIoMode)
		{
			int offset = 0;

			_pipeReadValidBackBufferBytes = 0;

			try
			{
				if (_pipeHandle.CanRead)
				{
					int bufferSize = _pipeReadBackBuffer.Length - offset;

					if (a_pipeIoMode == PipeIo.Blocking)
					{
						LogicalPipeState = PipeState.Connected;
						_pipeReadValidBackBufferBytes = _pipeHandle.Read(_pipeReadBackBuffer, offset, bufferSize);
					}
					else if (a_pipeIoMode == PipeIo.Asynchronous)
					{
						LogicalPipeState = PipeState.WaitingToRecv;
						_pipeHandle.BeginRead(_pipeReadBackBuffer, offset, bufferSize, ReadCompleteCb, this);
					}

					return true;
				}
			}
			catch (ArgumentException ex)
			{
				// this is probably a programming error
				Console.WriteLine("Error reading _pipeHandle - Arguments : {0}", ex.Message);
				LogicalPipeState = PipeState.Broken;
				throw;
			}
			catch (ObjectDisposedException ex)
			{
				Console.WriteLine("Error reading _pipeHandle - Pipe is closed : {0}", ex.Message);
				LogicalPipeState = PipeState.Broken;
			}
			catch (IOException ex)
			{
				Console.WriteLine("Error reading _pipeHandle - IO : {0}", ex.Message);
				LogicalPipeState = PipeState.Broken;
			}

			return false;
		}

		public bool RecvData(PipeIo a_pipeIoMode)
		{
			return RecvData(a_pipeIoMode, null);
		}

		public bool RecvData(PipeIo a_pipeIoMode, DataReadCallBack a_callback)
		{
			if (CheckPipeState())
			{
				// calling this function always kills your frontbuffer
				_pipeReadValidFrontBufferBytes = 0;

				if (a_pipeIoMode == PipeIo.Blocking)
				{
					// block until there are no outstanding transactions
					if (_waitHandle.WaitOne(_blockingTimeout, true))
					{
						// do it to the back buffer
						RecvDataInternal(PipeIo.Blocking);

						// swap in our read attempt
						SwapReadBuffers();

						// allow subsequent reads/writes to go
						_waitHandle.Set();
					}
				}
				else if (a_pipeIoMode == PipeIo.Asynchronous)
				{
					// if there are no outstanding transactions
					if (_waitHandle.WaitOne(0, true))
					{
						// swap the potentially valid back buffer to the front in case we succeed
						SwapReadBuffers();

						// callback to the user when the recieve happens
						_readCallback = a_callback;

						// attemp to start a read
						RecvDataInternal(PipeIo.Asynchronous);
					}
				}

				return (_pipeReadValidFrontBufferBytes > 0);
			}

			return false;
		}

		public bool SendData(int a_bytesToWrite, PipeIo a_pipeIoMode)
		{
			return SendData(a_bytesToWrite, a_pipeIoMode, null);
		}

		public bool SendData(int a_bytesToWrite, PipeIo a_pipeIoMode, DataWriteCallBack a_callback)
		{
			if (CheckPipeState())
			{
				// check to see if the previous write has finished
				if (_waitHandle.WaitOne(0, true))
				{
					bool result = false;

					if (a_pipeIoMode == PipeIo.Blocking)
					{
						// the user gets no callback with a sync read
						_writeCallback = null;

						// write the data in the front buffer
						result = SendDataInternal(a_bytesToWrite, PipeIo.Blocking);

						// allow subsequent reads/writes to go
						_waitHandle.Set();
					}
					else if (a_pipeIoMode == PipeIo.Asynchronous)
					{
						_writeCallback = a_callback;

						// queue a read into the front buffer
						result = SendDataInternal(a_bytesToWrite, PipeIo.Asynchronous);

						// if we succeeded then expose
						// a fresh front buffer to the user
						if (result)
							SwapWriteBuffers();
					}

					return result;
				}
			}

			return false;
		}

		private bool CheckPipeState()
		{
			switch (LogicalPipeState)
			{
				case PipeState.Disconnected:
				case PipeState.Broken:
					Disconnect();
					return false;

				case PipeState.Connecting:
					return false;

				case PipeState.Connected:
				case PipeState.WaitingToRecv:
				case PipeState.WaitingToSend:
					if (_pipeHandle.IsConnected)
						return true;
					else
						Disconnect();

					return false;
			}

			return false;
		}

		private void SwapWriteBuffers()
		{
			byte[] bufferTemp = _pipeWriteFrontBuffer;

			_pipeWriteFrontBuffer = _pipeWriteBackBuffer;

			_pipeWriteBackBuffer = bufferTemp;
		}

		private void SwapReadBuffers()
		{
			// swap buffer valid lengths
			int tempValidBytes = _pipeReadValidFrontBufferBytes;

			_pipeReadValidFrontBufferBytes = _pipeReadValidBackBufferBytes;

			_pipeReadValidBackBufferBytes = tempValidBytes;

			// swap buffers
			byte[] bufferTemp = _pipeReadFrontBuffer;

			_pipeReadFrontBuffer = _pipeReadBackBuffer;

			_pipeReadBackBuffer = bufferTemp;
		}

		private static void ReadCompleteCb(IAsyncResult a_result)
		{
			var reader = (iPipe) a_result.AsyncState;

			if (reader == null || reader._pipeHandle == null)
				return;

			try
			{
				// this asumes we always read to the backbuffer which may be wrong
				reader._pipeReadValidBackBufferBytes = reader._pipeHandle.EndRead(a_result);

				reader.LogicalPipeState = PipeState.Connected;
			}
			catch (OperationCanceledException)
			{
				reader.LogicalPipeState = PipeState.Broken;
			}
			catch (IOException ex)
			{
				Console.WriteLine("Error (End)reading _pipeHandle - IO : {0}", ex.Message);

				reader.LogicalPipeState = PipeState.Broken;
			}
			catch
			{
				reader.LogicalPipeState = PipeState.Broken;

				throw;
			}
			finally
			{
				// allow the Thread procedure to continue
				reader._waitHandle.Set();

				if (reader._readCallback != null)
					reader._readCallback(reader._pipeReadValidBackBufferBytes);
			}
		}

		private static void WriteCompleteCb(IAsyncResult a_result)
		{
			var writer = (iPipe) a_result.AsyncState;

			if (writer == null || writer._pipeHandle == null)
				return;

			try
			{
				writer._pipeHandle.EndWrite(a_result);

				writer.LogicalPipeState = PipeState.Connected;
			}
			catch (IOException ex)
			{
				Console.WriteLine("Error (End)writing _pipeHandle - IO : {0}", ex.Message);

				writer.LogicalPipeState = PipeState.Broken;
			}
			catch
			{
				writer.LogicalPipeState = PipeState.Broken;
			}
			finally
			{
				// allow the Thread procedure to continue
				writer._waitHandle.Set();

				if (writer._writeCallback != null)
					writer._writeCallback();
			}
		}

		private static void ConnectCompleteCb(IAsyncResult a_result)
		{
			var connector = (iPipe) a_result.AsyncState;

			if (connector == null)
				return;

			var nps = connector._pipeHandle as NamedPipeServerStream;

			if (nps != null)
			{
				try
				{
					nps.EndWaitForConnection(a_result);

					InnerConnectComplete(connector);
				}
				catch
				{
					connector.LogicalPipeState = PipeState.Broken;
				}
			}
		}

		private static void InnerConnectComplete(iPipe a_connector)
		{
			a_connector.LogicalPipeState = PipeState.Connected;

			if (a_connector._connectCallback != null)
				a_connector._connectCallback();
		}
	}
}