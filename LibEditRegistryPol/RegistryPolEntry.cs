using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LibEditRegistryPol
{
    [DebuggerDisplay("[{Key},{Value},{Type},{Data}]")]
    public class RegistryPolEntry : IComparable<RegistryPolEntry>
    {
        /// <summary>
        /// Path to the registry key
        /// </summary>
        /// <remarks>
        /// Do not include *HKEY_LOCAL_MACHINE* or *HKEY_CURRENT_USER* in the registry path. The location of the file determines which of these keys are used.
        /// </remarks>
        public string Key { get; set; }

        /// <summary>
        /// The name of the registry value.
        /// </summary>
        /// <remarks>
        /// The following values have special meaning for this field.
        /// 
        /// | Value             | Meaning |
        /// |-------------------|---------|
        /// | `**DeleteValues`  | A semicolon-delimited list of values to delete. Use as a value of the associated key. |
        /// | `**Del.valuename` | Deletes a single value. Use as a value of the associated key. |
        /// | `**DelVals`       | Deletes all values in a key.Use as a value of the associated key. |
        /// | `**DeleteKeys`    | A semicolon-delimited list of keys to delete. The value field needs to be terminated with a NULL or space immediately after **DeleteKeys. Example: **DeleteKeys/0; type;size;NoRun;NoFind |
        /// | `**SecureKey`     | **SecureKey=1 secures the key, giving administrators and the system full control, and giving users read-only access. **SecureKey= 0 resets access to the key to whatever is set on the root. For more information, see Access Rights and Access Masks.
        /// </remarks>
        public string Value { get; set; }

        /// <summary>
        /// The data type. The field can contain any of the registry value types defined in WinNT.h.
        /// REG_BINARY
        /// REG_DWORD
        /// REG_DWORD_LITTLE_ENDIAN
        /// REG_DWORD_BIG_ENDIAN
        /// REG_EXPAND_SZ
        /// REG_LINK
        /// REG_MULTI_SZ
        /// REG_NONE
        /// REG_QWORD
        /// REG_QWORD_LITTLE_ENDIAN
        /// REG_SZ
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// The user-supplied data.
        /// </summary>
        /// <remarks>
        /// If value, type, size, or data are missing or zero, only the registry key is created.
        /// </remarks>
        public ReadOnlyMemory<byte> Data { get; set; }

        #region Definition of Type
        /// <summary>
        /// No defined value type.
        /// </summary>
        public const int REG_NONE = (0);
        /// <summary>
        /// A null-terminated string. It's either a Unicode or an ANSI string, 
        /// depending on whether you use the Unicode or ANSI functions.
        /// </summary>
        public const int REG_SZ = (1);
        /// <summary>
        /// A null-terminated string that contains unexpanded references to environment variables, 
        /// for example, %PATH%. It's either a Unicode or an ANSI string, 
        /// depending on whether you use the Unicode or ANSI functions. 
        /// To expand the environment variable references, use the ExpandEnvironmentStrings function.
        /// </summary>
        public const int REG_EXPAND_SZ = (2);
        /// <summary>
        /// Binary data in any form.
        /// </summary>
        public const int REG_BINARY = (3);
        /// <summary>
        /// A 32-bit number.
        /// </summary>
        public const int REG_DWORD = (4);
        /// <summary>
        /// A 32-bit number in little-endian format. 
        /// Windows is designed to run on little-endian computer architectures. 
        /// Therefore, this value is defined as REG_DWORD in the Windows header files.
        /// </summary>
        public const int REG_DWORD_LITTLE_ENDIAN = (4);
        /// <summary>
        /// A 32-bit number in big-endian format. 
        /// Some UNIX systems support big-endian architectures.
        /// </summary>
        public const int REG_DWORD_BIG_ENDIAN = (5);
        /// <summary>
        /// A null-terminated Unicode string that contains the target path of a symbolic link 
        /// that was created by calling the RegCreateKeyEx function with REG_OPTION_CREATE_LINK.
        /// </summary>
        public const int REG_LINK = (6);
        /// <summary>
        /// A sequence of null-terminated strings, terminated by an empty string (\0). 
        /// The following is an example: String1\0String2\0String3\0LastString\0\0. 
        /// The first \0 terminates the first string, the second-from-last \0 terminates the last string, 
        /// and the final \0 terminates the sequence. Note that the final terminator must be factored into the length of the string.
        /// </summary>
        public const int REG_MULTI_SZ = (7);
        public const int REG_RESOURCE_LIST = (8);
        public const int REG_FULL_RESOURCE_DESCRIPTOR = (9);
        public const int REG_RESOURCE_REQUIREMENTS_LIST = (10);
        /// <summary>
        /// A 64-bit number.
        /// </summary>
        public const int REG_QWORD = (11);
        /// <summary>
        /// A 64-bit number in little-endian format. 
        /// Windows is designed to run on little-endian computer architectures. 
        /// Therefore, this value is defined as REG_QWORD in the Windows header files.
        /// </summary>
        public const int REG_QWORD_LITTLE_ENDIAN = (11);
        #endregion

        int IComparable<RegistryPolEntry>.CompareTo(RegistryPolEntry other) => CompareToCore(other);

        private int CompareToCore(RegistryPolEntry other)
        {
            int d;
            if (false
                || (d = RegistryPolHelper.CompareName(Key, other.Key)) != 0
                || (d = RegistryPolHelper.CompareName(Value, other.Value)) != 0
                || (d = Type.CompareTo(other.Type)) != 0
                || (d = RegistryPolHelper.CompareBytes(Data, other.Data)) != 0
            )
            {
                // nop
            }
            return d;
        }

        public override bool Equals(object obj) => CompareToCore((RegistryPolEntry)obj) == 0;
        public override int GetHashCode() => new { Key, Value, Type, Data, }.GetHashCode();
    }
}
