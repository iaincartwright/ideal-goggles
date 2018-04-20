using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace iControls
{
    public enum MinAlloc
    {
        Bytes1 = 0,
        Bytes2 = 1,
        Bytes4 = 2,
        Bytes8 = 3,
        Bytes16 = 4,
        Bytes32 = 5,
        Bytes64 = 6,
        Bytes128 = 7,
        Bytes256 = 8,
        Bytes512 = 9,
        Bytes1024 = 10,
        Bytes2048 = 11,
        Bytes4096 = 12
    }

    public partial class iMemoryView : UserControl
    {
        class HeapAllocation
        {
            private int _startOffset;
            private int _length;

            public HeapAllocation(int a_start, int a_len)
            {
                _startOffset = a_start;
                _length = a_len;
            }

            public int StartOffset
            {
                get { return _startOffset; }
                set { _startOffset = value; }
            }

            public int Length
            {
                get { return _length; }
                set { _length = value; }
            }
        }

        SortedDictionary<int, HeapAllocation> _heapAllocs;
        List<HeapAllocation> _heapFrees;
        Bitmap _bitmap;

        public iMemoryView()
        {
            InitializeComponent();

            Reset();
        }

        public void Reset()
        {
            _heapAllocs = new SortedDictionary<int, HeapAllocation>();
            _heapFrees = new List<HeapAllocation>(4096);

            _pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

            _images[0] = Image.FromStream(GetEmbeddedFile("chunk_yel.png"));
            _images[1] = Image.FromStream(GetEmbeddedFile("chunk_blu.png"));
            _images[2] = Image.FromStream(GetEmbeddedFile("chunk_grn.png"));
            _images[3] = Image.FromStream(GetEmbeddedFile("chunk_red.png"));
            _images[4] = Image.FromStream(GetEmbeddedFile("chunk_mag.png"));
        }

        /// <summary>
        /// gets or sets the total maximum size of the heap. Causes a redraw of the map when a new size is set
        /// </summary>
        public int HeapSize
        {
            get { return _heapSize; }
            set { _heapSize = value; RefreshMap(); }
        }

        public void AddAllocation(int a_start, int a_length)
        {
            if (_heapAllocs.ContainsKey(a_start))
            {
                _heapFrees.Add(_heapAllocs[a_start]);

                _heapAllocs[a_start] = new HeapAllocation(a_start, a_length);
            }
            else
            {
                _heapAllocs.Add(a_start, new HeapAllocation(a_start, a_length));
            }
        }

        public void FreeAlloc(int a_start)
        {
            if (_heapAllocs.ContainsKey(a_start))
            {
                _heapAllocs.Remove(a_start);
            }
        }

        private static Color s_backgroundColor = Color.PaleGoldenrod;

        public void InitHeap(string a_heapName, DateTime a_heapKey, int a_heapSize, MinAlloc a_minAlloc, int a_stackEntries)
        {
        }

        public void PageUp()
        {
        }

        public void PageDown()
        {
        }

        public void ScrollUp()
        {
        }

        public void ScrollDown()
        {
        }

        public void ZoomIn()
        {
        }

        public void ZoomOut()
        {
        }

        public void Home()
        {
        }

        public void End()
        {
        }

        public void DeinitHeap()
        {
        }

        public void MarkEmptySection(int a_offset)
        {
        }

        public void HeapViewUpdate()
        {
        }

        public void DrawBitmap(Bitmap a_targetBitmap, Image a_image)
        {
            if (a_targetBitmap == null)
                return;

            using (Graphics gd = Graphics.FromImage(a_targetBitmap))
            {
                gd.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                TextureBrush brush = new TextureBrush(a_image);

                // the first line
                int startWidth = _squareStartWide * a_image.Width;
                int startHigh = _squareStartHigh * a_image.Width;
                int firstWidth = _squaresFirstWide * a_image.Width;
                int firstHigh = _squaresFirstHigh * a_image.Width;

                if (firstWidth > 0 && firstHigh > 0)
                    gd.FillRectangle(brush, startWidth, startHigh, firstWidth, firstHigh);

                // the rectangular block
                int width = _squaresAcross * a_image.Width;
                int height = _squaresDown * a_image.Height;

                if (width > 0 && height > 0)
                    gd.FillRectangle(brush, 0, startHigh + firstHigh, width, height);

                // the last line
                int remainWidth = _squaresRemain * a_image.Width;

                if (remainWidth > 0)
                    gd.FillRectangle(brush, 0, startHigh + firstHigh + height, remainWidth, a_image.Height);

                brush.Dispose();
            }
        }

        /// <summary>
        /// The number of bytes represented by a minimum allocation square
        /// </summary>
        public int BytesPerSquare
        {
            get { return _bytesPerSquare; }
            set { if (_bytesPerSquare != value) { _bytesPerSquare = value; RefreshMap(); } }
        }

        /// <summary>
        /// The size in pixels of the minimum allocation square
        /// </summary>
        public int SquareSize
        {
            get { return _squareSize; }
            set { _squareSize = value; }
        }

        Image[] _images = new Image[5];

        public void RefreshMap()
        {
            if (_bytesPerSquare == 0)
                return;

            if (_squareSize == 0)
                return;

            if (_heapSize == 0)
                return;

            if (_pictureBox.Width < _squareSize)
                return;

            if (_pictureBox.Height < _squareSize)
                return;

            if (_heapAllocs == null)
                return;

            _bitmap = new Bitmap(_pictureBox.Width, _pictureBox.Height);

            // set the entire picture box to gray
            ClearBitmap(_bitmap);

            // recalculate variables dependant on window size
            // and scroll thumb positions
            CalculateOneTimers();

            // set the area covered by the heap to turquoise
            CalculateSquares(0, _heapSize);

            DrawBitmap(_bitmap, GetFreeImage());

            // now draw the actual allocations
            SortedDictionary<int, HeapAllocation>.ValueCollection heapEntries = _heapAllocs.Values;

            int heapImage = 0;
            foreach (HeapAllocation heapAlloc in heapEntries)
            {
                CalculateSquares(heapAlloc.StartOffset, heapAlloc.Length);

                DrawBitmap(_bitmap, _images[heapImage]);

                heapImage++;

                if (heapImage >= _images.Length)
                    heapImage = 0;
            }

            if (_pictureBox.Image != null)
                _pictureBox.Image.Dispose();

            _pictureBox.Image = _bitmap;
        }

        private Image GetFreeImage()
        {
            if (_freeImage == null)
            {
                _freeImage = new Bitmap(_squareSize, _squareSize);

                for (int i = 0; i < _squareSize; i++)
                    for (int j = 0; j < _squareSize; j++)
                        _freeImage.SetPixel(i, j, Color.PaleTurquoise);
            }

            return _freeImage;
        }

        private Bitmap _freeImage;

        private int _heapSize = 1024 * 1024;        // maximum heap value in bytes
        private int _bytesPerSquare = 8;           // bytes represented by one square
        private int _squareSize = 6;                // pixels on a side

        private int _squaresTotal;              // _heapSize/__bytesPerSquare
        private int _squaresAcross;             // (window width / _squareSize)
        private int _squaresDown;               // (window height / _squaresAcross)
        private int _squaresOffset;             // 
        private int _squaresRemain;

        private int _squareStartWide = 0;
        private int _squareStartHigh = 0;
        private int _squaresFirstWide = 0;
        private int _squaresFirstHigh = 0;

        private void ClearBitmap(Bitmap a_bitmap)
        {
            using (Graphics gd = Graphics.FromImage(a_bitmap))
            {
                gd.Clear(s_backgroundColor);
            }
        }

        int _startOffset = 0;
        int _squaresHeapDown;

        private void CalculateSquares(int a_allocBase, int a_allocSize)
        {
            _squaresOffset = (a_allocBase / _bytesPerSquare) - _startOffset;
            _squaresTotal = (a_allocSize + (_bytesPerSquare - 1)) / _bytesPerSquare;

            _squareStartWide = _squaresOffset % _squaresAcross;
            _squareStartHigh = _squaresOffset / _squaresAcross;
            _squaresFirstWide = Math.Min(_squaresAcross - _squareStartWide, _squaresTotal);
            _squaresFirstHigh = _squaresFirstWide > 0 ? 1 : 0;

            _squaresDown = (_squaresTotal - _squaresFirstWide) / _squaresAcross;
            _squaresRemain = (_squaresTotal - _squaresFirstWide) % _squaresAcross;
        }

        private void CalculateOneTimers()
        {
            _squaresAcross = _pictureBox.Width / _squareSize;
            _squaresHeapDown = (_heapSize / _squareSize) / _squaresAcross;

            vScroll.Minimum = 0;
            vScroll.Maximum = (_squaresHeapDown * _squareSize) - (_pictureBox.Height / _squareSize) + 15;

            _startOffset = vScroll.Value * _squaresAcross;
        }

        private void _pictureBox_Resize(object a_sender, EventArgs a_e)
        {
            float oldMax = (float)vScroll.Maximum;

            // set the new maximum value for the slider
            CalculateOneTimers();   

            // recalculate the vertical scroll thumb position
            vScroll.Value = (int)((float)vScroll.Value * ((float)vScroll.Maximum / oldMax));

            RefreshMap();
        }

        public Stream GetEmbeddedFile(string a_name)
        {
            try
            {
                Assembly assembly = Assembly.GetAssembly(GetType());

                return assembly.GetManifestResourceStream("iControls.Resources." + a_name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        private void vScroll_Scroll(object a_sender, ScrollEventArgs a_e)
        {
            RefreshMap();
        }

        private void hScroll_Scroll(object a_sender, ScrollEventArgs a_e)
        {
            RefreshMap();
        }

        /// <summary>
        /// sets the background colour of the control, outside of any memory area
        /// </summary>
        public Color BackgroundColor
        {
            get { return s_backgroundColor; }
            set { s_backgroundColor = value; }
        }
    }
}

