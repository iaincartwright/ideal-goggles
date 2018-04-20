using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace WorldProcessor
{
	class iRational : IFormattable
	{
		public double Value { get; private set; }

		public iRational(uint a_numerator, uint a_denominator)
		{
			Value = a_numerator / (double)a_denominator;
		}

		public iRational(int a_numerator, int a_denominator)
		{
			Value = a_numerator / (double)a_denominator;
		}

		public string ToString(string a_format, IFormatProvider a_formatProvider)
		{
			return Value.ToString(a_format, a_formatProvider);
		}
	}

	class iGeoTiff
	{
		static Dictionary<int, string> s_tagDict;
		static readonly Type[] s_TIFFTypes = new Type[13];

		//=============================================================================

		static iGeoTiff()
		{
			InitTags();
		}

		//=============================================================================

		public iGeoTiff(string a_filename)
		{
			if (File.Exists(a_filename))
			{
				using (var geoTIFF = new Bitmap(a_filename))
				{
					Console.WriteLine("//=============================================================================");
					Console.WriteLine("Loaded {0}  {1} x {2}", a_filename, geoTIFF.Width, geoTIFF.Height);
					Console.WriteLine("//=============================================================================");
					foreach (var item in geoTIFF.PropertyItems)
					{
						Console.WriteLine("{0} ({1} of {2}) - {3}", item.Id, item.Len, s_TIFFTypes[item.Type], s_tagDict[item.Id].Split(new[] {' '}, 2)[0]);
					}
				}
			}
			else
				throw new FileNotFoundException();
		}

		//=============================================================================

		static private void InitTags()
		{
			s_TIFFTypes[0] = null;
			s_TIFFTypes[1] = typeof(byte);
			s_TIFFTypes[2] = typeof(string);
			s_TIFFTypes[3] = typeof(ushort);
			s_TIFFTypes[4] = typeof(uint);
			s_TIFFTypes[5] = typeof(iRational);
			s_TIFFTypes[6] = typeof(sbyte);
			s_TIFFTypes[7] = typeof(byte);
			s_TIFFTypes[8] = typeof(short);
			s_TIFFTypes[9] = typeof(int);
			s_TIFFTypes[10] = typeof(iRational);
			s_TIFFTypes[11] = typeof(float);
			s_TIFFTypes[12] = typeof(double);
			// Types
			//1 = BYTE 8-bit unsigned integer. 
			//2 = ASCII 8-bit byte that contains a 7-bit ASCII code; the last byte must be NUL (binary zero). 
			//3 = SHORT 16-bit (2-byte) unsigned integer. 
			//4 = LONG 32-bit (4-byte) unsigned integer. 
			//5 = RATIONAL Two LONGs: the first represents the numerator of a fraction; the second, the denominator 
			//6 = SBYTE An 8-bit signed (twos-complement) integer. 
			//7 = UNDEFINED An 8-bit byte that may contain anything, depending on the definition of the field. 
			//8 = SSHORT A 16-bit (2-byte) signed (twos-complement) integer. 
			//9 = SLONG A 32-bit (4-byte) signed (twos-complement) integer. 
			//10 = SRATIONAL Two SLONG’s: the first represents the numerator of a fraction, the second the denominator. 
			//11 = FLOAT Single precision (4-byte) IEEE format. 
			//12 = DOUBLE Double precision (8-byte) IEEE format. 

			s_tagDict = new Dictionary<int, string>
			{ 
				//=============================================================================
				// REFERENCES
				// http://www.remotesensing.org/geotiff/spec/geotiff6.html#6.1
				// http://www.awaresystems.be/imaging/tiff/tifftags.html
				// http://trac.osgeo.org/geotiff/
				//=============================================================================
				// Baseline tags
				//=============================================================================
				{ 254 , "NewSubfileType A general indication of the kind of data contained in this subfile. "},
				{ 255 , "SubfileType A general indication of the kind of data contained in this subfile. "},
				{ 256 , "ImageWidth The number of columns in the image, i.e., the number of pixels per row. "},
				{ 257 , "ImageLength The number of rows of pixels in the image. "},
				{ 258 , "BitsPerSample Number of bits per component. "},
				{ 259 , "Compression Compression scheme used on the image data. "},
				{ 262 , "PhotometricInterpretation The color space of the image data. "},
				{ 263 , "Threshholding For black and white TIFF files that represent shades of gray, the technique used to convert from gray to black and white pixels. "},
				{ 264 , "CellWidth The width of the dithering or halftoning matrix used to create a dithered or halftoned bilevel file. "},
				{ 265 , "CellLength The length of the dithering or halftoning matrix used to create a dithered or halftoned bilevel file. "},
				{ 266 , "FillOrder The logical order of bits within a byte. "},
				{ 270 , "ImageDescription A string that describes the subject of the image. "},
				{ 271 , "Make The scanner manufacturer. "},
				{ 272 , "Model The scanner model name or number. "},
				{ 273 , "StripOffsets For each strip, the byte offset of that strip. "},
				{ 274 , "Orientation The orientation of the image with respect to the rows and columns. "},
				{ 277 , "SamplesPerPixel The number of components per pixel. "},
				{ 278 , "RowsPerStrip The number of rows per strip. "},
				{ 279 , "StripByteCounts For each strip, the number of bytes in the strip after compression. "},
				{ 280 , "MinSampleValue The minimum component value used. "},
				{ 281 , "MaxSampleValue The maximum component value used. "},
				{ 282 , "XResolution The number of pixels per ResolutionUnit in the ImageWidth direction. "},
				{ 283 , "YResolution The number of pixels per ResolutionUnit in the ImageLength direction. "},
				{ 284 , "PlanarConfiguration How the components of each pixel are stored. "},
				{ 288 , "FreeOffsets For each string of contiguous unused bytes in a TIFF file, the byte offset of the string. "},
				{ 289 , "FreeByteCounts For each string of contiguous unused bytes in a TIFF file, the number of bytes in the string. "},
				{ 290 , "GrayResponseUnit The precision of the information contained in the GrayResponseCurve. "},
				{ 291 , "GrayResponseCurve For grayscale data, the optical density of each possible pixel value. "},
				{ 296 , "ResolutionUnit The unit of measurement for XResolution and YResolution. "},
				{ 305 , "Software Name and version number of the software package(s) used to create the image. "},
				{ 306 , "DateTime Date and time of image creation. "},
				{ 315 , "Artist Person who created the image. "},
				{ 316 , "HostComputer The computer and/or operating system in use at the time of image creation. "},
				{ 320 , "ColorMap A color map for palette color images. "},
				{ 338 , "ExtraSamples Description of extra components. "},
				{ 33432 , "Copyright Copyright notice. "},
				//=============================================================================
				// Extension tags
				//=============================================================================
				{ 269 , "DocumentName The name of the document from which this image was scanned. "},
				{ 285 , "PageName The name of the page from which this image was scanned. "},
				{ 286 , "XPosition X position of the image. "},
				{ 287 , "YPosition Y position of the image. "},
				{ 292 , "T4Options Options for Group 3 Fax compression "},
				{ 293 , "T6Options Options for Group 4 Fax compression "},
				{ 297 , "PageNumber The page number of the page from which this image was scanned. "},
				{ 301 , "TransferFunction Describes a transfer function for the image in tabular style. "},
				{ 317 , "Predictor A mathematical operator that is applied to the image data before an encoding scheme is applied. "},
				{ 318 , "WhitePoint The chromaticity of the white point of the image. "},
				{ 319 , "PrimaryChromaticities The chromaticities of the primaries of the image. "},
				{ 321 , "HalftoneHints Conveys to the halftone function the range of gray levels within a colorimetrically-specified image that should retain tonal detail. "},
				{ 322 , "TileWidth The tile width in pixels. This is the number of columns in each tile. "},
				{ 323 , "TileLength The tile length (height) in pixels. This is the number of rows in each tile. "},
				{ 324 , "TileOffsets For each tile, the byte offset of that tile, as compressed and stored on disk. "},
				{ 325 , "TileByteCounts For each tile, the number of (compressed) bytes in that tile. "},
				{ 326 , "BadFaxLines Used in the TIFF-F standard, denotes the number of 'bad' scan lines encountered by the facsimile device. "},
				{ 327 , "CleanFaxData Used in the TIFF-F standard, indicates if 'bad' lines encountered during reception are stored in the data, or if 'bad' lines have been replaced by the receiver. "},
				{ 328 , "ConsecutiveBadFaxLines Used in the TIFF-F standard, denotes the maximum number of consecutive 'bad' scanlines received. "},
				{ 330 , "SubIFDs Offset to child IFDs. "},
				{ 332 , "InkSet The set of inks used in a separated (PhotometricInterpretation=5) image. "},
				{ 333 , "InkNames The name of each ink used in a separated image. "},
				{ 334 , "NumberOfInks The number of inks. "},
				{ 336 , "DotRange The component values that correspond to a 0% dot and 100% dot. "},
				{ 337 , "TargetPrinter A description of the printing environment for which this separation is intended. "},
				{ 339 , "SampleFormat Specifies how to interpret each data sample in a pixel. "},
				{ 340 , "SMinSampleValue Specifies the minimum sample value. "},
				{ 341 , "SMaxSampleValue Specifies the maximum sample value. "},
				{ 342 , "TransferRange Expands the range of the TransferFunction. "},
				{ 343 , "ClipPath Mirrors the essentials of PostScript's path creation functionality. "},
				{ 344 , "XClipPathUnits The number of units that span the width of the image, in terms of integer ClipPath coordinates. "},
				{ 345 , "YClipPathUnits The number of units that span the height of the image, in terms of integer ClipPath coordinates. "},
				{ 346 , "Indexed Aims to broaden the support for indexed images to include support for any color space. "},
				{ 347 , "JPEGTables JPEG quantization and/or Huffman tables. "},
				{ 351 , "OPIProxy OPI-related. "},
				{ 400 , "GlobalParametersIFD Used in the TIFF-FX standard to point to an IFD containing tags that are globally applicable to the complete TIFF file. "},
				{ 401 , "ProfileType Used in the TIFF-FX standard, denotes the type of data stored in this file or IFD. "},
				{ 402 , "FaxProfile Used in the TIFF-FX standard, denotes the 'profile' that applies to this file. "},
				{ 403 , "CodingMethods Used in the TIFF-FX standard, indicates which coding methods are used in the file. "},
				{ 404 , "VersionYear Used in the TIFF-FX standard, denotes the year of the standard specified by the FaxProfile field. "},
				{ 405 , "ModeNumber Used in the TIFF-FX standard, denotes the mode of the standard specified by the FaxProfile field. "},
				{ 433 , "Decode Used in the TIFF-F and TIFF-FX standards, holds information about the ITULAB (PhotometricInterpretation = 10) encoding. "},
				{ 434 , "DefaultImageColor Defined in the Mixed Raster Content part of RFC 2301, is the default color needed in areas where no image is available. "},
				{ 512 , "JPEGProc Old-style JPEG compression field. TechNote2 invalidates this part of the specification. "},
				{ 513 , "JPEGInterchangeFormat Old-style JPEG compression field. TechNote2 invalidates this part of the specification. "},
				{ 514 , "JPEGInterchangeFormatLength Old-style JPEG compression field. TechNote2 invalidates this part of the specification. "},
				{ 515 , "JPEGRestartInterval Old-style JPEG compression field. TechNote2 invalidates this part of the specification. "},
				{ 517 , "JPEGLosslessPredictors Old-style JPEG compression field. TechNote2 invalidates this part of the specification. "},
				{ 518 , "JPEGPointTransforms Old-style JPEG compression field. TechNote2 invalidates this part of the specification. "},
				{ 519 , "JPEGQTables Old-style JPEG compression field. TechNote2 invalidates this part of the specification. "},
				{ 520 , "JPEGDCTables Old-style JPEG compression field. TechNote2 invalidates this part of the specification. "},
				{ 521 , "JPEGACTables Old-style JPEG compression field. TechNote2 invalidates this part of the specification. "},
				{ 529 , "YCbCrCoefficients The transformation from RGB to YCbCr image data. "},
				{ 530 , "YCbCrSubSampling Specifies the subsampling factors used for the chrominance components of a YCbCr image. "},
				{ 531 , "YCbCrPositioning Specifies the positioning of subsampled chrominance components relative to luminance samples. "},
				{ 532 , "ReferenceBlackWhite Specifies a pair of headroom and footroom image data values (codes) for each pixel component. "},
				{ 559 , "StripRowCounts Defined in the Mixed Raster Content part of RFC 2301, used to replace RowsPerStrip for IFDs with variable-sized strips. "},
				{ 700 , "XMP XML packet containing XMP metadata "},
				{ 32781 , "ImageID OPI-related. "},
				{ 34732 , "ImageLayer Defined in the Mixed Raster Content part of RFC 2301, used to denote the particular function of this Image in the mixed raster scheme."},
				//=============================================================================
				// private tags
				//=============================================================================
				{ 32932 , "Wang Annotation Annotation data, as used in 'Imaging for Windows'. "},
				{ 33445 , "MD FileTag Specifies the pixel data format encoding in the Molecular Dynamics GEL file format. "},
				{ 33446 , "MD ScalePixel Specifies a scale factor in the Molecular Dynamics GEL file format. "},
				{ 33447 , "MD ColorTable Used to specify the conversion from 16bit to 8bit in the Molecular Dynamics GEL file format. "},
				{ 33448 , "MD LabName Name of the lab that scanned this file, as used in the Molecular Dynamics GEL file format. "},
				{ 33449 , "MD SampleInfo Information about the sample, as used in the Molecular Dynamics GEL file format. "},
				{ 33450 , "MD PrepDate Date the sample was prepared, as used in the Molecular Dynamics GEL file format. "},
				{ 33451 , "MD PrepTime Time the sample was prepared, as used in the Molecular Dynamics GEL file format. "},
				{ 33452 , "MD FileUnits Units for data in this file, as used in the Molecular Dynamics GEL file format. "},
				{ 33550 , "ModelPixelScaleTag Used in interchangeable GeoTIFF files. "},
				{ 33723 , "IPTC IPTC (International Press Telecommunications Council) metadata. "},
				{ 33918 , "INGR Packet Data Tag Intergraph Application specific storage. "},
				{ 33919 , "INGR Flag Registers Intergraph Application specific flags. "},
				{ 33920 , "IrasB Transformation Matrix Originally part of Intergraph's GeoTIFF tags, but likely understood by IrasB only. "},
				{ 33922 , "ModelTiepointTag part of Intergraph's GeoTIFF tags, but now used in interchangeable GeoTIFF files. "},
				{ 34264 , "ModelTransformationTag Used in interchangeable GeoTIFF files. "},
				{ 34377 , "Photoshop Collection of Photoshop 'Image Resource Blocks'. "},
				{ 34665 , "Exif IFD A pointer to the Exif IFD. "},
				{ 34675 , "ICC Profile ICC profile data. "},
				{ 34735 , "GeoKeyDirectoryTag Used in interchangeable GeoTIFF files. "},
				{ 34736 , "GeoDoubleParamsTag Used in interchangeable GeoTIFF files. "},
				{ 34737 , "GeoAsciiParamsTag Used in interchangeable GeoTIFF files. "},
				{ 34853 , "GPS IFD A pointer to the Exif-related GPS Info IFD. "},
				{ 34908 , "HylaFAX FaxRecvParams Used by HylaFAX. "},
				{ 34909 , "HylaFAX FaxSubAddress Used by HylaFAX. "},
				{ 34910 , "HylaFAX FaxRecvTime Used by HylaFAX. "},
				{ 37724 , "ImageSourceData Used by Adobe Photoshop. "},
				{ 40965 , "Interoperability IFD A pointer to the Exif-related Interoperability IFD. "},
				{ 42112 , "GDAL_METADATA Used by the GDAL library, holds an XML list of name=value 'metadata' values about the image as a whole, and about specific samples. "},
				{ 42113 , "GDAL_NODATA Used by the GDAL library, contains an ASCII encoded nodata or background pixel value. "},
				{ 50215 , "Oce Scanjob Description Used in the Oce scanning process. "},
				{ 50216 , "Oce Application Selector Used in the Oce scanning process. "},
				{ 50217 , "Oce Identification Number Used in the Oce scanning process. "},
				{ 50218 , "Oce ImageLogic Characteristics Used in the Oce scanning process. "},
				{ 50706 , "DNGVersion Used in IFD 0 of DNG files. "},
				{ 50707 , "DNGBackwardVersion Used in IFD 0 of DNG files. "},
				{ 50708 , "UniqueCameraModel Used in IFD 0 of DNG files. "},
				{ 50709 , "LocalizedCameraModel Used in IFD 0 of DNG files. "},
				{ 50710 , "CFAPlaneColor Used in Raw IFD of DNG files. "},
				{ 50711 , "CFALayout Used in Raw IFD of DNG files. "},
				{ 50712 , "LinearizationTable Used in Raw IFD of DNG files. "},
				{ 50713 , "BlackLevelRepeatDim Used in Raw IFD of DNG files. "},
				{ 50714 , "BlackLevel Used in Raw IFD of DNG files. "},
				{ 50715 , "BlackLevelDeltaH Used in Raw IFD of DNG files. "},
				{ 50716 , "BlackLevelDeltaV Used in Raw IFD of DNG files. "},
				{ 50717 , "WhiteLevel Used in Raw IFD of DNG files. "},
				{ 50718 , "DefaultScale Used in Raw IFD of DNG files. "},
				{ 50719 , "DefaultCropOrigin Used in Raw IFD of DNG files. "},
				{ 50720 , "DefaultCropSize Used in Raw IFD of DNG files. "},
				{ 50721 , "ColorMatrix1 Used in IFD 0 of DNG files. "},
				{ 50722 , "ColorMatrix2 Used in IFD 0 of DNG files. "},
				{ 50723 , "CameraCalibration1 Used in IFD 0 of DNG files. "},
				{ 50724 , "CameraCalibration2 Used in IFD 0 of DNG files. "},
				{ 50725 , "ReductionMatrix1 Used in IFD 0 of DNG files. "},
				{ 50726 , "ReductionMatrix2 Used in IFD 0 of DNG files. "},
				{ 50727 , "AnalogBalance Used in IFD 0 of DNG files. "},
				{ 50728 , "AsShotNeutral Used in IFD 0 of DNG files. "},
				{ 50729 , "AsShotWhiteXY Used in IFD 0 of DNG files. "},
				{ 50730 , "BaselineExposure Used in IFD 0 of DNG files. "},
				{ 50731 , "BaselineNoise Used in IFD 0 of DNG files. "},
				{ 50732 , "BaselineSharpness Used in IFD 0 of DNG files. "},
				{ 50733 , "BayerGreenSplit Used in Raw IFD of DNG files. "},
				{ 50734 , "LinearResponseLimit Used in IFD 0 of DNG files. "},
				{ 50735 , "CameraSerialNumber Used in IFD 0 of DNG files. "},
				{ 50736 , "LensInfo Used in IFD 0 of DNG files. "},
				{ 50737 , "ChromaBlurRadius Used in Raw IFD of DNG files. "},
				{ 50738 , "AntiAliasStrength Used in Raw IFD of DNG files. "},
				{ 50740 , "DNGPrivateData Used in IFD 0 of DNG files. "},
				{ 50741 , "MakerNoteSafety Used in IFD 0 of DNG files. "},
				{ 50778 , "CalibrationIlluminant1 Used in IFD 0 of DNG files. "},
				{ 50779 , "CalibrationIlluminant2 Used in IFD 0 of DNG files. "},
				{ 50780 , "BestQualityScale Used in Raw IFD of DNG files. "},
				{ 50784 , "Alias Layer Metadata Alias Sketchbook Pro layer usage description. "}	
			};
		}
	}
}
