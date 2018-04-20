using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace iUtils
{
  public class iTcpClient : IClient
  {
	  public static int BufferSize { get; set; } = 4096;

	  private readonly iTcpListener _listener;
    private TcpClient _client;
    private NetworkStream _netStream;

    private string _name;
    private string _error;

    private readonly byte[] _buffer = new byte[BufferSize];
    private int _dataLength = BufferSize;

    public event iDisconnectEventHandler Disconnect;
    public event iDatatRecievedEventHandler DataRecieved;
    public event iErrorEventHandler ClientError;

    // this constructor will connect to a remote listener
    /// <summary>
    /// Used by applications to to connect to an iTcpListener
    /// </summary>
    public iTcpClient(string a_host, int a_port)
    {
      try
      {
        BeginRead(new TcpClient(a_host, a_port));
      }
      catch (SocketException ex)
      {
        SocketIoError("Connection", ex);
      }
    }

    // this constructor is used by a connectionType to create client connections
    /// <summary>
    /// Used by iTcpListener to create a local client object
    /// </summary>
    internal iTcpClient(iTcpListener a_listener, TcpClient a_client)
    {
      _listener = a_listener;

      BeginRead(a_client);
    }

    private void OnDisconnect()
    {
      if (Disconnect != null)
        Disconnect(this);
    }

    private void OnDataRecieved()
    {
      if (DataRecieved != null)
        DataRecieved(this);
    }

    private void OnSocketError()
    {
      if (ClientError != null)
        ClientError(this);
    }

    public string Error => _error;

	  public int DataLength
    {
      get => _dataLength;
		  set => _dataLength = value;
	  }

    public byte[] Buffer => _buffer;

	  public IListener Listener => _listener as IListener;

	  public IClient Client => _client as IClient;

	  private void BeginRead(TcpClient a_newClient)
    {
      if (_client != a_newClient)
      {
        Close();

        if (a_newClient != null)
        {
          _client = a_newClient;

          _name = IPAddress.Parse(((IPEndPoint)_client.Client.LocalEndPoint).Address.ToString()) +
                    " on port number " + ((IPEndPoint)_client.Client.LocalEndPoint).Port.ToString();

          _netStream = _client.GetStream();

          try
          {
            _netStream.BeginRead(_buffer, 0, _buffer.Length, new AsyncCallback(AsyncRead), this);
          }
          catch (IOException ex)
          {
            if (ex.InnerException is SocketException)
              SocketIoError("BeginRead", ex.InnerException);
            else
              throw;
          }
          catch (Exception)
          {
            throw;
          }
        }
      }
    }

    private void SocketIoError(string a_msg, Exception a_ex)
    {
      SocketException sockEx = (SocketException)a_ex;

      _error = String.Format("Socket error in {2} : {0} ({1})", sockEx.Message, sockEx.ErrorCode, a_msg);

      OnSocketError();
    }

    public bool Send(byte[] a_data, int a_length)
    {
      if (IsConnected)
      {
        try
        {
          _netStream.Write(a_data, 0, a_length);
        }
        catch (IOException ex)
        {
          if (ex.InnerException is SocketException)
            SocketIoError("Send", ex.InnerException);
          else
            throw;
        }
        return true;
      }
      return false;
    }

    private static void AsyncRead(IAsyncResult a_result)
    {
      iTcpClient client = (iTcpClient)a_result.AsyncState;

      try
      {
        if (client._netStream.CanRead)
          client.DataLength = client._netStream.EndRead(a_result);
        else
          return;
      }
      catch (IOException ex)
      {
        if (ex.InnerException is SocketException)
          client.SocketIoError("AsyncRead - EndRead", ex.InnerException);
        else
          throw;
      }

      if (client.DataLength != 0)
      {
        client.OnDataRecieved();

        try
        {
          client._netStream.BeginRead(client._buffer, 0, client._buffer.Length, new AsyncCallback(AsyncRead), client);
        }
        catch (IOException ex)
        {
          if (ex.InnerException is SocketException)
            client.SocketIoError("AsyncRead - BeginRead", ex.InnerException);
          else
            throw;
        }
        catch
        {
          if (client._netStream == null || !client._netStream.CanRead)
            return;
          else
            throw;
        }
      }
      else
      {
        client.Close();
      }
    }

    public bool IsConnected => (_client != null) && (_client.Connected);

	  public void Close()
    {
      if (_client != null)
      {
        OnDisconnect();

        if (_netStream != null)
          _netStream.Close();

        if (IsConnected)
        {
          _client.Client.Shutdown(SocketShutdown.Both);

          _client.Client.Disconnect(false);
        }

        _client.Close();

        _client = null;
      }
    }

    public override string ToString()
    {
      if (IsConnected)
        return _name;
      else
        return "Disconnected";
    }
  }

  public class iTcpListener : IListener
  {
    private List<iTcpClient> _connections;
    private TcpListener _listener;
    private int _port;
    private string _error;
    private int _maxClients = 1;

    public event iConnectEventHandler Connect;
    public event iErrorEventHandler ListenError;

    public iTcpListener(int a_port)
    {
      _connections = new List<iTcpClient>();

      _port = a_port;
    }

    public bool Listen()
    {
      try
      {
        _listener = new TcpListener(IPAddress.Any, _port);

        _listener.Start();

        BeginAccept();

        return true;
      }
      catch (SocketException ex)
      {
        _error = String.Format("Error connecting connectionType socket : {0} ({1})", ex.Message, ex.ErrorCode);

        OnSocketError();
      }

      return false;
    }

    /// <summary>
    /// only listens for another client if we have less than _maxClients
    /// </summary>
    private void BeginAccept()
    {
      lock (_connections)
      {
        if (_connections.Count < _maxClients)
          _listener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClient), this);
      }
    }

    private void OnSocketError()
    {
      if (ListenError != null)
        ListenError(this);
    }

    private void OnConnect(iTcpClient a_client)
    {
      lock (_connections)
      {
        _connections.Add(a_client);
      }

      if (Connect != null)
        Connect(this, a_client);
    }

    private static void AcceptTcpClient(IAsyncResult a_result)
    {
      iTcpListener server = (iTcpListener)a_result.AsyncState;

      try
      {
        TcpClient tcpClient = server.Listener.EndAcceptTcpClient(a_result);

        if (tcpClient == null)
          return;

        iTcpClient client = new iTcpClient(server, tcpClient);

        client.Disconnect += new iDisconnectEventHandler(client_Disconnect);

        server.OnConnect(client);

        server.BeginAccept();
      }
      catch (SocketException ex)
      {
        server._error = String.Format("Error accepting client socket : {0} ({1})", ex.Message, ex.ErrorCode);

        server.OnSocketError();
      }
      catch (ObjectDisposedException)
      {
      }
    }

    private static void client_Disconnect(IClient a_iclient)
    {
      iTcpClient client = a_iclient as iTcpClient;

      if (client != null)
      {
        iTcpListener listener = client.Listener as iTcpListener;

        listener._connections.Remove(client);

        listener.BeginAccept();
      }
    }

    public void ShutDown()
    {
      if (_listener != null)
      {
        _listener.Stop();
      }

      if (_connections != null)
      {
        foreach (iTcpClient conn in _connections)
        {
          conn.Disconnect -= new iDisconnectEventHandler(client_Disconnect);
          conn.Close();
        }

        _connections.Clear();
      }
    }

    internal int MaxClients
    {
      get => _maxClients;
	    set => BeginAcceptIfFull(value);
    }

    private void BeginAcceptIfFull(int a_newMax)
    {
      int oldMax = _maxClients;

      _maxClients = a_newMax;

      lock (_connections)
      {
        // only start another accept connection
        // if we are already full
        if (_connections.Count >= oldMax)
          BeginAccept();
      }
    }

    private TcpListener Listener
    {
      get => _listener;
	    set => _listener = value;
    }

    public string Error => _error;

	  public int Port => _port;
  }
}