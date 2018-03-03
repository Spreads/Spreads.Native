extern crate libc;
use libc::{c_char, c_int, c_void, size_t};

pub const BLOSC_VERSION_MAJOR: u32 = 1;
pub const BLOSC_VERSION_MINOR: u32 = 14;
pub const BLOSC_VERSION_RELEASE: u32 = 0;
pub const BLOSC_VERSION_STRING: &'static str = "1.14.0";
pub const BLOSC_VERSION_REVISION: &'static str = "$Rev$";
pub const BLOSC_VERSION_DATE: &'static str = "$Date:: 2018-02-23 #$";
pub const BLOSCLZ_VERSION_STRING: &'static str = "1.1.0";
pub const BLOSC_VERSION_FORMAT: u32 = 2;
pub const BLOSC_MIN_HEADER_LENGTH: u32 = 16;
pub const BLOSC_MAX_OVERHEAD: u32 = 16;
pub const BLOSC_MAX_BUFFERSIZE: u32 = 2147483631;
pub const BLOSC_MAX_TYPESIZE: u32 = 255;
pub const BLOSC_MAX_THREADS: u32 = 256;
pub const BLOSC_NOSHUFFLE: u32 = 0;
pub const BLOSC_SHUFFLE: u32 = 1;
pub const BLOSC_BITSHUFFLE: u32 = 2;
pub const BLOSC_DOSHUFFLE: u32 = 1;
pub const BLOSC_MEMCPYED: u32 = 2;
pub const BLOSC_DOBITSHUFFLE: u32 = 4;
pub const BLOSC_BLOSCLZ: u32 = 0;
pub const BLOSC_LZ4: u32 = 1;
pub const BLOSC_LZ4HC: u32 = 2;
pub const BLOSC_SNAPPY: u32 = 3;
pub const BLOSC_ZLIB: u32 = 4;
pub const BLOSC_ZSTD: u32 = 5;
pub const BLOSC_BLOSCLZ_COMPNAME: &'static str = "blosclz";
pub const BLOSC_LZ4_COMPNAME: &'static str = "lz4";
pub const BLOSC_LZ4HC_COMPNAME: &'static str = "lz4hc";
pub const BLOSC_SNAPPY_COMPNAME: &'static str = "snappy";
pub const BLOSC_ZLIB_COMPNAME: &'static str = "zlib";
pub const BLOSC_ZSTD_COMPNAME: &'static str = "zstd";
pub const BLOSC_BLOSCLZ_LIB: u32 = 0;
pub const BLOSC_LZ4_LIB: u32 = 1;
pub const BLOSC_SNAPPY_LIB: u32 = 2;
pub const BLOSC_ZLIB_LIB: u32 = 3;
pub const BLOSC_ZSTD_LIB: u32 = 4;
pub const BLOSC_BLOSCLZ_LIBNAME: &'static str = "BloscLZ";
pub const BLOSC_LZ4_LIBNAME: &'static str = "LZ4";
pub const BLOSC_SNAPPY_LIBNAME: &'static str = "Snappy";
pub const BLOSC_ZLIB_LIBNAME: &'static str = "Zlib";
pub const BLOSC_ZSTD_LIBNAME: &'static str = "Zstd";
pub const BLOSC_BLOSCLZ_FORMAT: u32 = 0;
pub const BLOSC_LZ4_FORMAT: u32 = 1;
pub const BLOSC_LZ4HC_FORMAT: u32 = 1;
pub const BLOSC_SNAPPY_FORMAT: u32 = 2;
pub const BLOSC_ZLIB_FORMAT: u32 = 3;
pub const BLOSC_ZSTD_FORMAT: u32 = 4;
pub const BLOSC_BLOSCLZ_VERSION_FORMAT: u32 = 1;
pub const BLOSC_LZ4_VERSION_FORMAT: u32 = 1;
pub const BLOSC_LZ4HC_VERSION_FORMAT: u32 = 1;
pub const BLOSC_SNAPPY_VERSION_FORMAT: u32 = 1;
pub const BLOSC_ZLIB_VERSION_FORMAT: u32 = 1;
pub const BLOSC_ZSTD_VERSION_FORMAT: u32 = 1;
pub const BLOSC_ALWAYS_SPLIT: u32 = 1;
pub const BLOSC_NEVER_SPLIT: u32 = 2;
pub const BLOSC_AUTO_SPLIT: u32 = 3;
pub const BLOSC_FORWARD_COMPAT_SPLIT: u32 = 4;

