using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WorldProcessor
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	struct iShapeHeader
	{
		public Int32 FileCode;// read big-endian, converted to little endian
		private readonly Int32 _unused1;
		private readonly Int32 _unused2;
		private readonly Int32 _unused3;
		private readonly Int32 _unused4;
		private readonly Int32 _unused5;
		public Int32 FileLength;  // read in number of 16 bit words big endian, converted to numberof bytes liytle endian
		public readonly Int32 Version;
		public readonly iShapeType ShapeType;
		public readonly double Xmin;
		public readonly double Ymin;
		public readonly double Xmax;
		public readonly double Ymax;
		public readonly double Zmin;
		public readonly double Zmax;
		public readonly double Mmin;
		public readonly double Mmax;
	}

	public enum iShapeType
	{
		NullShape = 0,
		Point = 1,
		PolyLine = 3,
		Polygon = 5,
		MultiPoint = 8,
		PointZ = 11,
		PolyLineZ = 13,
		PolygonZ = 15,
		MultiPointZ = 18,
		PointM = 21,
		PolyLineM = 23,
		PolygonM = 25,
		MultiPointM = 28,
		MultiPatch = 31,
	}

	public enum iPartType
	{
		TriangleStrip = 0,
		TriangleFan = 1,
		OuterRing = 2,
		InnerRing = 3,
		FirstRing = 4,
		Ring = 5
	}

	class iShapeFile
	{
		public iShapeFile(string a_filename)
		{
			Console.WriteLine(a_filename);
			using (var binReader = new BinaryReader(File.OpenRead(a_filename)))
			{
				var readBytes = binReader.ReadBytes(Marshal.SizeOf(typeof(iShapeHeader)));

				var shapeHeader = BytesToStructure<iShapeHeader>(readBytes);

				// convert to little endian bytes
				shapeHeader.FileCode = EndianSwap(shapeHeader.FileCode);
				shapeHeader.FileLength = EndianSwap(shapeHeader.FileLength) << 1;

				Console.WriteLine("FileCode = {0}", shapeHeader.FileCode);
				Console.WriteLine("FileLength = {0}", shapeHeader.FileLength);
				Console.WriteLine("Version = {0}", shapeHeader.Version);
				Console.WriteLine("ShapeType = {0}", shapeHeader.ShapeType);
				Console.WriteLine("Xmin =	{0}", shapeHeader.Xmin);
				Console.WriteLine("Ymin =	{0}", shapeHeader.Ymin);
				Console.WriteLine("Xmax =	{0}", shapeHeader.Xmax);
				Console.WriteLine("Ymax =	{0}", shapeHeader.Ymax);
				Console.WriteLine("Zmin =	{0}", shapeHeader.Zmin);
				Console.WriteLine("Zmax =	{0}", shapeHeader.Zmax);
				Console.WriteLine("Mmin =	{0}", shapeHeader.Mmin);
				Console.WriteLine("Mmax =	{0}", shapeHeader.Mmax);

				while (binReader.BaseStream.Position < shapeHeader.FileLength)
				{
					int recNum = EndianSwap(binReader.ReadInt32());
					int recLen = EndianSwap(binReader.ReadInt32()) << 1;
					iShapeType recType = (iShapeType)binReader.ReadInt32();

					switch (recType)
					{
						case iShapeType.NullShape:
							break;
						case iShapeType.Point:
							break;
						case iShapeType.PolyLine:
							break;
						case iShapeType.Polygon:
							break;
						case iShapeType.MultiPoint:
							break;
						case iShapeType.PointZ:
							break;
						case iShapeType.PolyLineZ:
							break;
						case iShapeType.PolygonZ:
							break;
						case iShapeType.MultiPointZ:
							break;
						case iShapeType.PointM:
							break;
						case iShapeType.PolyLineM:
							break;
						case iShapeType.PolygonM:
							break;
						case iShapeType.MultiPointM:
							break;
						case iShapeType.MultiPatch:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}

					if (recType != shapeHeader.ShapeType)
						throw new Exception("Different header and content types");

					// Console.WriteLine("RecNum {0} RecLen {1} RecType {2}", recNum, recLen, recType);

					readBytes = binReader.ReadBytes(recLen - 4);
				}

				Console.WriteLine(a_filename);
				//Console.ReadKey(true);
			}
		}

		static int EndianSwap(int a_x)
		{
			return (int)(((uint)a_x >> 24) | (((uint)a_x << 8) & 0x00FF0000) | (((uint)a_x >> 8) & 0x0000FF00) | ((uint)a_x << 24));
		}

		static T BytesToStructure<T>(byte[] a_bytes)
		{
			int size = Marshal.SizeOf(typeof(T));
			var ptr = Marshal.AllocHGlobal(size);
			try
			{
				Marshal.Copy(a_bytes, 0, ptr, size);
				return (T)Marshal.PtrToStructure(ptr, typeof(T));
			}
			finally
			{
				Marshal.FreeHGlobal(ptr);
			}
		}
	}
}
