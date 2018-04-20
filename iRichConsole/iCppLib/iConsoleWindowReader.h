#pragma once

#using <mscorlib.dll>
#include "windows.h"

using namespace System;
using namespace System::Collections::Generic;

namespace iCppLib
{
	public ref class iCONSOLE_SCREEN_BUFFER_INFO
	{
	public:
		iCONSOLE_SCREEN_BUFFER_INFO()
		{
		}

		void CopyToThis(CONSOLE_SCREEN_BUFFER_INFO * pscrBuff)
		{
			dwSizeX = pscrBuff->dwSize.X;
			dwSizeY = pscrBuff->dwSize.Y;
			dwCursorPositionX = pscrBuff->dwCursorPosition.X;
			dwCursorPositionY = pscrBuff->dwCursorPosition.Y;
			dwMaximumWindowSizeX = pscrBuff->dwMaximumWindowSize.X;
			dwMaximumWindowSizeY = pscrBuff->dwMaximumWindowSize.Y;
			wAttributes = pscrBuff->wAttributes;
			srWindowLeft = pscrBuff->srWindow.Left;
			srWindowRight = pscrBuff->srWindow.Right;
			srWindowTop = pscrBuff->srWindow.Top;
			srWindowBottom = pscrBuff->srWindow.Bottom;
		};

		unsigned short dwSizeX;
		unsigned short dwSizeY;
		unsigned short dwCursorPositionX;
		unsigned short dwCursorPositionY;
		unsigned short wAttributes;
		unsigned short srWindowLeft;
		unsigned short srWindowTop;
		unsigned short srWindowRight;
		unsigned short srWindowBottom;
		unsigned short dwMaximumWindowSizeX;
		unsigned short dwMaximumWindowSizeY;
	};

	public struct CHAR_INFO_EX
	{
		int Offset;
		WORD UnicodeChar;
		WORD Attributes;
	};

	public ref class iCHAR_INFO_EX
	{
	public:
		void CopyToThis(CHAR_INFO_EX * pInfo)
		{
			Offset = pInfo->Offset;
			Attributes = pInfo->Attributes;
			UnicodeChar = (pInfo->UnicodeChar & 0xff);
		};

		property byte RedB {byte get() { return Attributes & BACKGROUND_RED ? (Attributes & BACKGROUND_INTENSITY ? 0xff : 0x7f) : 0; }}
		property byte GrnB {byte get() { return Attributes & BACKGROUND_GREEN ? (Attributes & BACKGROUND_INTENSITY ? 0xff : 0x7f) : 0; }}
		property byte BluB {byte get() { return Attributes & BACKGROUND_BLUE ? (Attributes & BACKGROUND_INTENSITY ? 0xff : 0x7f) : 0; }}

		property byte RedF {byte get() { return Attributes & FOREGROUND_RED ? (Attributes & FOREGROUND_INTENSITY ? 0xff : 0x7f) : 0; }}
		property byte GrnF {byte get() { return Attributes & FOREGROUND_GREEN ? (Attributes & FOREGROUND_INTENSITY ? 0xff : 0x7f) : 0; }}
		property byte BluF {byte get() { return Attributes & FOREGROUND_BLUE ? (Attributes & FOREGROUND_INTENSITY ? 0xff : 0x7f) : 0; }}

		int Offset;
		WORD UnicodeChar;
		WORD Attributes;
	};

	public ref class iConsoleWindowReader
	{
	public:
		iConsoleWindowReader(void);
		~iConsoleWindowReader(void);
		!iConsoleWindowReader(void);

		bool Update(void);
		int UpdateDeltas(void);

		int Encode(array<unsigned char> ^ ioBuffer);
		int Decode(array<unsigned char> ^ ioBuffer);

		iCONSOLE_SCREEN_BUFFER_INFO ^ iScreenBufferInfo;
		iCHAR_INFO_EX ^ iCharInfo;

		iCHAR_INFO_EX ^ GetCharDiff(int index)
		{
			iCharInfo->CopyToThis(&_deltas[index]);

			return iCharInfo;
		}

		array<unsigned int> ^ ColourTable;

	private:
		CONSOLE_SCREEN_BUFFER_INFO * _screenBufferInfo;
		COORD * _bufferSize;

		CHAR_INFO * _bufferOne;
		CHAR_INFO * _bufferTwo;
		CHAR_INFO * _currBuffer;
		CHAR_INFO * _prevBuffer;

		CHAR_INFO_EX * _deltas;

		int _maxBufferChars;	// maximum total screen buffer size (size of _bufferOne & _bufferTwo)
		int _recordCount;		// number of valid records in the _deltas array

	public:
		property int RecordCount {int get() { return _recordCount; }}

	private:
		void SetColourTable(unsigned int pColourTable[16])
		{
			for (int i = 0;i<ColourTable->Length;i++)
			{
				ColourTable[i] = pColourTable[i];
			}
		}

		void RebuildBuffers()
		{
			iScreenBufferInfo->CopyToThis(_screenBufferInfo);

			// say that our read buffer will be the same dimensions as
			// our window buffer
			_bufferSize->X = _screenBufferInfo->srWindow.Right + 1 - _screenBufferInfo->srWindow.Left;
			_bufferSize->Y = _screenBufferInfo->srWindow.Bottom + 1 - _screenBufferInfo->srWindow.Top;

			if (_bufferSize->X * _bufferSize->Y > _maxBufferChars)
			{
				delete _bufferOne;
				delete _bufferTwo;
				delete _deltas;

				_maxBufferChars = _bufferSize->X * _bufferSize->Y;

				_bufferOne = new CHAR_INFO[_maxBufferChars];
				_bufferTwo = new CHAR_INFO[_maxBufferChars];

				memset(_bufferOne, 0, sizeof(CHAR_INFO) * _maxBufferChars);
				memset(_bufferTwo, 0, sizeof(CHAR_INFO) * _maxBufferChars);

				// make sure we are pointing
				// to our new nuffers
				_currBuffer = _bufferOne;
				_prevBuffer = _bufferTwo;

				_deltas = new CHAR_INFO_EX[_maxBufferChars + 1];
				_deltas[0].Offset = -1;
				_deltas[_maxBufferChars].Offset = -1;

				// no valid records any more
				_recordCount = 0;
			}
		}

		void SwapBuffers()
		{
			if (_currBuffer == _bufferOne)
			{
				_currBuffer = _bufferTwo;
				_prevBuffer = _bufferOne;
			}
			else
			{
				_currBuffer = _bufferOne;
				_prevBuffer = _bufferTwo;
			}
		}

		void EncodeArray(array<unsigned char> ^ ioBuffer, int & offset, array<unsigned int> ^ data);
		void DecodeArray(array<unsigned char> ^ ioBuffer, int & offset, array<unsigned int> ^ data);

		template <class T>
		void EncodeBytes(array<unsigned char>^ ioBuffer, int & offset, T * value, int count);
		template <class T>
		void DecodeBytes(array<unsigned char>^ ioBuffer, int & offset, T * value, int count);
	};

}