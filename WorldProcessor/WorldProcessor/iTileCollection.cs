using System;
using System.Collections.Generic;

namespace WorldProcessor
{
	class iTileCollection : iTile
	{
		//=============================================================================
		// privates
		List<string> _files;
		readonly List<iTile> _allTiles;

		//=============================================================================
		// directory in isolated storage name
		public string Name { get; private set; }
		public int Count { get { return _allTiles.Count; } }

		//=============================================================================

		public iTileCollection(string a_collectionName)
		{
			Name = a_collectionName.ToLowerInvariant();

			_allTiles = new List<iTile>(256);
		}

		//=============================================================================

		public void LoadCollection()
		{
			try
			{
				if (iIsoStore.DirectoryExists(Name))
				{
					_files = new List<string>(iIsoStore.GetFileNames(Name + "\\*.bin"));

					foreach (var filename in _files)
					{
						var tile = Deserialize(Name + "\\" + filename);
						
						_allTiles.Add(tile);

						tile.ClearData();
					}

					if(_allTiles.Count > 0)
						// set my values to this tile
						ShallowCopyFrom(_allTiles[0]);
				}
			}
			catch (Exception ex)
			{
				ErrorSet("Load Collection", ex.Message);
			}
		}

		//=============================================================================

		private static void ErrorSet(string a_context, string a_message)
		{
			Console.Write(a_context + ": " + a_message);
		}

		//=============================================================================
	}
}
