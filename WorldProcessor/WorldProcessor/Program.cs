using System;
using System.Collections.Generic;

namespace WorldProcessor
{
	static class iProgram
	{
		static List<iTile> s_testFile = new List<iTile>();

		static string s_drive = "C:";

		static void Main(string[] a_args)
		{
			if (a_args.Length > 0)
				s_drive = a_args[0];

			var inFile = s_drive + @"\Users\iain\Data\mean2.asc";

			var shpList = new List<iShapeFile>
			{
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_AdministrativeBoundary.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_Airport.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_Building.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_ElectricityTransmissionLine.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_Foreshore.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_HeritageSite.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_Land.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_NamedPlace.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_Ornament.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_PublicAmenity.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_RailwayStation.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_RailwayTrack.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_RailwayTunnel.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_Road.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_SpotHeight.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_SurfaceWater_Area.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_SurfaceWater_Line.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_TidalBoundary.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_TidalWater.shp"),
					new iShapeFile(s_drive + @"\Users\iain\Data\OS VectorMap District (Vector) SY\data\SY\SY_Woodland.shp"),
			};

			var tiffList = new List<iGeoTiff>
			{
					new iGeoTiff(s_drive + @"\Users\iain\Data\OS VectorMap District (Raster) SY\data\SY\sy67.tif"),
          //new iGeoTiff(s_drive + @"\Users\iain\Data\SRTM_SE_250m_TIF\srtm_se_250m.tif"),
          //new iGeoTiff(s_drive + @"\Users\iain\Data\p093r086_7p20001005_z55_nn80.tif"),
          //new iGeoTiff(s_drive + @"\Users\iain\Data\p093r086_7t20001005_z55_nn30.tif")
      };

			foreach (var geoTiff in tiffList)
			{
				Console.WriteLine(geoTiff.ToString());
			}

			//const int percent_perslice = 10;
			//for (int i = 0; i < 100; i += percent_perslice)
			//{
			//    s_testFile.Add(iTileMaker.LoadOrCreate(inFile, i, percent_perslice, 0, 100));
			//}


			if (true)
			{
				for (int x = 0; x < 100; x += 10)
				{
					for (int y = 0; y < 100; y += 10)
					{
						iTileMaker.LoadOrCreate(inFile, x, 10, y, 10);
					}
				}
			}


			s_testFile.Add(iTileMaker.LoadOrCreate(inFile, 0, 10, 10, 10));

			var coll = new iTileCollection("Mean2");

			Console.Write("Loading collection {0}...", coll.Name);
			coll.LoadCollection();
			Console.WriteLine("{0} items, Done!", coll.Count);

			Console.WriteLine("\nPress any key to exit...");
			Console.ReadKey(true);
		}
	}
}
