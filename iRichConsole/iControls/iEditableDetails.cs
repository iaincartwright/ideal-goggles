using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace iControls
{
    public enum ColumnFilter
    {
        Substring
    }

    public partial class iEditableDetails : UserControl
    {
        public iEditableDetails()
        {
            InitializeComponent();

            _lv.View = View.Details;

            editPopup.Visible = false;
        }

        private static string[] s_spaceToken = { " " };
        private static string[] s_commentToken = { "//" };
        private static char[] s_delims = { ' ', '\t', '\n', '\r', ',', '=' };

        private readonly List<string[]> _tokens = new List<string[]>();
        private string _filename = "";
        private int[] _maxColWidth;
        private bool _readOnly = false;
        private string[] _columnText = new string[0];

        public event EventHandler SelectionChanged;

        public void InsertLine(string a_aline)
        {
            var line = a_aline.Trim();

            if (line.Contains("//"))
            {   // we have a comment so split it off
                var tokens = line.Split(s_commentToken, 2, StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length == 2)
                {
                    _tokens.Add(new[] { "//" + tokens[1].Trim() });

                    line = tokens[0].Trim();
                }
            }

            if (line.StartsWith("//"))
                _tokens.Add(new[] { line });
            else if (string.IsNullOrEmpty(line))
                _tokens.Add(s_spaceToken);
            else
                _tokens.Add(line.Split(s_delims, StringSplitOptions.RemoveEmptyEntries));
        }

        public void InsertLines(string[] a_lines)
        {
            foreach (var aline in a_lines)
                InsertLine(aline);
        }

        public void Clear()
        {
            _tokens?.Clear();
        }

        public void EditIniFile(string a_filename)
        {
            _filename = a_filename;

            _tokens?.Clear();

			var lines = File.ReadAllLines(a_filename);

            InsertLines(lines);

            RedrawView();
        }

		public string this[MouseEventArgs a_e] => _lv.Columns.Count > 0 ? _lv.HitTest(a_e.X, a_e.Y).Item?.GetSubItemAt(a_e.X, a_e.Y)?.Text : null;

		public string[] this[int a_row] => a_row < _tokens.Count ? _tokens[a_row] : null;

        public void SaveFileAs(string a_filename)
        {
            _filename = a_filename;

            SaveFile();
        }

        public void SaveFile()
        {
            using (var sw = new StreamWriter(_filename))
            {
                var indent = "    ";
                var actualIndent = "";

                foreach (ListViewItem item in _lv.Items)
                {
                    if (item.Text.StartsWith("}"))
                        actualIndent = actualIndent.Substring(indent.Length);

                    var line = actualIndent;

                    foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                    {
                        line += subItem.Text + " ";
                    }

                    sw.WriteLine(line);

                    if (item.Text.StartsWith("{"))
                        actualIndent += indent;
                }
            }
        }

        public void SetColumnTitle(params string[] a_text)
        {
            _columnText = new string[a_text.Length];

            for (var i = 0; i < a_text.Length; i++)
            {
                _columnText[i] = a_text[i];
            }

            NormaliseColumnWidths();
        }

        private void NormaliseColumnWidths()
        {
            if (_lv.Columns.Count <= 0)
                return;

            for (var i = 0; i < _columnText.Length && i < _lv.Columns.Count; i++)
            {
                _lv.Columns[i].Text = _columnText[i];
            }

            _maxColWidth = new int[_lv.Columns.Count];

            foreach (ListViewItem lvi in _lv.Items)
            {
                for (var i = 0; i < _maxColWidth.Length; i++)
                {
                    if (lvi.SubItems.Count > i)
                        _maxColWidth[i] = Math.Max(_maxColWidth[i], lvi.SubItems[i].Text.Length);
                }
            }

            for (var i = 0; i < _lv.Columns.Count; i++)
            {
                if (_lv.Columns[i].Text.Length < _maxColWidth[i])
                    _lv.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                else
                    _lv.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        public void InsertThousandsSeparators(int a_col)
        {
            if (_tokens != null && _tokens.Count > 0 && a_col >= 0 && a_col < _tokens[0].Length)
            {
                foreach (var tokens in _tokens)
                {
                    float tempVal;

                    if (float.TryParse(tokens[a_col], out tempVal))
                    {
						tokens[a_col] = $"{tempVal,9:N0}";
                    }
                }
            }
        }

        public void ShowAsMegaBytes(int a_col)
        {
            if (_tokens != null && _tokens.Count > 0 && a_col >= 0 && a_col < _tokens[0].Length)
            {
                foreach (var tokens in _tokens)
                {
                    float tempVal;

                    if (float.TryParse(tokens[a_col], out tempVal))
                    {
                        tokens[a_col] = string.Format("{0:N}", (tempVal) / (1024.0f * 1024.0f) + 0.01f);
                    }
                }
            }
        }

        private string[] _groups = new string[0];
        private string[] _headers = new string[0];

        public void GroupBy(int a_col, string[] a_groups, string[] a_headers)
        {
            _lv.BeginUpdate();

            _lv.Groups.Clear();

            _groups = a_groups;
            _headers = a_headers;

            for (var i = 0; i < _groups.Length; i++)
            {
                _lv.Groups.Add(_groups[i], _headers[i]);
            }

            _lv.Groups.Add("other", "Others");

            foreach (ListViewItem lvi in _lv.Items)
            {
                var lvg = _lv.Groups["other"];

                for (var i = 0; i < _groups.Length; i++)
                {
                    if (lvi.SubItems[a_col].Text.ToLower().StartsWith(_groups[i].ToLower()))
                    {
                        lvg = _lv.Groups[i];

                        break;
                    }
                }

                lvi.Group = lvg;
            }

            _lv.EndUpdate();
        }

        public bool ShowGroups
        {
            get { return _lv.ShowGroups; }
            set { _lv.ShowGroups = value; }
        }

        public void GroupBy(int a_col)
        {
            _lv.BeginUpdate();

            _lv.Groups.Clear();

            var groups = new List<string>(64);

            foreach (ListViewItem lvi in _lv.Items)
            {
                if (!groups.Contains(lvi.SubItems[a_col].Text))
                {
                    groups.Add(lvi.SubItems[a_col].Text);

                    _lv.Groups.Add(lvi.SubItems[a_col].Text, lvi.SubItems[a_col].Text);
                }

                lvi.Group = _lv.Groups[lvi.SubItems[a_col].Text];
            }

            _lv.EndUpdate();
        }

        public void RedrawView()
        {
            var items = new List<ListViewItem>(_tokens.Count);

            var maxCols = 0;

            foreach (var tokens in _tokens)
            {
                if (tokens.Length > maxCols)
                    maxCols = tokens.Length;

                if (FilterTokens(tokens) == false)
                    items.Add(new ListViewItem(tokens));
            }

            _lv.BeginUpdate();

            _lv.Clear();

            for (var i = 0; i < maxCols; i++)
                _lv.Columns.Add(" ", 100);

            _lv.Items.AddRange(items.ToArray());

            NormaliseColumnWidths();

            _lv.EndUpdate();
        }

        private int _filterOnCol = -1;
        private ColumnFilter _filterType = ColumnFilter.Substring;
        private string _filterParam = "";

        private bool FilterTokens(string[] a_tokens)
        {
            var filterMe = false;

            if (_filterOnCol >= 0 && _filterOnCol < a_tokens.Length)
            {
                switch (_filterType)
                {
                    case ColumnFilter.Substring:
                        return !a_tokens[_filterOnCol].ToLowerInvariant().Contains(_filterParam);

                    default:
                        break;
                }
            }

            return filterMe;
        }

        private void _lv_MouseDoubleClick(object a_sender, MouseEventArgs a_e)
        {
            base.OnMouseDoubleClick(a_e);

            if (!_readOnly && a_e.Button == MouseButtons.Left && _lv.Columns.Count > 0)
            {
                // Get the item at the mouse pointer.
				var info = _lv.HitTest(a_e.X, a_e.Y);
				var lvi = info.Item;
				var subItem = info.Item?.SubItems[0];

                    if (lvi == null)
                    {// we need to add a new item
                        lvi = _lv.Items.Add("");

                        subItem = lvi.SubItems[0];
                    }
                    else
                    {
                        subItem = lvi.GetSubItemAt(a_e.X, a_e.Y);
                    }

                    if (subItem == null)
                        // we need to add a new subitem
                        subItem = lvi.SubItems.Add("");

                    EditCell(lvi, subItem);
                }
            }

        static int s_fudgeFactorX = 6;
        static int s_fudgeFactorY = 0;

        public void FontScaleUp()
        {
            _lv.Font = new Font(_lv.Font.Name, _lv.Font.SizeInPoints + 1.0f, FontStyle.Regular);
        }

        public void FontScaleDown()
        {
			if (_lv.Font.SizeInPoints > 1.0f)
                _lv.Font = new Font(_lv.Font.Name, _lv.Font.SizeInPoints - 1.0f, FontStyle.Regular);
        }

        private void EditCell(ListViewItem a_lvi, ListViewItem.ListViewSubItem a_subItem)
        {
            if (a_subItem != null)
            {
                a_subItem.Tag = a_lvi; // attach the item to the sub item so we can delete the item if subitem[0].Text = ""
                var colNumber = a_lvi.SubItems.IndexOf(a_subItem);

                editPopup.BringToFront();
                editPopup.Location = new Point(a_subItem.Bounds.Location.X + s_fudgeFactorX, a_subItem.Bounds.Location.Y + s_fudgeFactorY);
                editPopup.Font = _lv.Font;
                editPopup.Width = _lv.Columns[colNumber].Width;
                editPopup.Height = a_subItem.Bounds.Height;
                editPopup.Text = a_subItem.Text;
                editPopup.Tag = a_subItem;
                editPopup.Visible = true;
                editPopup.Focus();
                editPopup.SelectAll();
            }
        }

        public void ReplaceInColumn(int a_col, string a_replace, string a_with, bool a_ignoreCase)
        {
            var start = 0;
            var end = _tokens.Count;

            for (var i = start; i < end; i++)
            {
                if (_tokens[i].Length <= a_col)
                {// if a line contains insufficient tokens then set to default
                    var oldVal = _tokens[i];

                    _tokens[i] = new string[a_col + 1];

                    oldVal.CopyTo(_tokens[i], 0);

                    _tokens[i][a_col] = "";
                }

                var isReplace = _tokens[i][a_col];

                if (a_ignoreCase)
                    isReplace = isReplace.ToLowerInvariant();

                if (isReplace == a_replace)
                    _tokens[i][a_col] = a_with;
            }
        }

        public IDictionary<string, float> SumColumnByGroup(int a_sumCol, int a_groupCol)
        {
            var start = 0;
            var end = _lv.Items.Count;

            var accumulator = new SortedDictionary<string, float>();

            if (a_sumCol < _lv.Columns.Count && a_groupCol < _lv.Columns.Count)
            {
                float result = 0;

                for (var i = start; i < end; i++)
                {
                    var groupKey = _lv.Items[i].SubItems[a_groupCol].Text;
                    var sumValue = _lv.Items[i].SubItems[a_sumCol].Text;

                    if (float.TryParse(sumValue, out result))
                    {
                        if (accumulator.ContainsKey(groupKey))
                            accumulator[groupKey] += result;
                        else
                            accumulator[groupKey] = result;
                    }
                }
            }

            return accumulator;
        }

        public long SumColumn(int a_col)
        {
            return SumColumn(a_col, 0, _lv.Items.Count);
        }

        public long SumColumn(int a_col, int a_start, int a_end)
        {
            long total = 0;

            if (a_col < _lv.Columns.Count)
            {
                float accumulator = 0;

                for (var i = a_start; i < a_end; i++)
                {
					float result;
                    if (float.TryParse(_lv.Items[i].SubItems[a_col].Text, out result))
                    {
                        accumulator += result;
                    }
                }

                total = Convert.ToInt64(accumulator);
            }

            return total;
        }

        public static void AddDelimeter(char a_newDelim)
        {
            for (var i = 0; i < s_delims.Length; i++)
            {
                if (s_delims[i] == a_newDelim)
                    return;
            }

            var delims = new char[s_delims.Length + 1];

            for (var i = 0; i < s_delims.Length; i++)
            {
                delims[i] = s_delims[i];
            }

            delims[s_delims.Length] = a_newDelim;

            s_delims = delims;
        }

        private void _lv_ColumnClick(object a_sender, ColumnClickEventArgs a_e)
        {
            SortByColumn(a_e.Column);
        }

        // this toggles the sort order
        public void SortByColumn(int a_col)
        {
            _lv.ListViewItemSorter = new ListViewItemComparer(a_col);
        }

        public void SortByColumn(int a_col, SortOrder a_order)
        {
            _lv.ListViewItemSorter = new ListViewItemComparer(a_col, a_order);
        }

        public static char[] Delimeters
        {
            get { return s_delims; }
        }

        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        private void _lv_SelectedIndexChanged(object a_sender, EventArgs a_e)
        {
            if (SelectionChanged != null && _lv.SelectedItems.Count > 0)
                SelectionChanged(a_sender, a_e);
        }

        public string FilterParam
        {
            set { _filterParam = value.ToLowerInvariant(); }
        }

        public int FilterOnCol
        {
            set { _filterOnCol = value; }
        }
    }

    // Implements the manual sorting of items by columns.
    class ListViewItemComparer : IComparer
    {
        static SortOrder s_oldOrder = SortOrder.Ascending;
        static int s_oldCol = -1;

        private SortOrder _order = SortOrder.Ascending;
        private int _col;

        public ListViewItemComparer()
        {
            _col = 0;
        }

        public ListViewItemComparer(int a_column)
        {
            _col = a_column;

            if (_col == s_oldCol) // if it's the same column then toggle
                _order = s_oldOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            else  // otherwise use same as lat column
                _order = s_oldOrder;

            s_oldOrder = _order;
            s_oldCol = _col;
        }

        public ListViewItemComparer(int a_column, SortOrder a_order)
        {
            _col = a_column;

            _order = a_order;
        }

        public int Compare(object a_x, object a_y)
        {
            var xStr = "";
            var yStr = "";

            if (((ListViewItem)a_x).SubItems.Count > _col)
                xStr = ((ListViewItem)a_x).SubItems[_col].Text;

            if (((ListViewItem)a_y).SubItems.Count > _col)
                yStr = ((ListViewItem)a_y).SubItems[_col].Text;

            float xInt;
            float yInt;

            if (_order == SortOrder.Descending)
            {
                if (float.TryParse(xStr, out xInt) && float.TryParse(yStr, out yInt))
                    return yInt.CompareTo(xInt);
                else
                    return string.CompareOrdinal(yStr, xStr);
            }
            else
            {
                if (float.TryParse(xStr, out xInt) && float.TryParse(yStr, out yInt))
                    return xInt.CompareTo(yInt);
                else
                    return String.CompareOrdinal(xStr, yStr);
            }
        }

        public SortOrder Order
        {
            get { return _order; }
            set { _order = value; }
        }
    }
}