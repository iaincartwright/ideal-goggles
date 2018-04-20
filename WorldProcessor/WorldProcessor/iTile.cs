using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace WorldProcessor
{
	[Serializable]
	class iTile
	{
		// these are the properties from the source (.asc) file
		public string SourceFile { get; protected set; }
		public int RowCountSrc { get; protected set; }
		public int ColCountSrc { get; protected set; }
		public double LatSrc { get; protected set; } // source data set, in degrees
		public double LonSrc { get; protected set; } // source data set, in degrees
		public double WidthSrc => CellSize * ColCountSrc; // source data set, in degrees
		public double HeightSrc => CellSize*RowCountSrc; // source data set, in degrees

		// these are the properties of the tile
		public string TileFile { get; protected set; }
		public int RowStart { get; protected set; }
		public int ColStart { get; protected set; }
		public int RowCount { get; protected set; }
		public int ColCount { get; protected set; }
		public double Lat { get; protected set; }	// this data set, in degrees
		public double Lon { get; protected set; }	// this data set, in degrees
		public double CellSize { get; protected set; } // in degrees
		public float NoDataValue { get; protected set; }

		public double Width => CellSize * ColCount; // this data set, in degrees
		public double Height => CellSize * RowCount; // this data set, in degrees

		public Dictionary<string, string> Properties { get; protected set; }
		public List<short[]> CellData { get; protected set; }
		public bool DataLoaded { get { return CellData != null; } }
		public void ClearData() { CellData = null; }

		//=========================================================================

		protected iTile()
		{
		}

		//=========================================================================

		protected iTile(string a_filename)
		{
			ShallowCopyFrom(Deserialize(a_filename));
		}

		//=========================================================================

		public iTile(iTile a_from)
		{
			ShallowCopyFrom(a_from);
		}

		//=========================================================================

		protected void ShallowCopyFrom(iTile a_from)
		{
			SourceFile = a_from.SourceFile;
			RowCountSrc = a_from.RowCountSrc;
			ColCountSrc = a_from.ColCountSrc;
			LatSrc = a_from.LatSrc;
			LonSrc = a_from.LonSrc;
			TileFile = a_from.TileFile;
			RowStart = a_from.RowStart;
			ColStart = a_from.ColStart;
			RowCount = a_from.RowCount;
			ColCount = a_from.ColCount;
			Lat = a_from.Lat;
			Lon = a_from.Lon;
			CellSize = a_from.CellSize;
			NoDataValue = a_from.NoDataValue;
			Properties = a_from.Properties;
			CellData = a_from.CellData;
		}

		//=========================================================================

		public void Serialize()
		{
			try
			{
				using (var stream = iIsoStore.FileStreamXWrite(TileFile))
				{
					var bin = new BinaryFormatter();

					bin.Serialize(stream, this);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error serializing : {0}", ex.Message);
			}
		}

		//=========================================================================
		/// <summary>
		/// Reads an iTile object from a file - may throw an exception
		/// </summary>
		/// <param name="a_filename">the filename</param>
		/// <returns>an iTile object</returns>
		protected static iTile Deserialize(string a_filename)
		{
			using (var stream = iIsoStore.FileStreamXRead(a_filename))
			{
				var bin = new BinaryFormatter();

				return (iTile)bin.Deserialize(stream);
			}
		}

		//=========================================================================
	}
}