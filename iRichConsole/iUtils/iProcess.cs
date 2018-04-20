using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace iUtils
{
	public static class iProcess
	{
		public delegate void GotOutput(string a_output);
		public delegate void GotError(string a_output);
		public delegate void DoneWork();

		private static Process s_theProcess;
		private static BackgroundWorker s_theOutputReader;
		private static BackgroundWorker s_theErrorReader;

		private static int s_theHour = -1;

		public static DoneWork FinishedFunction { get; set; }
		public static GotOutput OutputFunction { get; set; }
		public static GotError ErrorFunction { get; set; }
		public static string WorkingDirectory { get; set; } = "";
		public static bool IsRunning => s_theProcess != null;

		public static bool RunSync(string a_prog, string a_arg)
		{
			return Run(a_prog, a_arg, false);
		}

		public static bool RunSync(string a_prog, string a_arg, string a_working)
		{
			WorkingDirectory = a_working;

			return Run(a_prog, a_arg, false);
		}

		public static bool RunAsync(string a_prog, string a_arg)
		{
			return Run(a_prog, a_arg, true);
		}

		public static bool RunBatch(string a_batchFile, string a_args)
		{
			var batchArgs = $"/C \"{a_batchFile}\" {a_args}";

			return Run(Environment.GetEnvironmentVariable("ComSpec"), batchArgs, false);
		}

		public static bool Kill()
		{
			if (s_theProcess == null)
				return true;

			iProcessHelper.TerminateId(s_theProcess.Id);

			ProcessExited(null, null);

			return true;
		}

		private static void WaitTillHour(int a_hourNum)
		{
			if (a_hourNum < 0)
				a_hourNum = 0;

			if (a_hourNum > 23)
				a_hourNum = 0;

			s_theHour = a_hourNum;
		}

		private static bool Run(string a_prog, string a_arg, bool a_isAsync)
		{
#if DEBUG
      WriteOutput("\n***************** Starting Process {0} {1} **********************\n", a_prog, a_arg);
#endif
			if (string.IsNullOrEmpty(a_prog))
				WriteError("\nprogram path cannot be empty\n");
			else if (!Directory.Exists(WorkingDirectory))
				WriteError("\nworking directory \"" + WorkingDirectory + "\" does not exist\n");
			else if (s_theProcess != null)
				WriteError("\nanother process is already running\n");
			else
			{
				s_theProcess = new Process
				{
					StartInfo =
					{
						CreateNoWindow = true,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						FileName = a_prog,
						Arguments = a_arg,
						WorkingDirectory = WorkingDirectory
					},
					EnableRaisingEvents = true
				};

				s_theProcess.Exited += ProcessExited;

				try
				{
					SleepUntilReady();

					try
					{
						s_theProcess.Start();
#if DEBUG
            WriteOutput("\n***************** Started Process {0} {1} **********************\n", a_prog, a_arg);
#endif
						StartThreads();

						if (a_isAsync == false)
						{
							SleepPump();
						}

						return true;
					}
					catch (Exception ex)
					{
						WriteError("Process for {0}\nException starting process : {1}\n", a_prog, ex.Message);
					}
				}
				catch (Exception ex)
				{
					WriteError("Process for {0}\nException running process : {1}\n", a_prog, ex.Message);
				}
			}

			// call the tidy up stuff
			ProcessExited(null, null);

			return false;
		}

		private static void SleepUntilReady()
		{
			while (s_theHour >= 0)
			{
				if (DateTime.Now.Hour == s_theHour)
					s_theHour = -1;

				if (Application.MessageLoop)
					Application.DoEvents();

				Thread.Sleep(1000*60); // sleep for 60 seconds
			}
		}

		private static void SleepPump()
		{
			while (s_theProcess != null && s_theProcess.HasExited == false)
			{
				if (Application.MessageLoop)
					Application.DoEvents();

				Thread.Sleep(100);
			}
		}

		private static void TerminateThreads()
		{
			// this flushes the output
			WriteOutput("");

			//cancel the reader threads
			s_theOutputReader?.CancelAsync();

			s_theErrorReader?.CancelAsync();

			// close the process if necessary
			if (s_theProcess != null)
			{
#if DEBUG
				var runTime = s_theProcess.ExitTime - s_theProcess.StartTime;
				WriteOutput("\nProcess for {0}\nProcess duration = {1}\n", s_theProcess.StartInfo.FileName, runTime.ToString());
#endif
				s_theProcess.Close();
			}

			// give the process and threads so timeslice
			Thread.Sleep(200);

			s_theProcess = null;
			s_theOutputReader = null;
			s_theErrorReader = null;
#if DEBUG
      WriteOutput("\n***************** Finished Process **********************\n");
#endif
			OutputFunction = null;
			ErrorFunction = null;
			FinishedFunction = null;
		}

		private static void StartThreads()
		{
			s_theOutputReader = new BackgroundWorker
			{
				WorkerSupportsCancellation = true,
				WorkerReportsProgress = true
			};
			s_theOutputReader.DoWork += theOutputReader_DoWork;
			s_theOutputReader.ProgressChanged += theOutputReader_ProgressChanged;

			s_theErrorReader = new BackgroundWorker
			{
				WorkerSupportsCancellation = true,
				WorkerReportsProgress = true
			};
			s_theErrorReader.DoWork += theErrorReader_DoWork;
			s_theErrorReader.ProgressChanged += theErrorReader_ProgressChanged;

			s_theOutputReader.RunWorkerAsync();
			s_theErrorReader.RunWorkerAsync();
		}

		private static void WriteError(string a_errString)
		{
			if (ErrorFunction != null)
				ErrorFunction(a_errString);
			else
				Console.WriteLine(a_errString);
		}

		private static void WriteError(string a_errString, params object[] a_args)
		{
			WriteError(string.Format(a_errString, a_args));
		}

		private static void WriteOutput(string a_outString)
		{
			if (OutputFunction != null)
				OutputFunction(a_outString);
			else
				Console.WriteLine(a_outString);
		}

		private static void WriteOutput(string a_outString, params object[] a_args)
		{
			WriteOutput(string.Format(a_outString, a_args));
		}

		private static void ProccessCompleteCallback(object a_sender, RunWorkerCompletedEventArgs a_e)
		{
			FinishedFunction?.Invoke();
		}

		private static void theErrorReader_ProgressChanged(object a_sender, ProgressChangedEventArgs a_e)
		{
			WriteError((string)a_e.UserState);
		}

		private static void theOutputReader_ProgressChanged(object a_sender, ProgressChangedEventArgs a_e)
		{
			WriteOutput((string)a_e.UserState);
		}

		private static void theErrorReader_DoWork(object a_sender, DoWorkEventArgs a_e)
		{
			var worker = a_sender as BackgroundWorker;

			Thread.CurrentThread.Name = "theErrorReader_DoWork";

			ReadStream(worker, s_theProcess.StandardError);
		}

		private static void theOutputReader_DoWork(object a_sender, DoWorkEventArgs a_e)
		{
			var worker = a_sender as BackgroundWorker;

			Thread.CurrentThread.Name = "theOutputReader_DoWork";

			ReadStream(worker, s_theProcess.StandardOutput);
		}

		private static void ReadStream(BackgroundWorker a_thread, StreamReader a_reader)
		{
			var buffLen = 1024;

			var buffer = new byte[buffLen];

			while (a_thread.CancellationPending == false)
			{
				var charCount = a_reader.BaseStream.Read(buffer, 0, buffLen);

				if (charCount > 0)
				{
					var str = Encoding.ASCII.GetString(buffer, 0, charCount);

					a_thread.ReportProgress(0, str);
				}
				else
					Thread.Sleep(100);
			}
		}

		private static void ProcessExited(object a_sender, EventArgs a_e)
		{
			// call the clients callback
			ProccessCompleteCallback(null, null);

			// tidy up
			TerminateThreads();
		}
	}
}