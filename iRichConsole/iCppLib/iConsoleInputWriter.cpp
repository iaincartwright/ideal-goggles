#include "StdAfx.h"

#include <stdio.h>

#include "iConsoleInputWriter.h"

namespace iCppLib
{
	//****************************************************************************

	const int MAX_WRITE_CHARS = 1024;

	iConsoleInputWriter::iConsoleInputWriter(IntPtr handle)
	{
		_handle = handle.ToPointer();

		_inputBuffer = new INPUT_RECORD[MAX_WRITE_CHARS];

		_eventsToWrite = 0;
	}

	//****************************************************************************

	int iConsoleInputWriter::Update()
	{
		DWORD eventsWritten = 0;

		if(_eventsToWrite <= 0)
			return 0;

		if(WriteConsoleInput(_handle, _inputBuffer, _eventsToWrite, &eventsWritten))
		{
			if(eventsWritten == _eventsToWrite)
			{
				_eventsToWrite = 0;

				return eventsWritten;
			}
		}

		return -1;
	}

	//****************************************************************************

	void iConsoleInputWriter::AddKeyEvent(bool keyDown, int repeatCount, int virtKeyCode, int virtScanCode, wchar_t unicodeChar, int controlKey)
	{
		if(_eventsToWrite >= MAX_WRITE_CHARS)
			Update();

		_inputBuffer[_eventsToWrite].EventType = KEY_EVENT;

		_inputBuffer[_eventsToWrite].Event.KeyEvent.bKeyDown = keyDown ? TRUE : FALSE;
		_inputBuffer[_eventsToWrite].Event.KeyEvent.wRepeatCount = repeatCount;
		_inputBuffer[_eventsToWrite].Event.KeyEvent.wVirtualKeyCode = virtKeyCode;
		_inputBuffer[_eventsToWrite].Event.KeyEvent.wVirtualScanCode = virtScanCode;
		_inputBuffer[_eventsToWrite].Event.KeyEvent.uChar.UnicodeChar = unicodeChar;
		_inputBuffer[_eventsToWrite].Event.KeyEvent.dwControlKeyState = controlKey;

		_eventsToWrite++;
	}

	//****************************************************************************

	void iConsoleInputWriter::AddMouseEvent(int xpos, int ypos, int buttonState, int controlKey, int eventFlags)
	{
		if(_eventsToWrite >= MAX_WRITE_CHARS)
			Update();

		_inputBuffer[_eventsToWrite].EventType = MOUSE_EVENT;

		_inputBuffer[_eventsToWrite].Event.MouseEvent.dwMousePosition.X = xpos;
		_inputBuffer[_eventsToWrite].Event.MouseEvent.dwMousePosition.Y = ypos;
		_inputBuffer[_eventsToWrite].Event.MouseEvent.dwButtonState = buttonState;
		_inputBuffer[_eventsToWrite].Event.MouseEvent.dwControlKeyState = controlKey;
		_inputBuffer[_eventsToWrite].Event.MouseEvent.dwEventFlags = eventFlags;

		_eventsToWrite++;
	}

	//****************************************************************************

	void iConsoleInputWriter::AddResizeEvent(int xsize, int ysize)
	{
		if(_eventsToWrite >= MAX_WRITE_CHARS)
			Update();

		_inputBuffer[_eventsToWrite].EventType = WINDOW_BUFFER_SIZE_EVENT;

		_inputBuffer[_eventsToWrite].Event.WindowBufferSizeEvent.dwSize.X = xsize;
		_inputBuffer[_eventsToWrite].Event.WindowBufferSizeEvent.dwSize.Y = ysize;

		_eventsToWrite++;
	}

	//****************************************************************************
}