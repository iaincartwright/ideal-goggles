using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.IsolatedStorage;

namespace WorldProcessor
{
	static class iIsoStore
	{
		static readonly IsolatedStorageFile s_isoStore;
		
		static iIsoStore() 
		{
			s_isoStore = IsolatedStorageFile.GetMachineStoreForAssembly();
		}

		public static bool DirectoryExists(string a_name)
		{
			return s_isoStore.DirectoryExists(a_name);
		}

		public static bool FileExists(string a_name)
		{
			return s_isoStore.FileExists(a_name);
		}

		public static void CreateDirectory(string a_name)
		{
			s_isoStore.CreateDirectory(a_name);
		}

		public static Stream FileStream(string a_name, FileMode a_mode)
		{
			return s_isoStore.OpenFile(a_name, a_mode);
		}

		public static Stream FileStreamXWrite(string a_name)
		{
			return new GZipStream(s_isoStore.OpenFile(a_name, FileMode.Create), CompressionMode.Compress);
		}

		public static Stream FileStreamXRead(string a_name)
		{
			return new GZipStream(s_isoStore.OpenFile(a_name, FileMode.Open), CompressionMode.Decompress);
		}

		public static IEnumerable<string> GetFileNames(string a_name)
		{
			return s_isoStore.GetFileNames(a_name);
		}
	}
}
