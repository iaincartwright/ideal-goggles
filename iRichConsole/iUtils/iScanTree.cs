using System;
using System.Collections.Generic;
using System.IO;

namespace iUtils
{
  public delegate bool TraverseFileCallBack(FileInfo a_file, object a_userData);
  public delegate bool TraverseDirCallBack(DirectoryInfo a_file, object a_userData);

  public class iScanTree
  {
    TraverseDirCallBack _directoryCallBack;
    TraverseFileCallBack _fileCallBack;

    List<string> _fileGlobs;
    private object _userData = null;

    public iScanTree()
    {
      _directoryCallBack = DefaultDirCallBack;
      _fileCallBack = DefaultFileCallBack;
      _fileGlobs = new List<string>();
    }

    public iScanTree(string a_path, string a_glob)
      : this()
    {
      _fileGlobs.Add(a_glob);

      DoScan(a_path);
    }

    public List<string> FileGlobs
    {
      get => _fileGlobs;
	    set { _fileGlobs = value; }
    }

    public TraverseDirCallBack DirectoryCallBack
    {
      get => _directoryCallBack;
	    set { if (value == null) _directoryCallBack = DefaultDirCallBack; else _directoryCallBack = value; }
    }

    public TraverseFileCallBack FileCallBack
    {
      get => _fileCallBack;
	    set { if (value == null) _fileCallBack = DefaultFileCallBack; else _fileCallBack = value; }
    }

    public object UserData
    {
      set => _userData = value;
    }

    public void Scan(string a_path)
    {
      DoScan(a_path);
    }

    public void Scan(string a_path, object a_userData)
    {
      _userData = a_userData;

      DoScan(a_path);

      _userData = null;
    }

    public void Scan(params string[] a_paths)
    {
      foreach (string path in a_paths)
        DoScan(path);
    }

    private bool DefaultFileCallBack(FileInfo a_file, object a_userData)
    {
      return true;
    }

    private bool DefaultDirCallBack(DirectoryInfo a_file, object a_userData)
    {
      return true;
    }

    private void DoScan(string a_path)
    {
      foreach (string glob in _fileGlobs)
      {
        DoScan(a_path, glob);
      }
    }

    private void DoScan(string a_path, string a_glob)
    {
      if (Directory.Exists(a_path))
      {
        TraverseTree(DirectoryCallBack, FileCallBack, new DirectoryInfo(a_path), a_glob, _userData);
      }
      else
      {
        Console.WriteLine("Warning - path not found : " + a_path);
      }
    }

    private static void TraverseTree(TraverseDirCallBack a_directoryCallBack, TraverseFileCallBack a_fileCallBack, DirectoryInfo a_dirParent, string a_inFileGlob, object a_userData)
    {
      FileInfo[] theFiles = a_dirParent.GetFiles(a_inFileGlob);

      foreach (FileInfo file in theFiles)
      {
        if (a_fileCallBack(file, a_userData) == false)
          return;
      }

      DirectoryInfo[] theDirs = a_dirParent.GetDirectories();

      foreach (DirectoryInfo dir in theDirs)
      {
        if (a_directoryCallBack(dir, a_userData) == false)
          continue;

        TraverseTree(a_directoryCallBack, a_fileCallBack, dir, a_inFileGlob, a_userData);
      }
    }
  }
}