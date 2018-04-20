using System;
namespace WorldViewer
{
	class CONST
	{ 
		public const double ARC_SEC3 = 0.00083333333333333;
		public const int SRTM3_TILE_EDGE = 2884802;
	}
	

	interface IRawTile<T>
	{
		T[] CellData { get; }
		double CellSize { get; }
		int ColCount { get; }
		double Height { get; }
		double Lat { get; }
		void Load(string filename);
		void Load(int lat_grid, int lon_grid);
		double Lon { get; }
		T NoDataValue { get; }
		System.Collections.Generic.Dictionary<string, string> Properties { get; }
		int RowCount { get; }
		double Width { get; }
	}
}
