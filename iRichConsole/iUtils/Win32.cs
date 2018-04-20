using System.Runtime.InteropServices;

namespace iUtils
{
  public class Win32
  {
    [DllImport("kernel32.dll")]
    public static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    public static extern bool FreeConsole();
  }
}