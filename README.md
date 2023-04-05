# HiraokaHyperTools.LibEditRegistryPol

[![Nuget](https://img.shields.io/nuget/v/HiraokaHyperTools.LibEditRegistryPol)](https://www.nuget.org/packages/HiraokaHyperTools.LibEditRegistryPol)

This will provide reader and writer for `Registry.pol` file format: [Registry Policy File Format | Microsoft Learn](https://learn.microsoft.com/en-us/previous-versions/windows/desktop/policy/registry-policy-file-format)

Links: [Doxygen](https://hiraokahypertools.github.io/LibEditRegistryPol/html/)

## Reader sample

```cs
var bytes = File.ReadAllBytes(@"C:\Windows\System32\GroupPolicy\Machine\Registry.pol");
RegistryPolEntry[] entries = RegistryPol.Read(bytes).ToArray();
```

## Writer sample

```cs
ReadOnlyMemory<byte> written = RegistryPol.Write(new RegistryPolEntry[0]);
byte[] bytes = written.ToArray();
```
