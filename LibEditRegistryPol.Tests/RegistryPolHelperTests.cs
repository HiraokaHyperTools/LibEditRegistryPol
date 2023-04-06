using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibEditRegistryPol.Tests
{
    public class RegistryPolHelperTests
    {
        [Test]
        public void SetDWordTestAdding()
        {
            CollectionAssert.AreEqual(
                expected: new RegistryPolEntry[] { new RegistryPolEntry { Key = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", Value = "AlwaysAutoRebootAtScheduledTime", Type = RegistryPolEntry.REG_DWORD, Data = new byte[] { 1, 0, 0, 0 } } },
                actual: RegistryPolHelper.SetDWord(
                    entries: new RegistryPolEntry[0],
                    key: @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU",
                    value: @"AlwaysAutoRebootAtScheduledTime",
                    data: 1
                )
                    .ToArray()
            );
        }

        [Test]
        public void SetDWordTestReplacing()
        {
            CollectionAssert.AreEqual(
                expected: new RegistryPolEntry[] { new RegistryPolEntry { Key = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", Value = "AlwaysAutoRebootAtScheduledTime", Type = RegistryPolEntry.REG_DWORD, Data = new byte[] { 1, 0, 0, 0 } } },
                actual: RegistryPolHelper.SetDWord(
                    entries: new RegistryPolEntry[] { new RegistryPolEntry { Key = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", Value = "AlwaysAutoRebootAtScheduledTime", Type = RegistryPolEntry.REG_DWORD, Data = new byte[] { 0, 0, 0, 0 } } },
                    key: @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU",
                    value: @"AlwaysAutoRebootAtScheduledTime",
                    data: 1
                )
                    .ToArray()
            );
        }

        [Test]
        public void SetDWordTestReplacingLowerCase()
        {
            CollectionAssert.AreEqual(
                expected: new RegistryPolEntry[] { new RegistryPolEntry { Key = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", Value = "AlwaysAutoRebootAtScheduledTime", Type = RegistryPolEntry.REG_DWORD, Data = new byte[] { 1, 0, 0, 0 } } },
                actual: RegistryPolHelper.SetDWord(
                    entries: new RegistryPolEntry[] { new RegistryPolEntry { Key = @"software\policies\microsoft\windows\windowsupdate\au", Value = "alwaysautorebootatscheduledtime", Type = RegistryPolEntry.REG_DWORD, Data = new byte[] { 0, 0, 0, 0 } } },
                    key: @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU",
                    value: @"AlwaysAutoRebootAtScheduledTime",
                    data: 1
                )
                    .ToArray()
            );
        }

        [Test]
        public void SetStringTestAdding()
        {
            CollectionAssert.AreEqual(
                expected: new RegistryPolEntry[] { new RegistryPolEntry { Key = @"Key", Value = "Value", Type = RegistryPolEntry.REG_SZ, Data = new byte[] { 49, 0 } } },
                actual: RegistryPolHelper.SetString(
                    entries: new RegistryPolEntry[0],
                    key: @"Key",
                    value: @"Value",
                    data: "1"
                )
                    .ToArray()
            );
        }

        [Test]
        public void SetExpandStringTestAdding()
        {
            CollectionAssert.AreEqual(
                expected: new RegistryPolEntry[] { new RegistryPolEntry { Key = @"Key", Value = "Value", Type = RegistryPolEntry.REG_EXPAND_SZ, Data = new byte[] { 50, 0 } } },
                actual: RegistryPolHelper.SetString(
                    entries: new RegistryPolEntry[0],
                    key: @"Key",
                    value: @"Value",
                    data: "2",
                    szSpecifier: SzSpecifier.ExpandString
                )
                    .ToArray()
            );
        }

        [Test]
        public void SetMultiStringTestAdding()
        {
            CollectionAssert.AreEqual(
                expected: new RegistryPolEntry[] { new RegistryPolEntry { Key = @"Key", Value = "Value", Type = RegistryPolEntry.REG_MULTI_SZ, Data = new byte[] { 0x61, 0, 0, 0, 0x62, 0 } } },
                actual: RegistryPolHelper.SetString(
                    entries: new RegistryPolEntry[0],
                    key: @"Key",
                    value: @"Value",
                    data: "a\0b",
                    szSpecifier: SzSpecifier.MultiString
                )
                    .ToArray()
            );
        }

    }
}
