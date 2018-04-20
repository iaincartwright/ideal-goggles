using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WorldViewer
{
	internal class DemAscii : IRawTile<float>
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

		public DemAscii()
		{
			Properties = new Dictionary<string, string>(64);
			
			CellData = new float[0];
		}

		// lat_grid and long_grid reefer to lower left
		// coordinate lon 0 -> 179 lat = -90 -> +90
		public void Load(int a_latGrid, int a_lonGrid)
		{
		}

		public void Load(string a_filename)
		{
			List<string> dataLines  = new List<string>();

			try
			{
				dataLines.AddRange(File.ReadAllLines(a_filename));

				int index = 0;
				bool readHeader = true;
				while (readHeader && index < dataLines.Count)
				{
					string[] tokens = dataLines[index].Split(null as string[], StringSplitOptions.RemoveEmptyEntries);

					switch (tokens.Length)
					{
						case 2:
							Properties[tokens[0].ToLowerInvariant()] = tokens[1].ToLowerInvariant();
							break;

						// ignore
						case 0:
						case 1:
							break;

						default:
							// this must be data
							readHeader = false;
							break;
					}
					index++;
				}

				this.RowCount = int.Parse(Properties["ncols"]);
				this.ColCount = int.Parse(Properties["nrows"]);
				this.CellSize = double.Parse(Properties["cellsize"]);
				this.Lat = double.Parse(Properties["yllcorner"]);
				this.Lon = double.Parse(Properties["xllcorner"]);
				this.NoDataValue = float.Parse(Properties["nodata_value"]);
				this.CellData = new float[RowCount * ColCount];
				this.Width = CellSize * (double)ColCount;
				this.Height = CellSize * (double)RowCount;

				int dataIndex = 0;
				bool readData = true;
				while (readData && index < dataLines.Count)
				{
					string[] tokens = dataLines[index].Split(null as string[], StringSplitOptions.RemoveEmptyEntries);

					if (tokens.Length == ColCount)
					{
						foreach (string token in tokens)
						{
							CellData[dataIndex++] = float.Parse(token);
						}
					}
					index++;
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
