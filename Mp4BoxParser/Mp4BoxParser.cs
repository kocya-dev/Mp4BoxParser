using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mp4BoxParser
{
    public class Mp4Box : IComparable<Mp4Box>
    {
        public int Level { get; private set; }
        public Int64 Offset { get; private set; }
        public Int64 Size { get; private set; }
        public string Name { get; private set; }
        public bool IsExtSize{ get; private set; }
        public bool IsEmpty => Size == 0;
        public Int64 DataSize => IsExtSize ? Size - (8 + 8) : Size - 8;

        public Mp4Box(int level, Int64 offset, Int64 size, string name, bool useExtSize)
        {
            Level = level;
            Offset = offset;
            Size = size;
            Name = name;
            IsExtSize = useExtSize;
        }

        public int CompareTo(Mp4Box rhs)
        {
            return Offset.CompareTo(rhs.Offset);
        }

        static public Mp4Box Empty => new Mp4Box(0, 0, 0, string.Empty, false);
    }

    public class Mp4BoxParser : IDisposable
    {
        private int _level = 0;
        private FileStream _fs = null;
        private BinaryReader _br = null;
        private List<Mp4Box> _list = new List<Mp4Box>();
        private readonly List<string> _parentBoxs = new List<string>()
        {
            "moov",
            "trak",
            "mdia",
            "minf",
            "stbl"
        };

        public IReadOnlyList<Mp4Box> List => _list;

        public Mp4BoxParser(string filePath, bool searchChildBox)
        {
            _fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            _br = new BinaryReader(_fs, Encoding.UTF8);

            do
            {
                ;
            } while (FindNextBox(searchChildBox, _level, Mp4Box.Empty));
        }

        private bool FindNextBox(bool searchChildBox, int level, Mp4Box parentBox)
        {
            do
            {
                Mp4Box box = ReadBox(level);
                if (box.IsEmpty) return false;
                _list.Add(box);

                // 子検索あり、かつ現在のboxが親boxならば子boxを解析
                if (searchChildBox && _parentBoxs.Contains(box.Name))
                {
                    FindNextBox(searchChildBox, level + 1, box);
                }
                else
                {
                    // 子box or 子検索なしならスキップ
                    _br.BaseStream.Seek(box.DataSize, SeekOrigin.Current);
                }
            } while (!parentBox.IsEmpty && (_br.BaseStream.Position < (parentBox.Offset + parentBox.DataSize)));
            return true;
        }

        public void Dispose()
        {
            _list.Clear();
            if (_br != null)
            {
                _br.Dispose();
                _br = null;
            }
            if (_fs != null)
            {
                _fs.Dispose();
                _fs = null;
            }
        }
        private Mp4Box ReadBox(int level)
        {
            if (_br.BaseStream.Length - _br.BaseStream.Position < 8) return Mp4Box.Empty;

            Int64 offset = _br.BaseStream.Position;
            UInt32 size = _br.ReadUInt32BE();
            string name = new string(_br.ReadChars(4));

            // 64bitサイズ
            if (size == 1)
            {
                // size == 1の場合は64bit拡張サイズ領域を参照する
                if (_br.BaseStream.Length - _br.BaseStream.Position < 8) return Mp4Box.Empty;
                Int64 extSize = _br.ReadInt64BE();
                return new Mp4Box(level, offset, extSize, name, true);
            }

            // 32bitサイズ
            return new Mp4Box(level, offset, size, name, false);
        }
    }
}
