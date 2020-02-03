#include "./c-blosc/blosc/blosc-export.h"
// #include <stdlib.h>

/**
  Initialize the Blosc library environment.

  You must call this previous to any other Blosc call, unless you want
  Blosc to be used simultaneously in a multi-threaded environment, in
  which case you should *exclusively* use the
  blosc_compress_ctx()/blosc_decompress_ctx() pair (see below).
  */
BLOSC_EXPORT void blosc_init(void);

/**
  Destroy the Blosc library environment.

  You must call this after to you are done with all the Blosc calls,
  unless you have not used blosc_init() before (see blosc_init()
  above).
  */
BLOSC_EXPORT void blosc_destroy(void);

/**
  Compress a block of data in the `src` buffer and returns the size of
  the compressed block.  The size of `src` buffer is specified by
  `nbytes`.  There is not a minimum for `src` buffer size (`nbytes`).

  `clevel` is the desired compression level and must be a number
  between 0 (no compression) and 9 (maximum compression).

  `doshuffle` specifies whether the shuffle compression filters
  should be applied or not.  BLOSC_NOSHUFFLE means not applying it,
  BLOSC_SHUFFLE means applying it at a byte level and BLOSC_BITSHUFFLE
  at a bit level (slower but may achieve better entropy alignment).

  `typesize` is the number of bytes for the atomic type in binary
  `src` buffer.  This is mainly useful for the shuffle filters.
  For implementation reasons, only a 1 < `typesize` < 256 will allow the
  shuffle filter to work.  When `typesize` is not in this range, shuffle
  will be silently disabled.

  The `dest` buffer must have at least the size of `destsize`.  Blosc
  guarantees that if you set `destsize` to, at least,
  (`nbytes` + BLOSC_MAX_OVERHEAD), the compression will always succeed.
  The `src` buffer and the `dest` buffer can not overlap.

  Compression is memory safe and guaranteed not to write the `dest`
  buffer beyond what is specified in `destsize`.

  If `src` buffer cannot be compressed into `destsize`, the return
  value is zero and you should discard the contents of the `dest`
  buffer.

  A negative return value means that an internal error happened.  This
  should never happen.  If you see this, please report it back
  together with the buffer data causing this and compression settings.

  Environment variables
  ---------------------

  blosc_compress() honors different environment variables to control
  internal parameters without the need of doing that programatically.
  Here are the ones supported:

  BLOSC_CLEVEL=(INTEGER): This will overwrite the `clevel` parameter
  before the compression process starts.

  BLOSC_SHUFFLE=[NOSHUFFLE | SHUFFLE | BITSHUFFLE]: This will
  overwrite the `doshuffle` parameter before the compression process
  starts.

  BLOSC_TYPESIZE=(INTEGER): This will overwrite the `typesize`
  parameter before the compression process starts.

  BLOSC_COMPRESSOR=[BLOSCLZ | LZ4 | LZ4HC | SNAPPY | ZLIB]: This will
  call blosc_set_compressor(BLOSC_COMPRESSOR) before the compression
  process starts.

  BLOSC_NTHREADS=(INTEGER): This will call
  blosc_set_nthreads(BLOSC_NTHREADS) before the compression process
  starts.

  BLOSC_BLOCKSIZE=(INTEGER): This will call
  blosc_set_blocksize(BLOSC_BLOCKSIZE) before the compression process
  starts.  *NOTE:* The blocksize is a critical parameter with
  important restrictions in the allowed values, so use this with care.

  BLOSC_NOLOCK=(ANY VALUE): This will call blosc_compress_ctx() under
  the hood, with the `compressor`, `blocksize` and
  `numinternalthreads` parameters set to the same as the last calls to
  blosc_set_compressor(), blosc_set_blocksize() and
  blosc_set_nthreads().  BLOSC_CLEVEL, BLOSC_SHUFFLE, BLOSC_TYPESIZE
  environment vars will also be honored.

  BLOSC_SPLITMODE=[ FORWARD_COMPAT | AUTO | ALWAYS | NEVER ]:
  This will call blosc_set_splitmode() with the different supported values.
  See blosc_set_splitmode() docstrings for more info on each mode.

  */
