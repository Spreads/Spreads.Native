#[cfg(target_os = "windows")]
#[no_mangle]
pub extern "C" fn spreads_pal_get_cpu_number() -> libc::c_int {
    unsafe {
        let mut processor_number =  core::mem::MaybeUninit::<winapi::um::winnt::PROCESSOR_NUMBER>::uninit();
        winapi::um::processthreadsapi::GetCurrentProcessorNumberEx(processor_number.as_mut_ptr());
        return (processor_number.assume_init().Group as i32) << 6 | (processor_number.assume_init().Number as i32);
    }
}

#[cfg(target_os = "linux")]
#[no_mangle]
pub extern "C" fn spreads_pal_get_cpu_number() -> libc::c_int {
    unsafe {
        let cpu_id = libc::sched_getcpu();
        return if cpu_id < 0 { -1 } else { cpu_id };
    }
}

#[cfg(not(any(target_os = "windows", target_os = "linux")))]
#[no_mangle]
pub extern "C" fn spreads_pal_get_cpu_number() -> libc::c_int {
    return -1;
}
