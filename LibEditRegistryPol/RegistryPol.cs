using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace LibEditRegistryPol
{
    /// <summary>
    /// Implement simple reader/writer for Registry Policy File Format
    /// </summary>
    /// <remarks>
    /// See also:
    /// 
    /// - [Registry Policy File Format | Microsoft Learn](https://learn.microsoft.com/en-us/previous-versions/windows/desktop/policy/registry-policy-file-format)
    /// </remarks>
    public static class RegistryPol
    {
        public static readonly uint REGFILE_SIGNATURE = 0x67655250;
        public static readonly uint REGISTRY_FILE_VERSION = 1;

        private const int Opener = '[';
        private const int Closer = ']';
        private const int Divider = ';';

        public static ReadOnlyMemory<byte> Write(IEnumerable<RegistryPolEntry> entries)
        {
            using (var buf = new MemoryStream())
            using (var writer = new BinaryWriter(buf, Encoding.Unicode, true))
            {
                void WriteWideString(string text)
                {
                    writer.Write(Encoding.Unicode.GetBytes(text + '\0'));
                }

                writer.Write(REGFILE_SIGNATURE);
                writer.Write(REGISTRY_FILE_VERSION);
                foreach (var entry in entries)
                {
                    writer.Write((ushort)Opener);
                    WriteWideString(entry.Key);
                    writer.Write((ushort)Divider);
                    WriteWideString(entry.Value);
                    writer.Write((ushort)Divider);
                    writer.Write(entry.Type);
                    writer.Write((ushort)Divider);
                    writer.Write(entry.Data.Length);
                    writer.Write((ushort)Divider);
                    writer.Write(entry.Data.ToArray());
                    writer.Write((ushort)Closer);
                }
                return buf.ToArray();
            }
        }

        public static IEnumerable<RegistryPolEntry> Read(ReadOnlyMemory<byte> file)
        {
            var span = file.Span;

            if (false
                || BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(0, 4)) != REGFILE_SIGNATURE
                || BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4, 4)) != REGISTRY_FILE_VERSION
            )
            {
                throw new InvalidDataException();
            }

            var position = 8;

            void Ensure(ReadOnlySpan<byte> buf, int symbol, ref int offset)
            {
                if (BinaryPrimitives.ReadUInt16LittleEndian(buf.Slice(offset, 2)) != symbol)
                {
                    throw new InvalidDataException();
                }

                offset += 2;
            }

            string ReadWideString(ReadOnlySpan<byte> buf, ref int offset)
            {
                int iter = offset;
                while (true)
                {
                    if (buf[iter] == 0 && buf[iter + 1] == 0)
                    {
                        var bytes = buf.Slice(offset, iter - offset).ToArray();
                        offset = iter + 2;
                        return Encoding.Unicode.GetString(bytes, 0, bytes.Length);
                    }
                    else
                    {
                        iter += 2;
                    }
                }
            }

            var list = new List<RegistryPolEntry>();

            while (position < span.Length)
            {
                Ensure(span, Opener, ref position);
                var key = ReadWideString(span, ref position);
                Ensure(span, Divider, ref position);
                var value = ReadWideString(span, ref position);
                Ensure(span, Divider, ref position);
                var type = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(position, 4));
                position += 4;
                Ensure(span, Divider, ref position);
                var size = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(position, 4));
                position += 4;
                Ensure(span, Divider, ref position);
                var data = file.Slice(position, size);
                position += size;
                Ensure(span, Closer, ref position);
                list.Add(
                    new RegistryPolEntry
                    {
                        Key = key,
                        Value = value,
                        Type = type,
                        Data = data,
                    }
                );
            }

            return new ReadOnlyCollection<RegistryPolEntry>(list);
        }
    }
}