BLOSC_EXPORT int blosc_compress(int clevel, int doshuffle, size_t typesize,
                                size_t nbytes, const void *src, void *dest,
                                size_t destsize);

/**
  Context interface to blosc compression. This does not require a call
  to blosc_init() and can be called from multithreaded applications
  without the global lock being used, so allowing Blosc be executed
  simultaneously in those scenarios.

  It uses the same parameters than the blosc_compress() function plus:

  `compressor`: the string representing the type of compressor to use.

  `blocksize`: the requested size of the compressed blocks.  If 0, an
   automatic blocksize will be used.

  `numinternalthreads`: the number of threads to use internally.

  A negative return value means that an internal error happened.  This
  should never happen.  If you see this, please report it back
  together with the buffer data causing this and compression settings.
*/
BLOSC_EXPORT int blosc_compress_ctx(int clevel, int doshuffle, size_t typesize,
                                    size_t nbytes, const void *src, void *dest,
                                    size_t destsize, const char *compressor,
                                    size_t blocksize, int numinternalthreads);

/**
  Decompress a block of compressed data in `src`, put the result in
  `dest` and returns the size of the decompressed block.

  The `src` buffer and the `dest` buffer can not overlap.

  Decompression is memory safe and guaranteed not to write the `dest`
  buffer beyond what is specified in `destsize`.

  If an error occurs, e.g. the compressed data is corrupted or the
  output buffer is not large enough, then 0 (zero) or a negative value
  will be returned instead.

  Environment variables
  ---------------------

  blosc_decompress() honors different environment variables to control
  internal parameters without the need of doing that programatically.
  Here are the ones supported:

  BLOSC_NTHREADS=(INTEGER): This will call
  blosc_set_nthreads(BLOSC_NTHREADS) before the proper decompression
  process starts.

  BLOSC_NOLOCK=(ANY VALUE): This will call blosc_decompress_ctx()
  under the hood, with the `numinternalthreads` parameter set to the
  same value as the last call to blosc_set_nthreads().
*/
BLOSC_EXPORT int blosc_decompress(const void *src, void *dest, size_t destsize);

/**
  Context interface to blosc decompression. This does not require a
  call to blosc_init() and can be called from multithreaded
  applications without the global lock being used, so allowing Blosc
  be executed simultaneously in those scenarios.

  It uses the same parameters than the blosc_decompress() function plus:

  `numinternalthreads`: number of threads to use internally.

  Decompression is memory safe and guaranteed not to write the `dest`
  buffer more than what is specified in `destsize`.

  If an error occurs, e.g. the compressed data is corrupted or the
  output buffer is not large enough, then 0 (zero) or a negative value
  will be returned instead.
*/
BLOSC_EXPORT int blosc_decompress_ctx(const void *src, void *dest,
                                      size_t destsize, int numinternalthreads);

/**
  Get `nitems` (of typesize size) in `src` buffer starting in `start`.
  The items are returned in `dest` buffer, which has to have enough
  space for storing all items.

  Returns the number of bytes copied to `dest` or a negative value if
  some error happens.
  */
BLOSC_EXPORT int blosc_getitem(const void *src, int start, int nitems, void *dest);

/**
  Returns the current number of threads that are used for
  compression/decompression.
  */
BLOSC_EXPORT int blosc_get_nthreads(void);

/**
  Initialize a pool of threads for compression/decompression.  If
  `nthreads` is 1, then the serial version is chosen and a possible
  previous existing pool is ended.  If this is not called, `nthreads`
  is set to 1 internally.

  Returns the previous number of threads.
  */
BLOSC_EXPORT int blosc_set_nthreads(int nthreads);

/**
  Returns the current compressor that is being used for compression.
  */
BLOSC_EXPORT const char *blosc_get_compressor(void);

/**
  Select the compressor to be used.  The supported ones are "blosclz",
  "lz4", "lz4hc", "snappy", "zlib" and "ztsd".  If this function is not
  called, then "blosclz" will be used by default.

  In case the compressor is not recognized, or there is not support
  for it in this build, it returns a -1.  Else it returns the code for
  the compressor (>=0).
  */
BLOSC_EXPORT int blosc_set_compressor(const char *compname);