extern "C" {
    /// Initialize the Blosc library environment.
    ///
    /// You must call this previous to any other Blosc call, unless you want
    /// Blosc to be used simultaneously in a multi-threaded environment, in
    /// which case you should *exclusively* use the
    /// blosc_compress_ctx()/blosc_decompress_ctx() pair (see below).
    pub fn blosc_init();

    /// Destroy the Blosc library environment.
    ///
    /// You must call this after to you are done with all the Blosc calls,
    /// unless you have not used blosc_init() before (see blosc_init()
    /// above).
    pub fn blosc_destroy();

    /// Compress a block of data in the `src` buffer and returns the size of
    /// the compressed block.  The size of `src` buffer is specified by
    /// `nbytes`.  There is not a minimum for `src` buffer size (`nbytes`).
    ///
    /// `clevel` is the desired compression level and must be a number
    /// between 0 (no compression) and 9 (maximum compression).
    ///
    /// `doshuffle` specifies whether the shuffle compression preconditioner
    /// should be applied or not.  BLOSC_NOSHUFFLE means not applying it,
    /// BLOSC_SHUFFLE means applying it at a byte level and BLOSC_BITSHUFFLE
    /// at a bit level (slower but may achieve better entropy alignment).
    ///
    /// `typesize` is the number of bytes for the atomic type in binary
    /// `src` buffer.  This is mainly useful for the shuffle preconditioner.
    /// For implementation reasons, only a 1 < typesize < 256 will allow the
    /// shuffle filter to work.  When typesize is not in this range, shuffle
    /// will be silently disabled.
    ///
    /// The `dest` buffer must have at least the size of `destsize`.  Blosc
    /// guarantees that if you set `destsize` to, at least,
    /// (`nbytes`+BLOSC_MAX_OVERHEAD), the compression will always succeed.
    /// The `src` buffer and the `dest` buffer can not overlap.
    ///
    /// Compression is memory safe and guaranteed not to write the `dest`
    /// buffer more than what is specified in `destsize`.
    ///
    /// If `src` buffer cannot be compressed into `destsize`, the return
    /// value is zero and you should discard the contents of the `dest`
    /// buffer.
    ///
    /// A negative return value means that an internal error happened.  This
    /// should never happen.  If you see this, please report it back
    /// together with the buffer data causing this and compression settings.
    ///
    /// Environment variables
    /// ---------------------
    ///
    /// blosc_compress() honors different environment variables to control
    /// internal parameters without the need of doing that programatically.
    /// Here are the ones supported:
    ///
    /// BLOSC_CLEVEL=(INTEGER): This will overwrite the `clevel` parameter
    /// before the compression process starts.
    ///
    /// BLOSC_SHUFFLE=[NOSHUFFLE | SHUFFLE | BITSHUFFLE]: This will
    /// overwrite the `doshuffle` parameter before the compression process
    /// starts.
    ///
    /// BLOSC_TYPESIZE=(INTEGER): This will overwrite the `typesize`
    /// parameter before the compression process starts.
    ///
    /// BLOSC_COMPRESSOR=[BLOSCLZ | LZ4 | LZ4HC | SNAPPY | ZLIB]: This will
    /// call blosc_set_compressor(BLOSC_COMPRESSOR) before the compression
    /// process starts.
    ///
    /// BLOSC_NTHREADS=(INTEGER): This will call
    /// blosc_set_nthreads(BLOSC_NTHREADS) before the compression process
    /// starts.
    ///
    /// BLOSC_BLOCKSIZE=(INTEGER): This will call
    /// blosc_set_blocksize(BLOSC_BLOCKSIZE) before the compression process
    /// starts.  *NOTE:* The blocksize is a critical parameter with
    /// important restrictions in the allowed values, so use this with care.
    ///
    /// BLOSC_NOLOCK=(ANY VALUE): This will call blosc_compress_ctx() under
    /// the hood, with the `compressor`, `blocksize` and
    /// `numinternalthreads` parameters set to the same as the last calls to
    /// blosc_set_compressor(), blosc_set_blocksize() and
    /// blosc_set_nthreads().  BLOSC_CLEVEL, BLOSC_SHUFFLE, BLOSC_TYPESIZE
    /// environment vars will also be honored.
    ///
    /// BLOSC_SPLITMODE=[ FORWARD_COMPAT | AUTO | ALWAYS | NEVER ]:
    /// This will call blosc_set_splitmode() with the different supported values.
    /// See blosc_set_splitmode() docstrings for more info on each mode.
    pub fn blosc_compress(
        clevel: c_int,
        doshuffle: c_int,
        typesize: size_t,
        nbytes: size_t,
        src: *const c_void,
        dest: *mut c_void,
        destsize: size_t,
    ) -> c_int;

    /// Context interface to blosc compression. This does not require a call
    /// to blosc_init() and can be called from multithreaded applications
    /// without the global lock being used, so allowing Blosc be executed
    /// simultaneously in those scenarios.
    ///
    /// It uses the same parameters than the blosc_compress() function plus:
    ///
    /// `compressor`: the string representing the type of compressor to use.
    ///
    /// `blocksize`: the requested size of the compressed blocks.  If 0, an
    /// automatic blocksize will be used.
    ///
    /// `numinternalthreads`: the number of threads to use internally.
    ///
    /// A negative return value means that an internal error happened.  This
    /// should never happen.  If you see this, please report it back
    /// together with the buffer data causing this and compression settings.
    pub fn blosc_compress_ctx(
        clevel: c_int,
        doshuffle: c_int,
        typesize: size_t,
        nbytes: size_t,
        src: *const c_void,
        dest: *mut c_void,
        destsize: size_t,
        compressor: *const c_char,
        blocksize: size_t,
        numinternalthreads: c_int,
    ) -> c_int;

    /// Decompress a block of compressed data in `src`, put the result in
    /// `dest` and returns the size of the decompressed block.
    ///
    /// The `src` buffer and the `dest` buffer can not overlap.
    ///
    /// Decompression is memory safe and guaranteed not to write the `dest`
    /// buffer more than what is specified in `destsize`.
    ///
    /// If an error occurs, e.g. the compressed data is corrupted or the
    /// output buffer is not large enough, then 0 (zero) or a negative value
    /// will be returned instead.
    ///
    /// Environment variables
    /// ---------------------
    ///
    /// blosc_decompress() honors different environment variables to control
    /// internal parameters without the need of doing that programatically.
    /// Here are the ones supported:
    ///
    /// BLOSC_NTHREADS=(INTEGER): This will call
    /// blosc_set_nthreads(BLOSC_NTHREADS) before the proper decompression
    /// process starts.
    ///
    /// BLOSC_NOLOCK=(ANY VALUE): This will call blosc_decompress_ctx()
    /// under the hood, with the `numinternalthreads` parameter set to the
    /// same value as the last call to blosc_set_nthreads().
    pub fn blosc_decompress(src: *const c_void, dest: *mut c_void, destsize: size_t) -> c_int;

    /// Context interface to blosc decompression. This does not require a
    /// call to blosc_init() and can be called from multithreaded
    /// applications without the global lock being used, so allowing Blosc
    /// be executed simultaneously in those scenarios.
    ///
    /// It uses the same parameters than the blosc_decompress() function plus:
    ///
    /// `numinternalthreads`: number of threads to use internally.
    ///
    /// Decompression is memory safe and guaranteed not to write the `dest`
    /// buffer more than what is specified in `destsize`.
    ///
    /// If an error occurs, e.g. the compressed data is corrupted or the
    /// output buffer is not large enough, then 0 (zero) or a negative value
    /// will be returned instead.
    pub fn blosc_decompress_ctx(
        src: *const c_void,
        dest: *mut c_void,
        destsize: size_t,
        numinternalthreads: c_int,
    ) -> c_int;

    /// Get `nitems` (of typesize size) in `src` buffer starting in `start`.
    /// The items are returned in `dest` buffer, which has to have enough
    /// space for storing all items.
    ///
    /// Returns the number of bytes copied to `dest` or a negative value if
    /// some error happens.
    pub fn blosc_getitem(
        src: *const c_void,
        start: c_int,
        nitems: c_int,
        dest: *mut c_void,
    ) -> c_int;

    /// Returns the current number of threads that are used for
    /// compression/decompression.
    pub fn blosc_get_nthreads() -> c_int;

    /// Initialize a pool of threads for compression/decompression.  If
    /// `nthreads` is 1, then the serial version is chosen and a possible
    /// previous existing pool is ended.  If this is not called, `nthreads`
    /// is set to 1 internally.
    ///
    /// Returns the previous number of threads.
    pub fn blosc_set_nthreads(nthreads: c_int) -> c_int;

    /// Returns the current compressor that is used for compression.
    pub fn blosc_get_compressor() -> *const c_char;

    /// Select the compressor to be used.  The supported ones are "blosclz",
    /// "lz4", "lz4hc", "snappy", "zlib" and "ztsd".  If this function is not
    /// called, then "blosclz" will be used.
    ///
    /// In case the compressor is not recognized, or there is not support
    /// for it in this build, it returns a -1.  Else it returns the code for
    /// the compressor (>=0).
    pub fn blosc_set_compressor(compname: *const c_char) -> c_int;

    /// Get the `compname` associated with the `compcode`.
    ///
    /// If the compressor code is not recognized, or there is not support
    /// for it in this build, -1 is returned.  Else, the compressor code is
    /// returned.
    pub fn blosc_compcode_to_compname(compcode: c_int, compname: *mut *const c_char) -> c_int;

    /// Return the compressor code associated with the compressor name.
    ///
    /// If the compressor name is not recognized, or there is not support
    /// for it in this build, -1 is returned instead.
    pub fn blosc_compname_to_compcode(compname: *const c_char) -> c_int;

    /// Get a list of compressors supported in the current build.  The
    /// returned value is a string with a concatenation of "blosclz", "lz4",
    /// "lz4hc", "snappy", "zlib" or "zstd "separated by commas, depending
    /// on which ones are present in the build.
    ///
    /// This function does not leak, so you should not free() the returned
    /// list.
    ///
    /// This function should always succeed.
    pub fn blosc_list_compressors() -> *const c_char;

    /// Return the version of blosc in string format.
    ///
    /// Useful for dynamic libraries.
    pub fn blosc_get_version_string() -> *const c_char;

    /// Get info from compression libraries included in the current build.
    /// In `compname` you pass the compressor name that you want info from.
    /// In `complib` and `version` you get the compression library name and
    /// version (if available) as output.
    ///
    /// In `complib` and `version` you get a pointer to the compressor
    /// library name and the version in string format respectively.  After
    /// using the name and version, you should free() them so as to avoid
    /// leaks.
    ///
    /// If the compressor is supported, it returns the code for the library
    /// (>=0).  If it is not supported, this function returns -1.
    pub fn blosc_get_complib_info(
        compname: *const c_char,
        complib: *mut *mut c_char,
        version: *mut *mut c_char,
    ) -> c_int;

    /// Free possible memory temporaries and thread resources.  Use this
    /// when you are not going to use Blosc for a long while.  In case of
    /// problems releasing the resources, it returns a negative number, else
    /// it returns 0.
    pub fn blosc_free_resources() -> c_int;

    /// Return information about a compressed buffer, namely the number of
    /// uncompressed bytes (`nbytes`) and compressed (`cbytes`).  It also
    /// returns the `blocksize` (which is used internally for doing the
    /// compression by blocks).
    ///
    /// You only need to pass the first BLOSC_MIN_HEADER_LENGTH bytes of a
    /// compressed buffer for this call to work.
    ///
    /// If the format is not supported by the library, all output arguments will be
    /// filled with zeros.
    pub fn blosc_cbuffer_sizes(
        cbuffer: *const c_void,
        nbytes: *mut size_t,
        cbytes: *mut size_t,
        blocksize: *mut size_t,
    );

    /// Return information about a compressed buffer, namely the type size
    /// (`typesize`), as well as some internal `flags`.
    ///
    /// The `flags` is a set of bits, where the used ones are:
    /// bit 0: whether the shuffle filter has been applied or not
    /// bit 1: whether the internal buffer is a pure memcpy or not
    /// bit 2: whether the bit shuffle filter has been applied or not
    ///
    /// You can use the `BLOSC_DOSHUFFLE`, `BLOSC_DOBITSHUFFLE` and
    /// `BLOSC_MEMCPYED` symbols for extracting the interesting bits
    /// (e.g. ``flags & BLOSC_DOSHUFFLE`` says whether the buffer is
    /// byte-shuffled or not).
    ///
    /// If the format is not supported by the library, all output arguments will be
    /// filled with zeros.
    pub fn blosc_cbuffer_metainfo(cbuffer: *const c_void, typesize: *mut size_t, flags: *mut c_int);

    /// Return information about a compressed buffer, namely the internal
    /// Blosc format version (`version`) and the format for the internal
    /// Lempel-Ziv compressor used (`versionlz`).
    ///
    /// This function should always succeed.
    pub fn blosc_cbuffer_versions(
        cbuffer: *const c_void,
        version: *mut c_int,
        versionlz: *mut c_int,
    );

    /// Return the compressor library/format used in a compressed buffer.
    ///
    /// This function should always succeed.
    pub fn blosc_cbuffer_complib(cbuffer: *const c_void) -> *const c_char;

    /// Low-level functions follows.  Use them only if you are an expert!
    pub fn blosc_get_blocksize() -> c_int;

    /// Force the use of a specific blocksize.  If 0, an automatic
    /// blocksize will be used (the default).
    ///
    /// The blocksize is a critical parameter with important restrictions in
    /// the allowed values, so use this with care.
    pub fn blosc_set_blocksize(blocksize: size_t);

    /// Set the split mode.
    ///
    /// This function can take the next values:
    /// BLOSC_FORWARD_COMPAT_SPLIT
    /// BLOSC_AUTO_SPLIT
    /// BLOSC_NEVER_SPLIT
    /// BLOSC_ALWAYS_SPLIT
    ///
    /// FORWARD_COMPAT offers reasonably forward compatibility, AUTO is for nearly optimal results (based
    /// on heuristics), NEVER and ALWAYS are for the user experimenting when trying to get best
    /// compression ratios and/or speed.
    ///
    /// If not called, the default mode is BLOSC_FORWARD_COMPAT_SPLIT.
    ///
    /// This function should always succeed.
    pub fn blosc_set_splitmode(splitmode: c_int);
}



// #[cfg(test)]
// mod tests {
//     use blosc_get_nthreads;

//     #[test]
//     fn it_works() {
//         assert_eq!(2 + 2, 4);
//         unsafe {
//             let threads = blosc_get_nthreads();
//             println!("Hello, Blosc with n-threads: {}", threads);
//         }
//     }
// }
