#include "iUnmanagedHelper.h"
#include "windows.h"

iUnmanagedHelper::iUnmanagedHelper(void)
{
}

iUnmanagedHelper::~iUnmanagedHelper(void)
{
}

COORD bufferSize;
COORD bufferCoord;

bool ConsoleWindowUpdate(CONSOLE_SCREEN_BUFFER_INFO * _screenBufferInfo, CHAR_INFO * _currBuffer, SMALL_RECT * _currRect)
{
		bool retValue = false;

		COORD * _bufferSize = &bufferSize;
		COORD * _bufferCoord = &bufferCoord;

		HANDLE _handle = CreateFileA("CONOUT$", GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);

		if(GetConsoleScreenBufferInfo(_handle, _screenBufferInfo))
		{
			// say that our read buffer is the same dimensions as
			// our screen buffer
			_bufferSize->X = _screenBufferInfo->dwSize.X;
			_bufferSize->Y = _screenBufferInfo->dwSize.Y;

#define READ_WINDOW
#if defined(READ_WINDOW)
			// read to where the screen would be in our buffer
			_bufferCoord->X = _screenBufferInfo->srWindow.Left;
			_bufferCoord->Y = _screenBufferInfo->srWindow.Top;

			// read the window size
			*_currRect = _screenBufferInfo->srWindow;
#else
			// read the entire screen buffer each time
			_bufferCoord->X = 0;
			_bufferCoord->Y = 0;

			_currRect->Top = 0;
			_currRect->Left = 0;
			_currRect->Right = _bufferSize->X - 1;
			_currRect->Bottom = (_bufferSize->Y > 120 ? 120 : _bufferSize->Y) - 1;
#endif
			if(ReadConsoleOutput(_handle, _currBuffer, *_bufferSize, *_bufferCoord, _currRect))
				retValue = true;
		}

		CloseHandle(_handle);

		return retValue;
}