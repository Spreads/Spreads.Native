extern crate libc;
extern crate spreads_mimalloc_sys as ffi;

use core::alloc::{GlobalAlloc, Layout};
use core::ffi::c_void;
use ffi::*;

// Copied from https://github.com/rust-lang/rust/blob/master/src/libstd/sys_common/alloc.rs
#[cfg(all(any(
    target_arch = "x86",
    target_arch = "arm",
    target_arch = "mips",
    target_arch = "powerpc",
    target_arch = "powerpc64",
    target_arch = "asmjs",
    target_arch = "wasm32"
)))]
const MIN_ALIGN: usize = 8;

#[cfg(all(any(
    target_arch = "x86_64",
    target_arch = "aarch64",
    target_arch = "mips64",
    target_arch = "s390x",
    target_arch = "sparc64"
)))]
const MIN_ALIGN: usize = 16;

pub struct SpreadsMalloc;

unsafe impl GlobalAlloc for SpreadsMalloc {
    #[inline]
    unsafe fn alloc(&self, layout: Layout) -> *mut u8 {
        if layout.align() <= MIN_ALIGN && layout.align() <= layout.size() {
            mi_malloc(layout.size()) as *mut u8
        } else {
            if cfg!(target_os = "macos") {
                if layout.align() > (1 << 31) {
                    return core::ptr::null_mut();
                }
            }

            mi_malloc_aligned(layout.size(), layout.align()) as *mut u8
        }
    }

    #[inline]
    unsafe fn alloc_zeroed(&self, layout: Layout) -> *mut u8 {
        if layout.align() <= MIN_ALIGN && layout.align() <= layout.size() {
            mi_zalloc(layout.size()) as *mut u8
        } else {
            if cfg!(target_os = "macos") {
                if layout.align() > (1 << 31) {
                    return core::ptr::null_mut();
                }
            }

            mi_zalloc_aligned(layout.size(), layout.align()) as *mut u8
        }
    }

    #[inline]
    unsafe fn dealloc(&self, ptr: *mut u8, _layout: Layout) {
        mi_free(ptr as *mut c_void);
    }

    #[inline]
    unsafe fn realloc(&self, ptr: *mut u8, layout: Layout, new_size: usize) -> *mut u8 {
        if layout.align() <= MIN_ALIGN && layout.align() <= layout.size() {
            mi_realloc(ptr as *mut c_void, new_size) as *mut u8
        } else {
            mi_realloc_aligned(ptr as *mut c_void, new_size, layout.align()) as *mut u8
        }
    }
}

/// Allocate zero-initialized count elements of size bytes.
/// # Parameters
/// count	number of elements.
/// size	size of each element.
/// # Returns
/// pointer to the allocated memory of size*count bytes, or NULL if either out of memory or when count*size overflows.
/// Returns a unique pointer if called with either size or count of 0.
#[no_mangle]
pub extern "C" fn spreads_mem_calloc(count: usize, size: usize) -> *mut libc::c_void {
    unsafe {
        return mi_calloc(count, size);
    }
}

/// Allocate size bytes.
/// # Parameters
/// size	number of bytes to allocate.
/// # Returns
/// pointer to the allocated memory or NULL if out of memory. Returns a unique pointer if called with size 0.
#[no_mangle]
pub extern "C" fn spreads_mem_malloc(size: usize) -> *mut libc::c_void {
    unsafe {
        return mi_malloc(size);
    }
}

/// Re-allocate memory to newsize bytes.
/// # Parameters
/// p	pointer to previously allocated memory (or NULL).
/// newsize	the new required size in bytes.
/// # Returns
/// pointer to the re-allocated memory of newsize bytes, or NULL if out of memory. If NULL is returned, the pointer p is not freed. Otherwise the original pointer is either freed or returned as the reallocated result (in case it fits in-place with the new size). If the pointer p is NULL, it behaves as mi_malloc(newsize). If newsize is larger than the original size allocated for p, the bytes after size are uninitialized.
#[no_mangle]
pub extern "C" fn spreads_mem_realloc(p: *mut libc::c_void, newsize: usize) -> *mut libc::c_void {
    unsafe {
        return mi_realloc(p, newsize);
    }
}

