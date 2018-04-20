#include "StdAfx.h"

#include <stdio.h>

#include "iConsoleWindowReader.h"

namespace iCppLib
{
	//****************************************************************************
	iConsoleWindowReader::iConsoleWindowReader()
	{
		_screenBufferInfo = new CONSOLE_SCREEN_BUFFER_INFO();

		_bufferSize = new COORD();

		iScreenBufferInfo = gcnew iCONSOLE_SCREEN_BUFFER_INFO();

		iCharInfo = gcnew iCHAR_INFO_EX();

		ColourTable = gcnew array<unsigned int>(16);

		_bufferOne = 0;
		_bufferTwo = 0;
		_deltas = 0;

		// total size of the screen buffer
		_maxBufferChars = 0;

		// valid records in the delta array
		_recordCount = 0;
	}

	//*****************************************************************************
	iConsoleWindowReader::~iConsoleWindowReader(void)
	{
		this->!iConsoleWindowReader();
	}

	//*****************************************************************************
	iConsoleWindowReader::!iConsoleWindowReader(void)
	{
		delete _screenBufferInfo;
		delete _bufferOne;
		delete _bufferTwo;
		delete _deltas;
		delete _bufferSize;

		_screenBufferInfo = 0;
		_bufferOne = 0;
		_bufferTwo = 0;
		_deltas = 0;
		_bufferSize = 0;
	}

	//*****************************************************************************
	bool iConsoleWindowReader::Update(void)
	{
		static int rebuilder = 1;

		bool retValue = false;

		// get a handle to the current screen buffer
		// if you cache this hadle it may point to an inactive buffer!
		HANDLE _handle = CreateFileA("CONOUT$", GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);

		if (++rebuilder % 60 == 0)
		{
			memset(_prevBuffer, 0, sizeof(CHAR_INFO) * _maxBufferChars);
			memset(_currBuffer, 0, sizeof(CHAR_INFO) * _maxBufferChars);

			CONSOLE_SCREEN_BUFFER_INFOEX infoEx;

			infoEx.cbSize = sizeof(CONSOLE_SCREEN_BUFFER_INFOEX);

			BOOL retVal = GetConsoleScreenBufferInfoEx(_handle, &infoEx);

			SetColourTable((unsigned int *)infoEx.ColorTable);
		}
		else
		{
			if (GetConsoleScreenBufferInfo(_handle, _screenBufferInfo))
			{
				// if necessary rebuild our buffers
				RebuildBuffers();

				// preserve the last buffer
				SwapBuffers();

				// read to the origin of our buffer
				COORD _bufferCoord;
				_bufferCoord.X = 0;
				_bufferCoord.Y = 0;

				if (ReadConsoleOutputA(_handle, _currBuffer, *_bufferSize, _bufferCoord, &_screenBufferInfo->srWindow))
					retValue = true;
			}
		}

		CloseHandle(_handle);

		return retValue;
	}

	//*****************************************************************************
	int iConsoleWindowReader::UpdateDeltas()
	{
		// Deltas list index for next change record
		_recordCount = 0;

		// check each of the character cells in the screen window
		// and update the list with all cells that have changed
		for (int i = 0; i < _maxBufferChars; i++)
		{
			// this relies on CHAR_INFO being the same size as an int
			if (reinterpret_cast<int *>(_currBuffer)[i] != reinterpret_cast<int *>(_prevBuffer)[i])
			{
				_deltas[_recordCount].Attributes = _currBuffer[i].Attributes;
				_deltas[_recordCount].UnicodeChar = (WORD)_currBuffer[i].Char.AsciiChar;
				_deltas[_recordCount].Offset = i;

				//if(_currBuffer[i].Char.AsciiChar > 0x80)
				//{
				//	printf("");
				//}
				//
				//if(_currBuffer[i].Char.AsciiChar < 0x0020)
				//{
				//	printf("");
				//}

				_recordCount++;
			}
		}

		_deltas[_recordCount].Offset = -1;  // signal end of changes

		return _recordCount;
	}

	//*****************************************************************************
	int iConsoleWindowReader::Encode(array<unsigned char> ^ ioBuffer)
	{
		int bufferOffset = 0;

		EncodeBytes(ioBuffer, bufferOffset, _screenBufferInfo, 1);

		int numRecords = _recordCount;
		EncodeBytes(ioBuffer, bufferOffset, &numRecords, 1);
		_recordCount = numRecords;

		EncodeBytes(ioBuffer, bufferOffset, _deltas, _recordCount);

		EncodeArray(ioBuffer, bufferOffset, ColourTable);

		return bufferOffset;
	}

	//****************************************************************************
	int iConsoleWindowReader::Decode(array<unsigned char> ^ ioBuffer)
	{
		int bufferOffset = 0;

		DecodeBytes(ioBuffer, bufferOffset, _screenBufferInfo, 1);

		RebuildBuffers();

		int numRecords = _recordCount;
		DecodeBytes(ioBuffer, bufferOffset, &numRecords, 1);
		_recordCount = numRecords;

		DecodeBytes(ioBuffer, bufferOffset, _deltas, _recordCount);

		DecodeArray(ioBuffer, bufferOffset, ColourTable);

		return bufferOffset;
	}

	//****************************************************************************
	template <class T>
	inline void iConsoleWindowReader::EncodeBytes(array<unsigned char>^ ioBuffer, int & offset, T * value, int count)
	{
		interior_ptr<T> ptr = reinterpret_cast<interior_ptr<T>>(&ioBuffer[offset]);

		for (int i = 0; i < count;i++)
		{
			*(ptr + i) = value[i];

			offset += sizeof(T);
		}
	}

	//****************************************************************************
	template <class T>
	inline void iConsoleWindowReader::DecodeBytes(array<unsigned char>^ ioBuffer, int & offset, T * value, int count)
	{
		interior_ptr<T> p2 = reinterpret_cast<interior_ptr<T>>(&ioBuffer[offset]);

		while (count > 0)
		{
			offset += sizeof(T);

			*value = *p2;

			count--;
		}
	}

	//****************************************************************************
	void iConsoleWindowReader::EncodeArray(array<unsigned char> ^ ioBuffer, int & offset, array<unsigned int> ^ data)
	{
		interior_ptr<unsigned int> pBuffer = reinterpret_cast<interior_ptr<unsigned int>>(&ioBuffer[offset]);

		for (int i = 0; i < data->Length; i++)
		{
			*pBuffer = data[i];

			pBuffer++;

			offset += sizeof(unsigned int);
		}
	}

	//****************************************************************************

	void iConsoleWindowReader::DecodeArray(array<unsigned char> ^ ioBuffer, int & offset, array<unsigned int> ^ data)
	{
		interior_ptr<unsigned int> pBuffer = reinterpret_cast<interior_ptr<unsigned int>>(&ioBuffer[offset]);

		for (int i = 0; i < data->Length; i++)
		{
			data[i] = *pBuffer;

			pBuffer++;

			offset += sizeof(unsigned int);
		}
	}

	//****************************************************************************
} //namespace iCppLib