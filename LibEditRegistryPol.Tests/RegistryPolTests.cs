using NUnit.Framework;

namespace LibEditRegistryPol.Tests
{
    public class RegistryPolTests
    {
        private string ResolveSamples(string file) => Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            "..",
            "..",
            "..",
            "Samples",
            file
        );

        private static string Format(RegistryPolEntry entry) =>
            $"[{entry.Key};{entry.Value};{entry.Type};{BitConverter.ToString(entry.Data.ToArray())}]";

        [Test]
        public void ReadTest1()
        {
            var pol = File.ReadAllBytes(ResolveSamples("001Registry.pol"));
            var entries = RegistryPol.Read(pol).Select(Format).ToArray();
            Assert.AreEqual(expected: 7, actual: entries.Length);
            Assert.AreEqual(expected: @"[SOFTWARE\Policies\Microsoft\Windows\DeviceGuard;EnableVirtualizationBasedSecurity;4;00-00-00-00]", actual: entries[0]);
            Assert.AreEqual(expected: @"[SOFTWARE\Policies\Microsoft\Windows\DeviceGuard;**del.RequirePlatformSecurityFeatures;1;20-00-00-00]", actual: entries[1]);
            Assert.AreEqual(expected: @"[SOFTWARE\Policies\Microsoft\Windows\DeviceGuard;**del.HypervisorEnforcedCodeIntegrity;1;20-00-00-00]", actual: entries[2]);
            Assert.AreEqual(expected: @"[SOFTWARE\Policies\Microsoft\Windows\DeviceGuard;HVCIMATRequired;4;00-00-00-00]", actual: entries[3]);
            Assert.AreEqual(expected: @"[SOFTWARE\Policies\Microsoft\Windows\DeviceGuard;**del.LsaCfgFlags;1;20-00-00-00]", actual: entries[4]);
            Assert.AreEqual(expected: @"[SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU;AlwaysAutoRebootAtScheduledTime;4;01-00-00-00]", actual: entries[5]);
            Assert.AreEqual(expected: @"[SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU;AlwaysAutoRebootAtScheduledTimeMinutes;4;10-00-00-00]", actual: entries[6]);
        }

        [Test]
        public void ReadTest2()
        {
            var pol = File.ReadAllBytes(ResolveSamples("002Registry.pol"));
            var entries = RegistryPol.Read(pol).Select(Format).ToArray();
            entries.ToList().ForEach(Console.WriteLine);
            Assert.AreEqual(expected: 2, actual: entries.Length);
            Assert.AreEqual(expected: @"[SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer;;0;]", actual: entries[0]);
            Assert.AreEqual(expected: @"[SOFTWARE\Policies\Microsoft\Windows\Windows Error Reporting;DontShowUI;4;00-00-00-00]", actual: entries[1]);
        }

        [Test]
        [TestCase("001Registry.pol")]
        [TestCase("002Registry.pol")]
        public void ReadAndWriteTest(string fileName)
        {
            var pol = File.ReadAllBytes(ResolveSamples(fileName));
            var entries = RegistryPol.Read(pol);
            var written = RegistryPol.Write(entries);
            Assert.AreEqual(expected: pol, actual: written.ToArray());
        }

        [Test]
        public void WriteTest1()
        {
            var written = RegistryPol.Write(new RegistryPolEntry[0]);
            Deal("WriteTest1.bin", written.ToArray());
        }

        [Test]
        public void WriteTest2()
        {
            var written = RegistryPol.Write(
                new RegistryPolEntry[] {
                    new RegistryPolEntry { Key = "Key", Value = "Value", Type = 3, Data = new byte[] { 1, 2, 3, 4 }, }
                }
            );
            Deal("WriteTest2.bin", written.ToArray());
        }

        private void Deal(string fileName, byte[] bytes)
        {
#if true
            Assert.AreEqual(expected: File.ReadAllBytes(ResolveSamples(fileName)), actual: bytes);
#else
            File.WriteAllBytes(ResolveSamples(fileName), bytes);
#endif
        }

        [Test]
        [Ignore("TDD")]
        public void ReaderTest()
        {
            var bytes = File.ReadAllBytes(@"C:\Windows\System32\GroupPolicy\Machine\Registry.pol");
            RegistryPolEntry[] entries = RegistryPol.Read(bytes).ToArray();
        }

        [Test]
        [Ignore("TDD")]
        public void WriterTest()
        {
            ReadOnlyMemory<byte> written = RegistryPol.Write(new RegistryPolEntry[0]);
            byte[] bytes = written.ToArray();
        }
    }
}