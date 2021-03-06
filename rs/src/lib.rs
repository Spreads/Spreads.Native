// #![no_std]

extern crate libc;
pub extern crate spreads_pal;

pub mod mem_allocation;
pub mod compression;

#[global_allocator]
static A: mem_allocation::SpreadsMalloc = mem_allocation::SpreadsMalloc;

#[no_mangle]
pub extern "C" fn hello_spreads() -> *const u8 {
    "Hello, Spreads.Native v.0.1.0!\0".as_ptr()
}
