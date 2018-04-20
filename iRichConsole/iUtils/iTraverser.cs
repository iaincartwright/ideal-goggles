using System.IO;

namespace iUtils
{
  public class iTraverser
  {
    delegate bool TraverseFileCallBack(FileInfo a_file);
    delegate bool TraverseDirCallBack(DirectoryInfo a_file);

    void TraverseTree(TraverseDirCallBack a_dirCallBack, TraverseFileCallBack a_fileCallBack, DirectoryInfo a_dirParent, string[] a_inFileGlobs)
    {
      foreach (string inFileGlob in a_inFileGlobs)
      {
        FileInfo[] theFiles = a_dirParent.GetFiles(inFileGlob);

        foreach (FileInfo file in theFiles)
        {
          if (a_fileCallBack(file) == false)
            return;
        }
      }

      DirectoryInfo[] theDirs = a_dirParent.GetDirectories();

      foreach (DirectoryInfo dir in theDirs)
      {
        if (a_dirCallBack != null && a_dirCallBack(dir) == false)
          continue;

        TraverseTree(a_dirCallBack, a_fileCallBack, dir, a_inFileGlobs);
      }
    }
  }
}