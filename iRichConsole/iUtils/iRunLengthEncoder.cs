using System.Collections.Generic;
using System.Globalization;

namespace iUtils
{
	public class iRunLengthEncoder
	{
		public string Compress(string a_stringToCompress)
		{
			var result = string.Empty;
			var previousLetter = ' ';
			var count = 0;

			const char controlCharacter = '\x8000';

			foreach (var currentLetter in a_stringToCompress)
			{
				if (currentLetter != previousLetter)
				{
					if (count > 1)
						result += controlCharacter + count.ToString(CultureInfo.InvariantCulture) + currentLetter;
					else
						result += currentLetter;

					count = 0;
				}
				else
				{
					count++;
				}

				previousLetter = currentLetter;
			}

			return result;
		}

		public T [] Compress<T>(IEnumerable<T> a_toCompress)
		{
			var result = new List<T>();
			var previous = default(T);
			var count = 0;

			foreach (var current in a_toCompress)
			{
				if (current.Equals(previous))
				{
					if(count > 1)
						result.Add(current);

					count = 0;
				}
				else
				{
					count++;
				}

				previous = current;
			}

			return result.ToArray();
		}
	}
}