#pragma once

#using <mscorlib.dll>
#include "windows.h"

using namespace System;
using namespace System::Collections::Generic;

namespace iCppLib
{
	public ref class iConsoleInputWriter
	{
	public:
		iConsoleInputWriter(IntPtr handle);

		int Update(void);
		void AddKeyEvent(bool keyDown, int repeatCount, int virtKeyCode, int virtScanCode, wchar_t unicodeChar, int controlKey);
		void AddMouseEvent(int xpos, int ypos, int buttonState, int controlKey, int eventFlags);
		void AddResizeEvent(int xsize, int ysize);

	private:
		HANDLE		_handle;

		INPUT_RECORD * _inputBuffer;

		int _eventsToWrite;
		int _refreshTimeMS;

	public:
		property int RefreshTimeMS
		{
			int get() { return _refreshTimeMS; }
			void set(int value) { _refreshTimeMS = value; }
		}
	};
}