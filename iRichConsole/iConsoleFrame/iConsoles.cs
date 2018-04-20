using System;
using System.Drawing;
using System.Windows.Forms;
using iCppLib;
using iUtils;
using System.Runtime.InteropServices;
using System.Text;

namespace iConsoleFrame
{
    public partial class iConsoles : Form
    {
        public iConsoles()
        {
            InitializeComponent();
        }

        iPipe _pipeServer;
        System.Timers.Timer _readTimer2;
        iConsoleWindowReader _windowReader;

        private void iConsoles_Load(object a_sender, EventArgs a_e)
        {
            _readTimer2 = new System.Timers.Timer(16.0) {AutoReset = false};
            _readTimer2.Elapsed += read_timer2_Elapsed;
            _readTimer2.SynchronizingObject = this;
            _readTimer2.Enabled = false;

            _textBuffer = new StringBuilder(64 * 1024);

            _textBuffer.Append('\x0', 64*1024);

            _windowReader = new iConsoleWindowReader();

            _rtb_debug.DrawRuler();
        }

        StringBuilder _textBuffer;
        readonly iStopwatch _textRenderSw = new iStopwatch("text render");

        void read_timer2_Elapsed(object a_sender, System.Timers.ElapsedEventArgs a_e)
        {
            try
            {
                switch (_pipeServer.LogicalPipeState)
                {
                    case iPipe.PipeState.Connected:

                        bool recvResult = _pipeServer.RecvData(iPipe.PipeIo.Asynchronous, ReadCallback);

                        if (recvResult)
                        {
                            _rtb_debug.WriteLine($"Processing {_pipeServer.ReadDataLength} bytes....", Color.Green);

                            // do something with the data
                            _windowReader.Decode(_pipeServer.PipeReadBuffer);

                            int updated = _windowReader.RecordCount;

                            for (int i = 0; i < updated; i++)
                            {
                                var theInfo = _windowReader.GetCharDiff(i);

                                _textBuffer[theInfo.Offset] = (char)theInfo.UnicodeChar;
                            }

                            _textRenderSw.PulseStart();
                            _rtc_console.Text = _textBuffer.ToString();
                            _textRenderSw.PulseStop();

                            _rtb_debug.WriteLine(_textRenderSw.ReportString());

                            //int xpos = _windowReader.iScreenBufferInfo.dwCursorPositionX;

                            //_rtc_console.SetConsoleSize(_windowReader.iScreenBufferInfo.srWindowRight + 1, _windowReader.iScreenBufferInfo.srWindowBottom + 1);
                        }

                        break;

                    case iPipe.PipeState.WaitingToSend:
                        _pipeServer.SendData(0, iPipe.PipeIo.Asynchronous);
                        break;

                    case iPipe.PipeState.Disconnected:  // reconnect?
                        _rtb_debug.WriteLine("Connecting...", Color.DarkOrange);
                        _pipeServer.Connect("iConsoleTest", iPipe.PipeType.Server, ConnectCallback);
                        _rtb_debug.WriteLine("Started connecting", Color.DarkOrange);
                        break;

                    case iPipe.PipeState.WaitingToRecv:
                    case iPipe.PipeState.Connecting:
                    case iPipe.PipeState.Broken:        // reconnect? 
                    default:
                        break;

                }
            }
            catch { throw; }

            _readTimer2.Enabled = true;
        }

        private void testToolStripMenuItem_Click(object a_sender, EventArgs a_e)
        {
            _pipeServer = new iPipe
            {
                PipeReadBufferSizeKb = 256,
                PipeWritBufferSizeKb = 256
            };

            _readTimer2.Enabled = true;
        }

        private void ConnectCallback()
        {
            if (InvokeRequired)
                Invoke(new iPipe.ConnectCallBack(ConnectCallback));
            else
            {
                _rtb_debug.WriteLine("Client has connected", Color.Red);
            }
        }

        private void SendCallback()
        {
            if (InvokeRequired)
                Invoke(new iPipe.DataWriteCallBack(SendCallback));
            else
            {
                _rtb_debug.WriteLine("Some bytes were written", Color.CadetBlue);
            }
        }

        int _readNumber;
        private void ReadCallback(int a_byteRead)
        {
            if (InvokeRequired)
                Invoke(new iPipe.DataReadCallBack(ReadCallback), a_byteRead);
            else
            {
                _rtb_debug.WriteLine(string.Format("{1} - Read {0} bytes", a_byteRead, _readNumber++), Color.Blue);
            }
        }
    }
}
