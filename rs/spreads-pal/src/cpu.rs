#[cfg(windows)]
#[no_mangle]
pub extern "C" fn spreads_pal_get_cpu_number() -> libc::c_int {
    unsafe {
        return winapi::um::processthreadsapi::GetCurrentProcessorNumber() as i32;
    }
}

#[cfg(any(linux, unix))]
#[no_mangle]
pub extern "C" fn spreads_pal_get_cpu_number() -> libc::c_int {
    unsafe {
        let cpu_id = libc::sched_getcpu();
        return if cpu_id < 0 { -1 } else { cpu_id };
    }
}

#[cfg(not(any(windows, linux, unix)))]
#[no_mangle]
pub extern "C" fn spreads_pal_get_cpu_number() -> libc::c_int {
    return -1;
}