/// Try to re-allocate memory to newsize bytes in place.
/// # Parameters
/// p	pointer to previously allocated memory (or NULL).
/// newsize	the new required size in bytes.
/// # Returns
/// pointer to the re-allocated memory of newsize bytes (always equal to p), or NULL if either out of memory or if the memory could not be expanded in place. If NULL is returned, the pointer p is not freed. Otherwise the original pointer is returned as the reallocated result since it fits in-place with the new size. If newsize is larger than the original size allocated for p, the bytes after size are uninitialized.
#[no_mangle]
pub extern "C" fn spreads_mem_expand(p: *mut libc::c_void, newsize: usize) -> *mut libc::c_void {
    unsafe {
        return mi_expand(p, newsize);
    }
}

/// Free previously allocated memory.
/// The pointer p must have been allocated before (or be NULL).
/// # Parameters
/// p	pointer to free, or NULL.
#[no_mangle]
pub extern "C" fn spreads_mem_free(p: *mut libc::c_void) {
    unsafe {
        return mi_free(p);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_strdup(s: *const libc::c_char) -> *mut libc::c_char {
    unsafe {
        return mi_strdup(s);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_strndup(s: *const libc::c_char, n: usize) -> *mut libc::c_char {
    unsafe {
        return mi_strndup(s, n);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_realpath(
    fname: *const libc::c_char,
    resolved_name: *mut libc::c_char,
) -> *mut libc::c_char {
    unsafe {
        return mi_realpath(fname, resolved_name);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_malloc_small(size: usize) -> *mut libc::c_void {
    unsafe {
        return mi_malloc_small(size);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_zalloc_small(size: usize) -> *mut libc::c_void {
    unsafe {
        return mi_zalloc_small(size);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_zalloc(size: usize) -> *mut libc::c_void {
    unsafe {
        return mi_zalloc(size);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_mallocn(count: usize, size: usize) -> *mut libc::c_void {
    unsafe {
        return mi_mallocn(count, size);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_reallocn(
    p: *mut libc::c_void,
    count: usize,
    size: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_reallocn(p, count, size);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_reallocf(p: *mut libc::c_void, newsize: usize) -> *mut libc::c_void {
    unsafe {
        return mi_reallocf(p, newsize);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_usable_size(p: *const libc::c_void) -> usize {
    unsafe {
        return mi_usable_size(p);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_good_size(size: usize) -> usize {
    unsafe {
        return mi_good_size(size);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_register_deferred_free(
    deferred_free: mi_deferred_free_fun,
    arg: *mut libc::c_void,
) {
    unsafe {
        return mi_register_deferred_free(deferred_free, arg);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_register_output(out: mi_output_fun, arg: *mut libc::c_void) {
    unsafe {
        return mi_register_output(out, arg);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_register_error(fun: mi_error_fun, arg: *mut libc::c_void) {
    unsafe {
        return mi_register_error(fun, arg);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_collect(force: bool) {
    unsafe {
        return mi_collect(force);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_mialloc_version() -> libc::c_int {
    unsafe {
        return mi_version();
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_stats_reset() {
    unsafe {
        return mi_stats_reset();
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_stats_merge() {
    unsafe {
        return mi_stats_merge();
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_stats_print() {
    unsafe {
        return mi_stats_print(0 as *mut libc::c_void); // out is ignored
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_stats_print_out(out: mi_output_fun, arg: *mut libc::c_void) {
    unsafe {
        return mi_stats_print_out(out, arg);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_process_init() {
    unsafe {
        return mi_process_init();
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_thread_init() {
    unsafe {
        return mi_thread_init();
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_thread_done() {
    unsafe {
        return mi_thread_done();
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_thread_stats_print_out(out: mi_output_fun, arg: *mut libc::c_void) {
    unsafe {
        return mi_thread_stats_print_out(out, arg);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_malloc_aligned(size: usize, alignment: usize) -> *mut libc::c_void {
    unsafe {
        return mi_malloc_aligned(size, alignment);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_malloc_aligned_at(
    size: usize,
    alignment: usize,
    offset: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_malloc_aligned_at(size, alignment, offset);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_zalloc_aligned(size: usize, alignment: usize) -> *mut libc::c_void {
    unsafe {
        return mi_zalloc_aligned(size, alignment);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_zalloc_aligned_at(
    size: usize,
    alignment: usize,
    offset: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_zalloc_aligned_at(size, alignment, offset);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_calloc_aligned(
    count: usize,
    size: usize,
    alignment: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_calloc_aligned(count, size, alignment);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_calloc_aligned_at(
    count: usize,
    size: usize,
    alignment: usize,
    offset: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_calloc_aligned_at(count, size, alignment, offset);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_realloc_aligned(
    p: *mut libc::c_void,
    newsize: usize,
    alignment: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_realloc_aligned(p, newsize, alignment);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_realloc_aligned_at(
    p: *mut libc::c_void,
    newsize: usize,
    alignment: usize,
    offset: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_realloc_aligned_at(p, newsize, alignment, offset);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_rezalloc(p: *mut libc::c_void, newsize: usize) -> *mut libc::c_void {
    unsafe {
        return mi_rezalloc(p, newsize);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_recalloc(
    p: *mut libc::c_void,
    newcount: usize,
    size: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_recalloc(p, newcount, size);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_rezalloc_aligned(
    p: *mut libc::c_void,
    newsize: usize,
    alignment: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_rezalloc_aligned(p, newsize, alignment);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_rezalloc_aligned_at(
    p: *mut libc::c_void,
    newsize: usize,
    alignment: usize,
    offset: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_rezalloc_aligned_at(p, newsize, alignment, offset);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_recalloc_aligned(
    p: *mut libc::c_void,
    newcount: usize,
    size: usize,
    alignment: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_recalloc_aligned(p, newcount, size, alignment);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_recalloc_aligned_at(
    p: *mut libc::c_void,
    newcount: usize,
    size: usize,
    alignment: usize,
    offset: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_recalloc_aligned_at(p, newcount, size, alignment, offset);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_check_owned(p: *const libc::c_void) -> bool {
    unsafe {
        return mi_check_owned(p);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_reserve_huge_os_pages_interleave(
    pages: usize,
    numa_nodes: usize,
    timeout_msecs: usize,
) -> libc::c_int {
    unsafe {
        return mi_reserve_huge_os_pages_interleave(pages, numa_nodes, timeout_msecs);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_reserve_huge_os_pages_at(
    pages: usize,
    numa_node: libc::c_int,
    timeout_msecs: usize,
) -> libc::c_int {
    unsafe {
        return mi_reserve_huge_os_pages_at(pages, numa_node, timeout_msecs);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_reserve_huge_os_pages(
    pages: usize,
    max_secs: f64,
    pages_reserved: *mut usize,
) -> libc::c_int {
    unsafe {
        return mi_reserve_huge_os_pages(pages, max_secs, pages_reserved);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_option_is_enabled(option: mi_option_t) -> bool {
    unsafe {
        return mi_option_is_enabled(option);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_option_enable(option: mi_option_t) {
    unsafe {
        return mi_option_enable(option);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_option_disable(option: mi_option_t) {
    unsafe {
        return mi_option_disable(option);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_option_set_enabled(option: mi_option_t, enable: bool) {
    unsafe {
        return mi_option_set_enabled(option, enable);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_option_set_enabled_default(option: mi_option_t, enable: bool) {
    unsafe {
        return mi_option_set_enabled_default(option, enable);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_option_get(option: mi_option_t) -> libc::c_long {
    unsafe {
        return mi_option_get(option);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_option_set(option: mi_option_t, value: libc::c_long) {
    unsafe {
        return mi_option_set(option, value);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_option_set_default(option: mi_option_t, value: libc::c_long) {
    unsafe {
        return mi_option_set_default(option, value);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_heap_new() -> *mut mi_heap_t {
    unsafe {
        return mi_heap_new();
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_delete(heap: *mut mi_heap_t) {
    unsafe {
        return mi_heap_delete(heap);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_destroy(heap: *mut mi_heap_t) {
    unsafe {
        return mi_heap_destroy(heap);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_set_default(heap: *mut mi_heap_t) -> *mut mi_heap_t {
    unsafe {
        return mi_heap_set_default(heap);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_heap_get_default() -> *mut mi_heap_t {
    unsafe {
        return mi_heap_get_default();
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_heap_get_backing() -> *mut mi_heap_t {
    unsafe {
        return mi_heap_get_backing();
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_heap_visit_blocks(
    heap: *const mi_heap_t,
    visit_all_blocks: bool,
    visitor: mi_block_visit_fun,
    arg: *mut libc::c_void,
) -> bool {
    unsafe {
        return mi_heap_visit_blocks(heap, visit_all_blocks, visitor, arg);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_is_in_heap_region(p: *const libc::c_void) -> bool {
    unsafe {
        return mi_is_in_heap_region(p);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_heap_collect(heap: *mut mi_heap_t, force: bool) {
    unsafe {
        return mi_heap_collect(heap, force);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_malloc(heap: *mut mi_heap_t, size: usize) -> *mut libc::c_void {
    unsafe {
        return mi_heap_malloc(heap, size);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_zalloc(heap: *mut mi_heap_t, size: usize) -> *mut libc::c_void {
    unsafe {
        return mi_heap_zalloc(heap, size);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_calloc(
    heap: *mut mi_heap_t,
    count: usize,
    size: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_calloc(heap, count, size);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_mallocn(
    heap: *mut mi_heap_t,
    count: usize,
    size: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_mallocn(heap, count, size);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_malloc_small(
    heap: *mut mi_heap_t,
    size: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_malloc_small(heap, size);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_realloc(
    heap: *mut mi_heap_t,
    p: *mut libc::c_void,
    newsize: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_realloc(heap, p, newsize);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_reallocn(
    heap: *mut mi_heap_t,
    p: *mut libc::c_void,
    count: usize,
    size: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_reallocn(heap, p, count, size);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_reallocf(
    heap: *mut mi_heap_t,
    p: *mut libc::c_void,
    newsize: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_reallocf(heap, p, newsize);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_strdup(
    heap: *mut mi_heap_t,
    s: *const libc::c_char,
) -> *mut libc::c_char {
    unsafe {
        return mi_heap_strdup(heap, s);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_strndup(
    heap: *mut mi_heap_t,
    s: *const libc::c_char,
    n: usize,
) -> *mut libc::c_char {
    unsafe {
        return mi_heap_strndup(heap, s, n);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_realpath(
    heap: *mut mi_heap_t,
    fname: *const libc::c_char,
    resolved_name: *mut libc::c_char,
) -> *mut libc::c_char {
    unsafe {
        return mi_heap_realpath(heap, fname, resolved_name);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_malloc_aligned(
    heap: *mut mi_heap_t,
    size: usize,
    alignment: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_malloc_aligned(heap, size, alignment);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_malloc_aligned_at(
    heap: *mut mi_heap_t,
    size: usize,
    alignment: usize,
    offset: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_malloc_aligned_at(heap, size, alignment, offset);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_zalloc_aligned(
    heap: *mut mi_heap_t,
    size: usize,
    alignment: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_zalloc_aligned(heap, size, alignment);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_zalloc_aligned_at(
    heap: *mut mi_heap_t,
    size: usize,
    alignment: usize,
    offset: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_zalloc_aligned_at(heap, size, alignment, offset);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_calloc_aligned(
    heap: *mut mi_heap_t,
    count: usize,
    size: usize,
    alignment: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_calloc_aligned(heap, count, size, alignment);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_calloc_aligned_at(
    heap: *mut mi_heap_t,
    count: usize,
    size: usize,
    alignment: usize,
    offset: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_calloc_aligned_at(heap, count, size, alignment, offset);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_realloc_aligned(
    heap: *mut mi_heap_t,
    p: *mut libc::c_void,
    newsize: usize,
    alignment: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_realloc_aligned(heap, p, newsize, alignment);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_realloc_aligned_at(
    heap: *mut mi_heap_t,
    p: *mut libc::c_void,
    newsize: usize,
    alignment: usize,
    offset: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_realloc_aligned_at(heap, p, newsize, alignment, offset);
    }
}

#[no_mangle]
pub extern "C" fn spreads_mem_heap_rezalloc(
    heap: *mut mi_heap_t,
    p: *mut libc::c_void,
    newsize: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_rezalloc(heap, p, newsize);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_recalloc(
    heap: *mut mi_heap_t,
    p: *mut libc::c_void,
    newcount: usize,
    size: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_recalloc(heap, p, newcount, size);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_rezalloc_aligned(
    heap: *mut mi_heap_t,
    p: *mut libc::c_void,
    newsize: usize,
    alignment: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_rezalloc_aligned(heap, p, newsize, alignment);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_rezalloc_aligned_at(
    heap: *mut mi_heap_t,
    p: *mut libc::c_void,
    newsize: usize,
    alignment: usize,
    offset: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_rezalloc_aligned_at(heap, p, newsize, alignment, offset);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_recalloc_aligned(
    heap: *mut mi_heap_t,
    p: *mut libc::c_void,
    newcount: usize,
    size: usize,
    alignment: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_recalloc_aligned(heap, p, newcount, size, alignment);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_recalloc_aligned_at(
    heap: *mut mi_heap_t,
    p: *mut libc::c_void,
    newcount: usize,
    size: usize,
    alignment: usize,
    offset: usize,
) -> *mut libc::c_void {
    unsafe {
        return mi_heap_recalloc_aligned_at(heap, p, newcount, size, alignment, offset);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_contains_block(
    heap: *mut mi_heap_t,
    p: *const libc::c_void,
) -> bool {
    unsafe {
        return mi_heap_contains_block(heap, p);
    }
}
#[no_mangle]
pub extern "C" fn spreads_mem_heap_check_owned(
    heap: *mut mi_heap_t,
    p: *const libc::c_void,
) -> bool {
    unsafe {
        return mi_heap_check_owned(heap, p);
    }
}

#[cfg(test)]
mod tests {
    extern crate alloc;

    use super::*;

    #[test]
    fn it_frees_allocated_memory() {
        unsafe {
            let layout = Layout::from_size_align(8, 8).unwrap();
            let alloc = SpreadsMalloc;

            let ptr = alloc.alloc(layout.clone());
            alloc.dealloc(ptr, layout);
        }
    }

    #[test]
    fn it_frees_zero_allocated_memory() {
        unsafe {
            let layout = Layout::from_size_align(8, 8).unwrap();
            let alloc = SpreadsMalloc;

            let ptr = alloc.alloc_zeroed(layout.clone());
            alloc.dealloc(ptr, layout);
        }
    }

    #[test]
    fn it_frees_reallocated_memory() {
        unsafe {
            let layout = Layout::from_size_align(8, 8).unwrap();
            let alloc = SpreadsMalloc;

            let ptr = alloc.alloc(layout.clone());
            let ptr = alloc.realloc(ptr, layout.clone(), 16);
            alloc.dealloc(ptr, layout);
        }
    }

    #[test]
    fn it_could_call_mimalloc() {
        let x = spreads_mem_malloc(10);
        spreads_mem_free(x);
        spreads_mem_collect(false);
    }

    #[test]
    fn it_could_free_on_different_thread() {
        let x = spreads_mem_malloc(2 * 1024 * 1024 + 1) as i64;

        let t = std::thread::spawn(move || {
            spreads_mem_free(x as *mut libc::c_void);
        });
        t.join().expect("should join");
    }

    #[test]
    fn it_could_print_stats() {
        spreads_mem_stats_print();
    }

    // A list of C functions that are being imported
    extern "C" {
        pub fn printf(format: *const u8, ...) -> i32;
    }

    #[no_mangle]
    pub extern "C" fn test_print_out(msg: *const libc::c_char, _arg: *mut libc::c_void) {
        unsafe {
            printf(msg as *const u8);
        }
    }

    #[test]
    fn it_could_print_out_stats() {
        // spreads_mem_register_output(Option::Some(test_print_out), 0 as *mut libc::c_void);
        // spreads_mem_option_set(mi_option_e_mi_option_show_stats, 0);
        // spreads_mem_option_set_default(mi_option_e_mi_option_show_stats, 0);
        // spreads_mem_option_set_enabled_default(mi_option_e_mi_option_show_stats, false);
        spreads_mem_option_disable(mi_option_e_mi_option_show_stats);
        spreads_mem_option_disable(mi_option_e_mi_option_verbose);
        // spreads_mem_option_set_enabled(mi_option_e_mi_option_show_stats, false);
        spreads_mem_stats_print_out(Option::Some(test_print_out), 0 as *mut libc::c_void);
    }
}