/**
  Get the `compname` associated with the `compcode`.

  If the compressor code is not recognized, or there is not support
  for it in this build, -1 is returned.  Else, the compressor code is
  returned.
 */
BLOSC_EXPORT int blosc_compcode_to_compname(int compcode, const char **compname);

/**
  Return the compressor code associated with the compressor name.

  If the compressor name is not recognized, or there is not support
  for it in this build, -1 is returned instead.
 */
BLOSC_EXPORT int blosc_compname_to_compcode(const char *compname);

/**
  Get a list of compressors supported in the current build.  The
  returned value is a string with a concatenation of "blosclz", "lz4",
  "lz4hc", "snappy", "zlib" or "zstd "separated by commas, depending
  on which ones are present in the build.

  This function does not leak, so you should not free() the returned
  list.

  This function should always succeed.
  */
BLOSC_EXPORT const char *blosc_list_compressors(void);

/**
  Return the version of the C-Blosc library in string format.

  Useful for dynamic libraries.
*/
BLOSC_EXPORT const char *blosc_get_version_string(void);

/**
  Get info from compression libraries included in the current build.
  In `compname` you pass the compressor name that you want info from.

  In `complib` and `version` you get a pointer to the compressor
  library name and the version in string format respectively.  After
  using the name and version, you should free() them so as to avoid
  leaks.  If any of `complib` and `version` are NULL, they will not be
  assigned to anything, and the user should not need to free them.

  If the compressor is supported, it returns the code for the library
  (>=0).  If it is not supported, this function returns -1.
  */
BLOSC_EXPORT int blosc_get_complib_info(const char *compname, char **complib, char **version);

/**
  Free possible memory temporaries and thread resources.  Use this
  when you are not going to use Blosc for a long while.  In case of
  problems releasing the resources, it returns a negative number, else
  it returns 0.
  */
BLOSC_EXPORT int blosc_free_resources(void);

/**
  Return information about a compressed buffer, namely the number of
  uncompressed bytes (`nbytes`) and compressed (`cbytes`).  It also
  returns the `blocksize` (which is used internally for doing the
  compression by blocks).

  You only need to pass the first BLOSC_MIN_HEADER_LENGTH bytes of a
  compressed buffer for this call to work.

  If the format is not supported by the library, all output arguments will be
  filled with zeros.
  */
BLOSC_EXPORT void blosc_cbuffer_sizes(const void *cbuffer, size_t *nbytes,
                                      size_t *cbytes, size_t *blocksize);

/**
  Return meta-information about a compressed buffer, namely the type size
  (`typesize`), as well as some internal `flags`.

  The `flags` is a set of bits, where the used ones are:
    * bit 0: whether the shuffle filter has been applied or not
    * bit 1: whether the internal buffer is a pure memcpy or not
    * bit 2: whether the bit shuffle filter has been applied or not

  You can use the `BLOSC_DOSHUFFLE`, `BLOSC_DOBITSHUFFLE` and
  `BLOSC_MEMCPYED` symbols for extracting the interesting bits
  (e.g. ``flags & BLOSC_DOSHUFFLE`` says whether the buffer is
  byte-shuffled or not).

  You only need to pass the first BLOSC_MIN_HEADER_LENGTH bytes of a
  compressed buffer for this call to work.

  If the format is not supported by the library, all output arguments will be
  filled with zeros.
  */
BLOSC_EXPORT void blosc_cbuffer_metainfo(const void *cbuffer, size_t *typesize,
                                         int *flags);

/**
  Return information about a compressed buffer, namely the internal
  Blosc format version (`version`) and the format for the internal
  compressor used (`compversion`).

  This function should always succeed.
  */
BLOSC_EXPORT void blosc_cbuffer_versions(const void *cbuffer, int *version,
                                         int *compversion);

/**
  Return the compressor library/format used in a compressed buffer.

  This function should always succeed.
  */
BLOSC_EXPORT const char *blosc_cbuffer_complib(const void *cbuffer);

/*********************************************************************

  Low-level functions follows.  Use them only if you are an expert!

*********************************************************************/

/**
  Get the internal blocksize to be used during compression.  0 means
  that an automatic blocksize is computed internally (the default).
  */
