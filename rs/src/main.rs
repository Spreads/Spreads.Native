extern crate spreads_blosc_sys;

use spreads_blosc_sys::{blosc_get_nthreads, blosc_set_nthreads};

pub fn main() {
    unsafe {
        blosc_set_nthreads(8);
        let threads = blosc_get_nthreads();
        println!("Hello, Blosc with N-threads: {}", threads);
    }
}
