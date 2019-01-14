# Spreads.Native

Spreads' native dependencies and low-level IL methods.

## Overview

Currently implemented methods from [Blosc](https://github.com/Blosc/c-blosc/): 
SIMD-optimized shuffle/unshuffle and LZ4, Zstd, Zlib/GZip/Deflate compression/decompression. 
Currently works on Windows x64/x86 and Linux x64 (tested on WSL & Docker Ubuntu). Targets `netstandard2.0`.

Unsafe methods from archived [Spreads.Unsafe](https://github.com/Spreads/Spreads.Unsafe).

## License

MPL 2.0. See the [lincese file](https://github.com/Spreads/Spreads.Native/blob/master/LICENSE.txt) and [third-party licenses](https://github.com/Spreads/Spreads.Native/blob/master/LICENSE.Dependencies.txt).
