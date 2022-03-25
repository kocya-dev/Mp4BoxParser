using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mp4BoxParser
{
    internal static class BinaryReaderExtenstion
    {
        public static short ReadInt16BE(this BinaryReader br)
        {
            byte[] bytes = br.ReadBytes(2);
            return (short)((bytes[0] << 8) | bytes[1]);
        }

        public static ushort ReadUInt16BE(this BinaryReader br)
        {
            byte[] bytes = br.ReadBytes(2);
            return (ushort)((bytes[0] << 8) | bytes[1]);
        }

        public static int ReadInt32BE(this BinaryReader br)
        {
            byte[] bytes = br.ReadBytes(4);
            return (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
        }

        public static uint ReadUInt32BE(this BinaryReader br)
        {
            byte[] bytes = br.ReadBytes(4);
            return (uint)((bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3]);
        }

        public static long ReadInt64BE(this BinaryReader br)
        {
            byte[] bytes = br.ReadBytes(4);
            uint num1 = (uint)((bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3]);
            uint num2 = (uint)((bytes[4] << 24) | (bytes[5] << 16) | (bytes[6] << 8) | bytes[7]);
            return (long)(((ulong)num2 << 32) | num1);
        }

        public static ulong ReadUInt64BE(this BinaryReader br)
        {
            byte[] bytes = br.ReadBytes(4);
            uint num1 = (uint)((bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3]);
            uint num2 = (uint)((bytes[4] << 24) | (bytes[5] << 16) | (bytes[6] << 8) | bytes[7]);
            return (((ulong)num2 << 32) | num1);
        }

    }
}
