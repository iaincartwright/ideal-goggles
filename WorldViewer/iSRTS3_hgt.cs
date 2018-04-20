using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace WorldViewer
{
	internal class Srts3Hgt : IRawTile<float>
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

		public Srts3Hgt()
		{
			Properties = new Dictionary<string, string>(64);
			
			CellData = new float[0];

			Buffer = new byte[Const.Srtm3TileEdge];
		}

		public void Load(int a_latGrid, int a_lonGrid)
		{

		}

		public byte[] Buffer;

		public void Load(string a_filename)
		{
			try
			{
				using (FileStream stream = new FileStream(a_filename, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					//stream.ReadByte();
					//stream.ReadByte();

					GZipStream decomp = new GZipStream(stream, CompressionMode.Decompress, true);

					int byteCount = decomp.Read(Buffer, 0, Const.Srtm3TileEdge);

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
