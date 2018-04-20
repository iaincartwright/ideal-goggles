// iStringTokeniser.h

#pragma once

using namespace System;

namespace iStringTokeniser 
{
	public ref class Splitter
	{
	public:
		static int DoSplit(array<wchar_t> ^ srcStr, array<short> ^ dstBuffer, int srcStart, int dstLen)
		{
			const wchar_t * seps = L" \n\r";
	
			pin_ptr<wchar_t> pString = &srcStr[0];
			
			wchar_t * token = wcstok(pString, seps); 

			// skip any leading tokens
			int skipCount = 0;
			while((skipCount < srcStart) && (token != NULL))
			{
				// next token
				token = wcstok(NULL, seps);
				
				skipCount++;
			}
			
			// now gather the actual data
			bool allZero = true;
			int dataCount = 0;
			while((dataCount < dstLen) && (token != NULL))
			{
				dstBuffer[dataCount] = _wtoi(token);

				if(dstBuffer[dataCount] != 0)
					allZero = false;

				// next token
				token = wcstok(NULL, seps);
				
				dataCount++;
			}
			
			// if the are all zeros then return 1 otherwise return the dataCount
			return allZero ? 1 : dataCount;
		}
	};
}


