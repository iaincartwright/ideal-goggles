using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

using OSGeo.GDAL;


namespace iADFReader
{
	class GDALReadDirect
	{
		public static void usage()
		{
			Console.WriteLine("usage: GDALDatasetRasterIO {GDAL dataset name} {output file name}");
			System.Environment.Exit(-1);
		}

		public static void Main(string[] args)
		{
			if (args.Length < 2) usage();

			// Using early initialization of System.Console
			Console.WriteLine("");

			try
			{
				/* -------------------------------------------------------------------- */
				/*      Register driver(s).                                             */
				/* -------------------------------------------------------------------- */
				Gdal.AllRegister();

				/* -------------------------------------------------------------------- */
				/*      Open dataset.                                                   */
				/* -------------------------------------------------------------------- */
				Dataset ds = Gdal.Open(args[0], Access.GA_ReadOnly);

				if (ds == null)
				{
					Console.WriteLine("Can't open " + args[0]);
					System.Environment.Exit(-1);
				}

				Console.WriteLine("------------------------------------------------------");
				Console.WriteLine("Raster dataset parameters:");
				Console.WriteLine("  Description: " + ds.GetDescription());
				Console.WriteLine("  RasterCount: " + ds.RasterCount);
				Console.WriteLine("  RasterSizeX: " + ds.RasterXSize);
				Console.WriteLine("  RasterSizeY: " + ds.RasterYSize);
				Console.WriteLine("------------------------------------------------------");
				Console.WriteLine("  Projection:  " );
				string[] elements = ds.GetProjectionRef().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string elem in elements)
					Console.WriteLine("    " + elem);
				Console.WriteLine("------------------------------------------------------");

				if (ds.GetGCPCount() > 0)
				{
					Console.WriteLine("------------------------------------------------------");
					Console.WriteLine("GCP List");
					Console.WriteLine("------------------------------------------------------");
					GCP[] listGCP = ds.GetGCPs();
					for (int i = 0; i < ds.GetGCPCount(); i++)
					{
						Console.WriteLine("  +++++++++++++++++++++++++++++");
						Console.WriteLine("  Id:   {0}", listGCP[i].Id);
						Console.WriteLine("  Info: {0}", listGCP[i].Info);
					}
					Console.WriteLine("------------------------------------------------------");
				}

				string[] files = ds.GetFileList();

				foreach (var file in files)
				{
					Console.WriteLine("==> {0}", file);
				}

				/* -------------------------------------------------------------------- */
				/*      Get driver                                                      */
				/* -------------------------------------------------------------------- */
				Driver drv = ds.GetDriver();

				if (drv == null)
				{
					Console.WriteLine("Can't get driver.");
					System.Environment.Exit(-1);
				}

				Console.WriteLine("Using driver " + drv.LongName);

				/* -------------------------------------------------------------------- */
				/*      Processing the raster                                           */
				/* -------------------------------------------------------------------- */
				// SaveBitmapDirect(args[1], ds, 0, 0, ds.RasterXSize, ds.RasterYSize, ds.RasterXSize, ds.RasterYSize);
			}
			catch (Exception e)
			{
				Console.WriteLine("Application error: " + e.Message);
			}
			finally
			{
				Console.ReadKey(true);
			}
		}

