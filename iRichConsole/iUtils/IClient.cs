namespace iUtils
{
  public interface IClient
  {
    event iDatatRecievedEventHandler DataRecieved;
    event iDisconnectEventHandler Disconnect;
    event iErrorEventHandler ClientError;

    IListener Listener { get; }

    IClient Client { get; }

    string Error { get; }

    bool IsConnected { get; }

    byte[] Buffer { get; }

    int DataLength { get; set; }

    void Close();

    bool Send(byte[] a_data, int a_length);

    string ToString();
  }
}