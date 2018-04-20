using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iUtils
{
	public class iRunLengthDecoder
	{
		bool _doRaw;
		bool _doRle;
		int _runLength;

		readonly iDataReader _dataReader;

		//===============================================================================

		public iRunLengthDecoder(iDataReader a_dataReader)
		{
			_dataReader = a_dataReader;

			_doRaw = false;
			_doRle = false;
			_runLength = 0;
		}

		//===============================================================================

		public ushort GetNextU16()
		{
			return GetNext<ushort>();
		}

		//===============================================================================

		public uint GetNextU32()
		{
			return GetNext<uint>();
		}

		//===============================================================================

		public T GetNext<T>()
		{
			T retValue;

			if (_doRaw)
			{
				retValue = _dataReader.Get<T>();

				if (_runLength == 1)
					_doRaw = false;
				else
					_runLength--;
			}
			else if (_doRle)
			{
				if (_runLength == 1)
				{
					retValue = _dataReader.Get<T>();

					_doRle = false;
				}
				else
				{
					retValue = _dataReader.GetNoIncrement<T>();

					_runLength--;
				}
			}
			else
			{
				byte runLength = _dataReader.Get<byte>();

				if ((runLength & 0x80) != 0)
					_doRle = true;
				else
					_doRaw = true;

				_runLength = runLength & 0x7F;

				return GetNext<T>();
			}

			return retValue;
		}

		//===============================================================================
	}
}
