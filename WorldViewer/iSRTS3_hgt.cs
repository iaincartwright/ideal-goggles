using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace WorldViewer
{
	internal class iSRTS3_hgt : IRawTile<float>
	{
		public Dictionary<string, string> Properties { get; private set; }
		public int RowCount { get; private set; }
		public int ColCount { get; private set; }

		public double Lat { get; private set; }
		public double Lon { get; private set; }
		public double Width { get; private set; }
		public double Height { get; private set; }
		public double CellSize { get; private set; }

		public float[] CellData { get; private set; }
		public float NoDataValue { get; private set; }

		public iSRTS3_hgt()
		{
			Properties = new Dictionary<string, string>(64);
			
			CellData = new float[0];

			_buffer = new byte[CONST.SRTM3_TILE_EDGE];
		}

		public void Load(int lat_grid, int lon_grid)
		{

		}

		public byte[] _buffer;

		public void Load(string filename)
		{
			try
			{
				using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					//stream.ReadByte();
					//stream.ReadByte();

					GZipStream decomp = new GZipStream(stream, CompressionMode.Decompress, true);

					int byteCount = decomp.Read(_buffer, 0, CONST.SRTM3_TILE_EDGE);

					Console.WriteLine("Read {0} bytes", byteCount);
				}


			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				Console.WriteLine();
			}
		}
	}
}
