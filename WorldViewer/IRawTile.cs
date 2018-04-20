using System;
namespace WorldViewer
{
	class Const
	{ 
		public const double ArcSec3 = 0.00083333333333333;
		public const int Srtm3TileEdge = 2884802;
	}
	

	interface IRawTile<T>
	{
		T[] CellData { get; }
		double CellSize { get; }
		int ColCount { get; }
		double Height { get; }
		double Lat { get; }
		void Load(string a_filename);
		void Load(int a_latGrid, int a_lonGrid);
		double Lon { get; }
		T NoDataValue { get; }
		System.Collections.Generic.Dictionary<string, string> Properties { get; }
		int RowCount { get; }
		double Width { get; }
	}
}
