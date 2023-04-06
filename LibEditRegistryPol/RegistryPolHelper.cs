using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibEditRegistryPol
{
    public static class RegistryPolHelper
    {
        public static int CompareName(string a, string b) => a.ToLowerInvariant().CompareTo(b.ToLowerInvariant());
        public static int CompareBytes(ReadOnlyMemory<byte> a, ReadOnlyMemory<byte> b) => a.Span.SequenceCompareTo(b.Span);

        public static IEnumerable<RegistryPolEntry> SetEntry(
            IEnumerable<RegistryPolEntry> entries,
            string key,
            string value,
            int type,
            ReadOnlyMemory<byte> data
        )
        {
            return DropEntry(entries, key, value)
                .Concat(
                    new RegistryPolEntry[] {
                        new RegistryPolEntry
                        {
                            Key = key,
                            Value = value,
                            Type = type,
                            Data = data,
                        }
                    }
                );
        }

        public static IEnumerable<RegistryPolEntry> DropEntry(
            IEnumerable<RegistryPolEntry> entries,
            string key,
            string value
        )
        {
            return entries
                .Where(entry => true
                    && CompareName(entry.Key, key) != 0
                    && CompareName(entry.Value, value) != 0
                );
        }

        public static IEnumerable<RegistryPolEntry> SetDWord(IEnumerable<RegistryPolEntry> entries, string key, string value, int data)
        {
            var bytes = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(bytes, data);

            return SetEntry(entries, key, value, RegistryPolEntry.REG_DWORD, bytes);
        }

        public static IEnumerable<RegistryPolEntry> SetString(IEnumerable<RegistryPolEntry> entries, string key, string value, string data, SzSpecifier szSpecifier = SzSpecifier.String)
        {
            var bytes = Encoding.Unicode.GetBytes(data);

            return SetEntry(entries, key, value, (int)szSpecifier, bytes);
        }
    }
}
