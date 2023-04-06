using CommandLine;
using LibEditRegistryPol;
using System;
using System.Collections.Generic;
using System.IO;

namespace EditRegistryPol
{
    internal class Program
    {
        [Verb("set-dword")]
        private class SetDWordOpt
        {
            [Value(0, Required = true)]
            public string RegistryPolFile { get; set; }

            [Value(1, Required = true)]
            public string Key { get; set; }

            [Value(2, Required = true)]
            public string ValueName { get; set; }

            [Value(3, Required = true)]
            public string Data { get; set; }
        }

        [Verb("drop-item")]
        private class DropItemOpt
        {
            [Value(0, Required = true)]
            public string RegistryPolFile { get; set; }

            [Value(1, Required = true)]
            public string Key { get; set; }

            [Value(2, Required = true)]
            public string ValueName { get; set; }
        }

        [Verb("list")]
        private class ListOpt
        {
            [Value(0, Required = true)]
            public string RegistryPolFile { get; set; }
        }

        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<ListOpt, SetDWordOpt, DropItemOpt>(args)
                .MapResult<ListOpt, SetDWordOpt, DropItemOpt, int>(
                    DoList,
                    DoSetDWord,
                    DoDropItem,
                    ex => 1
                );
        }

        private static int DoDropItem(DropItemOpt arg)
        {
            var written = RegistryPol.Write(
                RegistryPolHelper.DropEntry(
                    ReadPolOrEmpty(arg.RegistryPolFile),
                    arg.Key,
                    arg.ValueName
                )
            );
            File.WriteAllBytes(arg.RegistryPolFile, written.ToArray());
            return 0;
        }

        private static int DoList(ListOpt arg)
        {
            foreach (var item in ReadPolOrEmpty(arg.RegistryPolFile))
            {
                Console.WriteLine($"[{item.Key};{item.Value};{item.Type};{item.Data.Length};{BitConverter.ToString(item.Data.ToArray())}]");
            }
            return 0;
        }

        private static IEnumerable<RegistryPolEntry> ReadPolOrEmpty(string registryPolFile)
        {
            var fi = new FileInfo(registryPolFile);
            if (fi.Exists && 8 <= fi.Length)
            {
                return RegistryPol.Read(File.ReadAllBytes(registryPolFile));
            }
            else
            {
                return new RegistryPolEntry[0];
            }
        }

        private static int DoSetDWord(SetDWordOpt arg)
        {
            var written = RegistryPol.Write(
                RegistryPolHelper.SetDWord(
                    ReadPolOrEmpty(arg.RegistryPolFile),
                    arg.Key,
                    arg.ValueName,
                    ConvertToInt32(arg.Data)
                )
            );
            File.WriteAllBytes(arg.RegistryPolFile, written.ToArray());
            return 0;
        }

        private static int ConvertToInt32(string data)
        {
            data = data.ToLowerInvariant();

            if (data.StartsWith("-"))
            {
                return Convert.ToInt32(data);
            }
            else if (data.StartsWith("0x"))
            {
                return (int)Convert.ToUInt32(data.Substring(2), 16);
            }
            else
            {
                return (int)Convert.ToUInt32(data);
            }
        }
    }
}
