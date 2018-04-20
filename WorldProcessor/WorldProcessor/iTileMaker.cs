using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using iUtils;
using iStringTokeniser;

namespace WorldProcessor
{
	[Serializable]
	class iTileMaker : iTile
	{
		//=========================================================================

		private iTileMaker()
		{
		}

		//=========================================================================

		private iTileMaker(iTile a_from)
			: base(a_from)
		{
		}

		//=========================================================================
		public static iTile Create(string a_filename, int a_stripStartPercent, int a_stripWidthPercent, int a_bandStartPercent, int a_bandWidthPercent)
		{
			// create a tilemaker and load the data
			var tileMaker = new iTileMaker();

			using (var reader = tileMaker.ReadSourceHeader(a_filename))
			{
				tileMaker.MakeSubTile(a_stripStartPercent, a_stripWidthPercent, a_bandStartPercent, a_bandWidthPercent);

				tileMaker.LoadSourceData(reader);
			}

			// create a fresh tile from the maker base and serialize it 
			var createdTile = new iTile(tileMaker);

			createdTile.Serialize();

			return createdTile;
		}

		//=========================================================================
		public static iTile LoadOrCreate(string a_filename, int a_stripStartPercent, int a_stripWidthPercent, int a_bandStartPercent, int a_bandWidthPercent)
		{
			// read the source file header into a temp tile
			var elevTile = LoadSourceHeader(a_filename);

			if (elevTile != null)
			{
				// we found the source file, so set params for the subtile we are interested in
				elevTile.MakeSubTile(a_stripStartPercent, a_stripWidthPercent, a_bandStartPercent, a_bandWidthPercent);

				Console.WriteLine("Loading {0}...", elevTile.TileFile);

				// now load the subtile, if it exists
				elevTile = Deserialize(elevTile.TileFile);

				if (elevTile == null)
				{
					// desrialisation failed so start from scratch
					elevTile = new iTileMaker();

					// read the header
					using (var reader = elevTile.ReadSourceHeader(a_filename))
					{
						// remake the subtile params again
						elevTile.MakeSubTile(a_stripStartPercent, a_stripWidthPercent, a_bandStartPercent, a_bandWidthPercent);

						Console.WriteLine("Creating {0}...", elevTile.TileFile);

						// read the data from the source
						elevTile.LoadSourceData(reader);
					}

					// write the data out
					elevTile.Serialize();
				}

				Console.WriteLine("Done!");

				elevTile.TraceTile();
			}

			return elevTile;
		}

		//=========================================================================
		/// <summary>
		/// Loads the header from an ascii source file into a temporary tile
		/// </summary>
		/// <param name="a_filename">the name of the source file (.asc)</param>
		/// <returns>a tile with only the header filled out</returns>
		private static iTileMaker LoadSourceHeader(string a_filename)
		{
			iTileMaker returnTile = null;

			var elevTile = new iTileMaker();

			try
			{
				using (var streamReader = elevTile.ReadSourceHeader(a_filename))
				{
					if (streamReader != null)
						returnTile = elevTile;
				}
			}
			catch (Exception)
			{
			}

			return returnTile;
		}

		//=========================================================================
		/// <summary>
		/// Does the actual work of loading the header and initialising most of the
		/// fields. Call MakeSubTile(...) before attempting to load any data into 
		/// the tile.
		/// </summary>
		/// <param name="a_filename">the name of the source file (.asc)</param>
		/// <returns>a StreamReader object pointing to the dtart of the data</returns>
		private StreamReader ReadSourceHeader(string a_filename)
		{
			Properties = new Dictionary<string, string>(64);

			var reader = new StreamReader(a_filename, Encoding.ASCII, false, ColCountSrc * 6 + 64);

			for (int i = 0; i < 6; i++)
			{
				var readLine = reader.ReadLine();

				if (readLine != null)
				{
					var tokens = readLine.Split(null as string[], StringSplitOptions.RemoveEmptyEntries);

					if (tokens.Length == 2)
						Properties[tokens[0].ToLowerInvariant()] = tokens[1].ToLowerInvariant();
					else
						throw new InvalidDataException($"File {a_filename} has bad header at line {i}");
				}
			}

			SourceFile = a_filename;

			// these refer to the source file only
			RowCountSrc = int.Parse(Properties["nrows"]);
			ColCountSrc = int.Parse(Properties["ncols"]);
			LatSrc = double.Parse(Properties["yllcorner"]);
			LonSrc = double.Parse(Properties["xllcorner"]);

			// these are common to source and sub tile
			CellSize = double.Parse(Properties["cellsize"]);
			NoDataValue = float.Parse(Properties["nodata_value"]);

			// these are changed when read as a subtile
			// initialise to tile values
			RowStart = ColStart = 0;
			RowCount = RowCountSrc;
			ColCount = ColCountSrc;
			Lat = LatSrc;
			Lon = LonSrc;

			return reader;
		}

