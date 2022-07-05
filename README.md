# [Spreads.Native](https://www.nuget.org/packages/Spreads.Native)

### Native compression

[Spreads.Native.Compression](http://docs.dataspreads.io/spreads/libs/native/api/Spreads.Native.Compression.html) class exposes methods from [Blosc](https://github.com/Blosc/c-blosc/): 
* SIMD-optimized shuffle/unshuffle.
* Compression: LZ4, Zstd, Zlib/GZip/Deflate compression/decompression. 
Currently works on Windows x64/x86 and Linux x64 (tested on WSL & Docker Ubuntu). Targets `netstandard2.0`.

### Mimalloc

Full [mimalloc](https://github.com/microsoft/mimalloc) API in .NET.

### Cpu.GetCurrentCoreId method

Equivalent of [Thread.GetCurrentProcessorId](https://docs.microsoft.com/en-us/dotnet/api/system.threading.thread.getcurrentprocessorid?view=netcore-3.1) method, 
but which works on .NET Standard 2.0 and guarantees that the returned value could be used directly
as an index in arrays with Cpu.CoreCount length. This allows to avoid expensive modulo 
operation in the most common use cases of per-core data structures.

## License

MPL 2.0. See the [license file](https://github.com/Spreads/Spreads.Native/blob/master/LICENSE.txt) and [third-party licenses](https://github.com/Spreads/Spreads.Native/blob/master/LICENSE.Dependencies.txt).
