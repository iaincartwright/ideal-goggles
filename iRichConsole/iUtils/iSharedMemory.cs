using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace iUtils
{
	public class iSharedMemory : IDisposable
	{
		private readonly MemoryMappedFile _mappedFile;

		public MemoryMappedViewAccessor Data { get; private set; }

		public iSharedMemory(string a_memoryName)
		{
			_mappedFile = MemoryMappedFile.OpenExisting(a_memoryName, MemoryMappedFileRights.ReadWrite);

			Data = _mappedFile.CreateViewAccessor();
		}

		public iSharedMemory(string a_memoryName, Int64 a_memoryLength)
		{
			_mappedFile = MemoryMappedFile.CreateNew(a_memoryName, a_memoryLength, MemoryMappedFileAccess.ReadWrite);

			Data = _mappedFile.CreateViewAccessor();
		}

		private bool IsDisposed { get; set; }

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool a_isDisposing)
		{
			try
			{
				if (!IsDisposed)
				{
					if (a_isDisposing)
					{
						// Release all managed resources here
						_mappedFile.Dispose();
						Data.Dispose();
					}

					// Release all unmanaged resources here
				}
			}
			finally
			{
				IsDisposed = true;
			}
		}
	}
}