		//=========================================================================
		/// <summary>
		/// Set up the subtile - this must be called, even if the entire .asc file
		/// is held in one tile as it allocates the list for the rows and makes
		/// correct file name
		/// </summary>
		/// <param name="a_stripStartPercent">the start column percentage</param>
		/// <param name="a_stripWidthPercent">the subtile width percentage</param>
		/// <param name="a_bandStartPercent">the start row percentage</param>
		/// <param name="a_bandWidthPercent">the subtile width percentage</param>
		private void MakeSubTile(int a_stripStartPercent, int a_stripWidthPercent, int a_bandStartPercent, int a_bandWidthPercent)
		{
			ColStart = (ColCountSrc * a_stripStartPercent) / 100;
			ColCount = (ColCountSrc * a_stripWidthPercent) / 100;

			RowStart = (RowCountSrc * a_bandStartPercent) / 100;
			RowCount = (RowCountSrc * a_bandWidthPercent) / 100;

			Lon = LonSrc + (CellSize * ColStart);
			Lat = LatSrc + (CellSize * RowStart);

			MakeTileFileName();
		}

		//=========================================================================

		private void MakeTileFileName()
		{
			var baseName = Path.GetFileNameWithoutExtension(SourceFile);

			if (baseName != null)
			{
				var tileFolder = baseName.ToLowerInvariant();

				if (iIsoStore.DirectoryExists(tileFolder) == false)
				{
					iIsoStore.CreateDirectory(tileFolder);
				}

				TileFile = $"{tileFolder}\\{ColStart:D6}-{ColCount:D6}-{RowStart:D6}-{RowCount:D6}.bin";
			}
		}

		//=========================================================================

		private void LoadSourceData(TextReader a_stream, bool a_compress = false)
		{
			var sw = new iStopwatch(TileFile);

			CellData = new List<short[]>(RowCount);

			var thisLine = new short[ColCount];
			const short[] emptyLine = null;

			// in theory a compressed line could be a maximum of 1.5 times the length 
			// of an uncompressed line
			var thisLineCompressed = new List<short>(ColCount * 2);

			int rowNum = 0;
			// skip the leading rows
			while (rowNum < RowStart)
			{
				a_stream.ReadLine();
				rowNum++;
			}

			// now read the rows we are interested in
			rowNum = 0;
			while (rowNum < RowCount)
			{
				sw.PulseStart();

				var lineIn = a_stream.ReadLine();

				// this is a read error
				if (lineIn == null)
				{
					throw new InvalidDataException($"Unexpeted EOF in data at line {rowNum}");
				}

				// returns ColCount for a full row of data or 1 for a row of zeros - any other
				// return value is an error
				int dataCount = Splitter.DoSplit(lineIn.ToCharArray(), thisLine, ColStart, ColCount);

				if (dataCount == ColCount)
				{ // a full row was detected
					if (a_compress)
					{
						// remove the last compressed data line
						thisLineCompressed.Clear();

						// do the actual compression
						CompressLine(thisLine, thisLineCompressed);

						// if we actually have compressed, then use that
						if (thisLineCompressed.Count < ColCount)
						{
							CellData.Add(thisLineCompressed.ToArray());
						}
						else
						{ // otherwise use the original uncompressed line
							CellData.Add(thisLine);
							thisLine = new short[ColCount];
						}
					}
					else
					{
						CellData.Add(thisLine);
						thisLine = new short[ColCount];
					}
				}
				else if (dataCount == 1)
				{ // a return value of 1 means all zeroes were detected in the row
					CellData.Add(emptyLine);
				}
				else
				{ // a bad row was detected
					Console.WriteLine("Error : unexpected return value from Splitter.DoSplit(): {0}", dataCount);
					throw new InvalidDataException($"File {TileFile} has bad data at line {rowNum}");
				}

				sw.PulseStop();

				rowNum++;

				if ((rowNum % 100) == 0)
					Console.WriteLine("Processed line {0} of {1}", rowNum, RowCount);
			}

			sw.Report();
		}

		//=========================================================================

		private void CompressLine(short[] a_thisLine, List<short> a_thisLineCompressed)
		{
			for (int col = 0; col < ColCount;)
			{
				a_thisLineCompressed.Add(a_thisLine[col]);

				// compress 0's - even if there is only one of them
				if (a_thisLine[col] == 0)
				{
					short compressCount = 0;

					while ((col < ColCount) && (a_thisLine[col] == 0))
					{
						compressCount++;
						col++;
					}

					a_thisLineCompressed.Add(compressCount);
				}
				else
				{
					col++;
				}
			}
		}

		//=========================================================================

		private void TraceTile()
		{
			Console.WriteLine("Source file name	{0}", SourceFile);
			Console.WriteLine("Source Lat/Lon		{0}/{1}", LatSrc, LonSrc);
			Console.WriteLine("Source Height/Width	{0}/{1}", HeightSrc, WidthSrc);
			Console.WriteLine("Source Rows/Cols	{0}/{1}", RowCountSrc, ColCountSrc);
			Console.WriteLine("Source Cell Size	{0}", CellSize);
			Console.WriteLine("Tile file name	{0}", TileFile);
			Console.WriteLine("Tile Lat/Lon		{0}/{1}", Lat, Lon);
			Console.WriteLine("Tile StartRow/StartCol		{0}/{1}", RowStart, ColStart);
			Console.WriteLine("Tile Rows/Cols		{0}/{1}", RowCount, ColCount);
			Console.WriteLine("Tile Height/Width	{0}/{1}", Height, Width);
			Console.WriteLine("Data Rows		{0}", CellData?.Count ?? 0);
		}

		//=========================================================================

		private new static iTileMaker Deserialize(string a_filename)
		{
			try
			{
				var tempTile = iTile.Deserialize(a_filename);

				return new iTileMaker(tempTile);
			}
			catch
			{ // we expect this to fail when we are making tiles so return null
				return null;
			}
		}

		//=========================================================================
	}
}