BLOSC_EXPORT int blosc_get_blocksize(void);

/**
  Force the use of a specific blocksize.  If 0, an automatic
  blocksize will be used (the default).

  The blocksize is a critical parameter with important restrictions in
  the allowed values, so use this with care.
  */
BLOSC_EXPORT void blosc_set_blocksize(size_t blocksize);

/**
  Set the split mode.

  This function can take the next values:
  *  BLOSC_FORWARD_COMPAT_SPLIT
  *  BLOSC_AUTO_SPLIT
  *  BLOSC_NEVER_SPLIT
  *  BLOSC_ALWAYS_SPLIT

  BLOSC_FORWARD_COMPAT offers reasonably forward compatibility,
  BLOSC_AUTO_SPLIT is for nearly optimal results (based on heuristics),
  BLOSC_NEVER_SPLIT and BLOSC_ALWAYS_SPLIT are for the user experimenting
  when trying to get best compression ratios and/or speed.

  If not called, the default mode is BLOSC_FORWARD_COMPAT_SPLIT.

  This function should always succeed.
 */
BLOSC_EXPORT void blosc_set_splitmode(int splitmode);

BLOSC_EXPORT int compress_lz4(const char *input, size_t input_length,
                              char *output, size_t maxout, int clevel);

BLOSC_EXPORT int decompress_lz4(const char *input, size_t compressed_length,
                                char *output, size_t maxout);

BLOSC_EXPORT int compress_zstd(const char *input, size_t input_length,
                               char *output, size_t maxout, int clevel);

BLOSC_EXPORT int decompress_zstd(const char *input, size_t compressed_length,
                                 char *output, size_t maxout);

BLOSC_EXPORT int compress_zlib(const char *input, size_t input_length,
                               char *output, size_t maxout, int clevel);

BLOSC_EXPORT int decompress_zlib(const char *input, size_t compressed_length,
                                 char *output, size_t maxout);

BLOSC_EXPORT int compress_deflate(const char *input, size_t input_length,
                                  char *output, size_t maxout, int clevel);

BLOSC_EXPORT int decompress_deflate(const char *input, size_t compressed_length,
                                    char *output, size_t maxout);

BLOSC_EXPORT int compress_gzip(const char *input, size_t input_length,
                               char *output, size_t maxout, int clevel);

BLOSC_EXPORT int decompress_gzip(const char *input, size_t compressed_length,
                                 char *output, size_t maxout);

BLOSC_EXPORT int compress_noop(const char *input, size_t input_length,
                               char *output, size_t maxout, int clevel);

BLOSC_EXPORT int decompress_noop(const char *input, size_t compressed_length,
                                 char *output, size_t maxout);

/**
  Primary shuffle and bitshuffle routines.
  This function dynamically dispatches to the appropriate hardware-accelerated
  routine based on the host processor's architecture. If the host processor is
  not supported by any of the hardware-accelerated routines, the generic
  (non-accelerated) implementation is used instead.
  Consumers should almost always prefer to call this routine instead of directly
  calling the hardware-accelerated routines because this method is both cross-
  platform and future-proof.
*/
BLOSC_EXPORT void
blosc_internal_shuffle(const size_t bytesoftype, const size_t blocksize,
        const char *_src, const char *_dest);

BLOSC_EXPORT int
blosc_internal_bitshuffle(const size_t bytesoftype, const size_t blocksize,
           const char *const _src, const char *_dest,
           const char *_tmp);

/**
  Primary unshuffle and bitunshuffle routine.
  This function dynamically dispatches to the appropriate hardware-accelerated
  routine based on the host processor's architecture. If the host processor is
  not supported by any of the hardware-accelerated routines, the generic
  (non-accelerated) implementation is used instead.
  Consumers should almost always prefer to call this routine instead of directly
  calling the hardware-accelerated routines because this method is both cross-
  platform and future-proof.
*/
BLOSC_EXPORT void
blosc_internal_unshuffle(const size_t bytesoftype, const size_t blocksize,
          const char *_src, const char *_dest);

BLOSC_EXPORT int
blosc_internal_bitunshuffle(const size_t bytesoftype, const size_t blocksize,
             const char *const _src, const char *_dest,
             const char *_tmp);