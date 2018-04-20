using System;
using System.ComponentModel;

namespace iUtils
{
	public class iDataReader
	{
		private readonly byte[] _data;

		public int Offset { get; protected set; }
		public int Length => _data.Length;

		public iDataReader(byte[] a_bytes)
		{
			_data = a_bytes;
		}

		public T Get<T>()
		{
			throw new NotImplementedException();
		}

		internal T1 GetNoIncrement<T1>()
		{
			throw new NotImplementedException();
		}
	}
}