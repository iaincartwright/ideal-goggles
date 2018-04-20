using System;

namespace iUtils
{
  public delegate void iConnectEventHandler(IListener a_sender, IClient a_client);
  public delegate void iDisconnectEventHandler(IClient a_client);
  public delegate void iDatatRecievedEventHandler(IClient a_client);
  public delegate void iErrorEventHandler(object a_sender);

  public interface IListener
  {
    event iConnectEventHandler Connect;
    event iErrorEventHandler ListenError;

    string Error { get; }

    bool Listen();

    void ShutDown();
  }
}