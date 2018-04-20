using System;
using System.IO;

namespace iUtils
{
	public class iFileOps
	{
		public static void Copy(string a_src, string a_dst, bool a_create)
		{
			DoCopy(a_src, a_dst, a_create);
		}

		public static void Copy(string a_src, string a_dst)
		{
			DoCopy(a_src, a_dst, false);
		}

		public static void Touch(string a_src)
		{
			if (File.Exists(a_src))
			{
				var attribs = File.GetAttributes(a_src);

				if ((attribs & FileAttributes.ReadOnly) != 0)
				{
					File.SetAttributes(a_src, attribs & ~FileAttributes.ReadOnly);
				}

				File.SetLastWriteTime(a_src, DateTime.Now);

				if ((attribs & FileAttributes.ReadOnly) != 0)
				{
					File.SetAttributes(a_src, attribs);
				}
			}
			else
				File.Create(a_src).Close();
		}

		public static void MakeRw(string a_src)
		{
			if (File.Exists(a_src))
			{
				if ((File.GetAttributes(a_src) & FileAttributes.ReadOnly) != 0)
				{
					File.SetAttributes(a_src, FileAttributes.Normal);
				}
			}
		}

		public static void MakeRo(string a_src)
		{
			if (File.Exists(a_src))
			{
				if ((File.GetAttributes(a_src) & FileAttributes.ReadOnly) == 0)
				{
					File.SetAttributes(a_src, FileAttributes.ReadOnly);
				}
			}
		}

		public static void Rename(string a_src, string a_dst, bool a_overwrite)
		{
			try
			{
				if (a_overwrite && File.Exists(a_dst))
				{
					File.SetAttributes(a_dst, FileAttributes.Normal);

					File.Delete(a_dst);
				}

				File.Move(a_src, a_dst);
			}
			catch (Exception ex)
			{
				iLog.LogError("Rename error :\nSource - {0}\nDest  - {1}\nError : {2}", a_src, a_dst, ex.Message);
			}
		}

		public static void Delete(string a_path)
		{
			if (IsGlob(a_path))
			{
			}
			else if (IsDir(a_path))
			{
				Directory.Delete(a_path, true);
			}
			else
			{
				// assume it's a file
				if (File.Exists(a_path))
				{
					if ((File.GetAttributes(a_path) & FileAttributes.ReadOnly) != 0)
					{
						File.SetAttributes(a_path, FileAttributes.Normal);
					}

					File.Delete(a_path);
				}
			}
		}

		static readonly char[] s_sep = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

		/// <summary>
		/// Pass in two full paths and generate a relative path
		/// </summary>
		/// <param name="a_fromFolder"></param>
		/// <param name="a_toFolder"></param>
		/// <returns></returns>
		public static string GetRelativePath(string a_fromFolder, string a_toFolder)
		{
			a_fromFolder = a_fromFolder.ToLowerInvariant();
			a_toFolder = a_toFolder.ToLowerInvariant();

			if (Path.IsPathRooted(a_fromFolder) && Path.IsPathRooted(a_toFolder))
			{
				if (a_fromFolder[0] == a_toFolder[0])
				{
					var curr = a_fromFolder.Split(s_sep, StringSplitOptions.RemoveEmptyEntries);
					var abs = a_toFolder.Split(s_sep, StringSplitOptions.RemoveEmptyEntries);

					var inCommon = 1;

					for (; inCommon < Math.Min(curr.Length, abs.Length); inCommon++)
						if (curr[inCommon] != abs[inCommon])
							break;

					var relativePath = "";

					for (var i = 0; i < curr.Length - inCommon; i++)
						relativePath += "..\\";

					for (var i = inCommon; i < abs.Length; i++)
						relativePath += (abs[i] + "\\");

					relativePath = relativePath.TrimEnd('\\');

					if (relativePath == "")
						return ".";
					else
						return relativePath;
				}
			}

			return a_toFolder;
		}

		private static bool IsGlob(string a_path)
		{
			return a_path.Contains("*") || a_path.Contains("?");
		}

		private static bool IsDir(string a_path)
		{
			return a_path.EndsWith("\\") || Directory.Exists(a_path);
		}

		private static string ExtractDir(string a_path)
		{
			var dir = Path.GetDirectoryName(a_path);

			if (string.IsNullOrEmpty(dir))
				dir = a_path;

			return dir;
		}

		private static string ExtractName(string a_path)
		{
			return Path.GetFileName(a_path);
		}

		public static void MakeDir(string a_newDir)
		{
			if (Directory.Exists(a_newDir) == false)
			{
				Directory.CreateDirectory(a_newDir);
			}
		}

		public static void FileToString(string a_filename, out string[] a_lines)
		{
			a_lines = new[] { a_filename + " does not exist" };

			if (File.Exists(a_filename))
				a_lines = File.ReadAllLines(a_filename);
		}

		public static string FileToString(string a_filename)
		{
			var result = a_filename + " does not exist";

			if (File.Exists(a_filename))
				result = File.ReadAllText(a_filename);

			return result;
		}

		public static void StringToFile(string a_filename, string a_outString)
		{
			MakeRw(a_filename);

			File.WriteAllText(a_filename, a_outString);
		}

		public static void StringToFile(string a_filename, string[] a_lines)
		{
			MakeRw(a_filename);

			File.WriteAllLines(a_filename, a_lines);
		}

		private static void DoCopy(string a_src, string a_dst, bool a_create)
		{
			//decompose the source paths
			var dirSrc = ExtractDir(a_src);
			var fileSrc = ExtractName(a_src);

			if (string.IsNullOrEmpty(fileSrc))
				fileSrc = "*.*";

			// if source is a glob or dest is a dir only
			if (IsGlob(fileSrc) || IsDir(a_dst))
			{
				// it's a glob copy so use dst unchanged as dest path
				var dirDst = a_dst;

				// create the destination directory if requested
				if (a_create)
					MakeDir(dirDst);

				var dirSrcInfo = new DirectoryInfo(dirSrc);

				foreach (var srcFile in dirSrcInfo.GetFiles(fileSrc))
				{
					var tgtFile = dirDst + "\\" + srcFile.Name;

					iLog.LogError("Copying :\n{0}\n{1}", srcFile.FullName, tgtFile);

					try
					{
						MakeRw(tgtFile);

						srcFile.CopyTo(tgtFile, true);
					}
					catch (Exception ex)
					{
						iLog.LogError("Error copying {0} to {1} : {2}", srcFile.FullName, tgtFile, ex.Message);
					}
				}
			}
			else
			{
				var dirDst = ExtractDir(a_dst);
				var fileDst = ExtractName(a_dst);

				// create the destination directory if requested
				if (a_create)
					MakeDir(dirDst);

				var srcFile = dirSrc + "\\" + fileSrc;
				var tgtFile = dirDst + "\\" + fileDst;

				iLog.LogError("Copying :\n{0}\n{1}", srcFile, tgtFile);

				try
				{
					MakeRw(tgtFile);

					File.Copy(srcFile, tgtFile, true);
				}
				catch (Exception ex)
				{
					iLog.LogError("Error on copy : {0}", ex.Message);
				}
			}
		}
	}
}