#![no_std]

extern crate libc;
extern crate spreads_blosc_sys;

#[no_mangle]
pub extern "C" fn hello_spreads() -> *const u8 {
    "Hello, Spreads.Native v.0.1.0!\0".as_ptr()
}

#[no_mangle]
pub extern "C" fn spreads_compress_lz4(
    input: *const libc::c_char,
    input_length: usize,
    output: *mut libc::c_char,
    maxout: usize,
    clevel: libc::c_int,
) -> libc::c_int {
    return unsafe { spreads_blosc_sys::compress_lz4(input, input_length, output, maxout, clevel) };
}

#[no_mangle]
pub extern "C" fn spreads_decompress_lz4(
    input: *const libc::c_char,
    input_length: usize,
    output: *mut libc::c_char,
    maxout: usize,
) -> libc::c_int {
    return unsafe { spreads_blosc_sys::decompress_lz4(input, input_length, output, maxout) };
}

#[no_mangle]
pub extern "C" fn spreads_compress_zstd(
    input: *const libc::c_char,
    input_length: usize,
    output: *mut libc::c_char,
    maxout: usize,
    clevel: libc::c_int,
) -> libc::c_int {
    return unsafe {
        spreads_blosc_sys::compress_zstd(input, input_length, output, maxout, clevel)
    };
}

#[no_mangle]
pub extern "C" fn spreads_decompress_zstd(
    input: *const libc::c_char,
    input_length: usize,
    output: *mut libc::c_char,
    maxout: usize,
) -> libc::c_int {
    return unsafe { spreads_blosc_sys::decompress_zstd(input, input_length, output, maxout) };
}

#[no_mangle]
pub extern "C" fn spreads_compress_zlib(
    input: *const libc::c_char,
    input_length: usize,
    output: *mut libc::c_char,
    maxout: usize,
    clevel: libc::c_int,
) -> libc::c_int {
    return unsafe {
        spreads_blosc_sys::compress_zlib(input, input_length, output, maxout, clevel)
    };
}

#[no_mangle]
pub extern "C" fn spreads_decompress_zlib(
    input: *const libc::c_char,
    input_length: usize,
    output: *mut libc::c_char,
    maxout: usize,
) -> libc::c_int {
    return unsafe { spreads_blosc_sys::decompress_zlib(input, input_length, output, maxout) };
}

#[no_mangle]
pub extern "C" fn spreads_compress_deflate(
    input: *const libc::c_char,
    input_length: usize,
    output: *mut libc::c_char,
    maxout: usize,
    clevel: libc::c_int,
) -> libc::c_int {
    return unsafe {
        spreads_blosc_sys::compress_deflate(input, input_length, output, maxout, clevel)
    };
}

#[no_mangle]
pub extern "C" fn spreads_decompress_deflate(
    input: *const libc::c_char,
    input_length: usize,
    output: *mut libc::c_char,
    maxout: usize,
) -> libc::c_int {
    return unsafe { spreads_blosc_sys::decompress_deflate(input, input_length, output, maxout) };
}

#[no_mangle]
pub extern "C" fn spreads_compress_gzip(
    input: *const libc::c_char,
    input_length: usize,
    output: *mut libc::c_char,
    maxout: usize,
    clevel: libc::c_int,
) -> libc::c_int {
    return unsafe {
        spreads_blosc_sys::compress_gzip(input, input_length, output, maxout, clevel)
    };
}

#[no_mangle]
pub extern "C" fn spreads_decompress_gzip(
    input: *const libc::c_char,
    input_length: usize,
    output: *mut libc::c_char,
    maxout: usize,
) -> libc::c_int {
    return unsafe { spreads_blosc_sys::decompress_gzip(input, input_length, output, maxout) };
}

#[no_mangle]
pub extern "C" fn spreads_shuffle(
    bytesoftype: usize,
    blocksize: usize,
    src: *const libc::c_char,
    dest: *const libc::c_char,
) {
    return unsafe { spreads_blosc_sys::blosc_internal_shuffle(bytesoftype, blocksize, src, dest) };
}

#[no_mangle]
pub extern "C" fn spreads_unshuffle(
    bytesoftype: usize,
    blocksize: usize,
    src: *const libc::c_char,
    dest: *const libc::c_char,
) {
    return unsafe { spreads_blosc_sys::blosc_internal_unshuffle(bytesoftype, blocksize, src, dest) };
}

#[test]
pub fn could_set_threads() {
    unsafe {
        blosc_set_nthreads(8);
        let threads = blosc_get_nthreads();
        assert_eq!(threads, 8);
        println!("Hello, Blosc with N-threads: {}", threads);
    }
}
