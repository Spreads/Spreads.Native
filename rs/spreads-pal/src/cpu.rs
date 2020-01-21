#[cfg(windows)]
pub fn get_cpu_number() -> libc::c_int {
    unsafe {
        return winapi::um::processthreadsapi::GetCurrentProcessorNumber() as i32;
    }
}

#[cfg(any(linux, unix))]
pub fn get_cpu_number() -> libc::c_int {
    unsafe {
        return libc::sched_getcpu();
    }
}

#[cfg(not(any(windows, linux, unix)))]
pub fn get_cpu_number() -> libc::c_int {
    return -1;
}