		private static void SaveBitmapDirect(string filename, Dataset ds, int xOff, int yOff, int width, int height, int imageWidth, int imageHeight)
		{
			if (ds.RasterCount == 0)
				return;

			int[] bandMap = new int[4] { 1, 1, 1, 1 };
			int channelCount = 1;
			bool hasAlpha = false;
			bool isIndexed = false;
			int channelSize = 8;
			ColorTable ct = null;
			// Evaluate the bands and find out a proper image transfer format
			for (int i = 0; i < ds.RasterCount; i++)
			{
				Band band = ds.GetRasterBand(i + 1);
				
				if (Gdal.GetDataTypeSize(band.DataType) > 8)
					channelSize = 16;
				
				switch (band.GetRasterColorInterpretation())
				{
					case ColorInterp.GCI_AlphaBand:
						channelCount = 4;
						hasAlpha = true;
						bandMap[3] = i + 1;
						break;
					case ColorInterp.GCI_BlueBand:
						if (channelCount < 3)
							channelCount = 3;
						bandMap[0] = i + 1;
						break;
					case ColorInterp.GCI_RedBand:
						if (channelCount < 3)
							channelCount = 3;
						bandMap[2] = i + 1;
						break;
					case ColorInterp.GCI_GreenBand:
						if (channelCount < 3)
							channelCount = 3;
						bandMap[1] = i + 1;
						break;
					case ColorInterp.GCI_PaletteIndex:
						ct = band.GetRasterColorTable();
						isIndexed = true;
						bandMap[0] = i + 1;
						break;
					case ColorInterp.GCI_GrayIndex:
						isIndexed = true;
						bandMap[0] = i + 1;
						break;
					default:
						// we create the bandmap using the dataset ordering by default
						if (i < 4 && bandMap[i] == 0)
						{
							if (channelCount < i)
								channelCount = i;
							bandMap[i] = i + 1;
						}
						break;
				}
			}

			// find out the pixel format based on the gathered information
			PixelFormat pixelFormat;
			DataType dataType;
			int pixelSpace;

			if (isIndexed)
			{
				pixelFormat = PixelFormat.Format8bppIndexed;
				dataType = DataType.GDT_Byte;
				pixelSpace = 1;
			}
			else
			{
				if (channelCount == 1)
				{
					if (channelSize > 8)
					{
						pixelFormat = PixelFormat.Format16bppGrayScale;
						dataType = DataType.GDT_Int16;
						pixelSpace = 2;
					}
					else
					{
						pixelFormat = PixelFormat.Format24bppRgb;
						channelCount = 3;
						dataType = DataType.GDT_Byte;
						pixelSpace = 3;
					}
				}
				else
				{
					if (hasAlpha)
					{
						if (channelSize > 8)
						{
							pixelFormat = PixelFormat.Format64bppArgb;
							dataType = DataType.GDT_UInt16;
							pixelSpace = 8;
						}
						else
						{
							pixelFormat = PixelFormat.Format32bppArgb;
							dataType = DataType.GDT_Byte;
							pixelSpace = 4;
						}
						channelCount = 4;
					}
					else
					{
						if (channelSize > 8)
						{
							pixelFormat = PixelFormat.Format48bppRgb;
							dataType = DataType.GDT_UInt16;
							pixelSpace = 6;
						}
						else
						{
							pixelFormat = PixelFormat.Format24bppRgb;
							dataType = DataType.GDT_Byte;
							pixelSpace = 3;
						}
						channelCount = 3;
					}
				}
			}

			// Create a Bitmap to store the GDAL image in
			//int edgeKb = 32;
			int edgeKb = 17;
			//int edgeKb = 4;
			Bitmap bitmapTest = new Bitmap(edgeKb * 1024, edgeKb * 1024, pixelFormat);

			Bitmap bitmap = new Bitmap(imageWidth, imageHeight, pixelFormat);

			if (isIndexed)
			{
				// setting up the color table
				if (ct != null)
				{
					int iCol = ct.GetCount();
					ColorPalette pal = bitmap.Palette;
					for (int i = 0; i < iCol; i++)
					{
						ColorEntry ce = ct.GetColorEntry(i);
						pal.Entries[i] = Color.FromArgb(ce.c4, ce.c1, ce.c2, ce.c3);
					}
					bitmap.Palette = pal;
				}
				else
				{
					// grayscale
					ColorPalette pal = bitmap.Palette;
					for (int i = 0; i < 256; i++)
						pal.Entries[i] = Color.FromArgb(255, i, i, i);
					bitmap.Palette = pal;
				}
			}

			// Use GDAL raster reading methods to read the image data directly into the Bitmap
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadWrite, pixelFormat);

			try
			{
				int stride = bitmapData.Stride;
				IntPtr buf = bitmapData.Scan0;

				ds.ReadRaster(xOff, yOff, width, height, buf, imageWidth, imageHeight, dataType,
						channelCount, bandMap, pixelSpace, stride, 1);
			}
			finally
			{
				bitmap.UnlockBits(bitmapData);
			}

			bitmap.Save(filename);
		}
	}
}
