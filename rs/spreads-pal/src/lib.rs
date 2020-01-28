pub mod cpu;

// #[no_mangle]
// pub extern "C" fn spreads_get_cpu_number() -> libc::c_int {
//     return cpu::get_cpu_number();
// }

#[cfg(test)]
mod tests {
    #[test]
    #[cfg(any(windows,linux, unix))]
    fn can_get_cpu_number_current() {
        // TODO xplat set affinity
        let result = super::cpu::spreads_get_cpu_number();
        println!("CPU number: {}", result);
        assert!(result >= 0);
    }

    #[test]
    #[cfg(any(linux, unix))]
    fn can_get_cpu_number_with_affinity() {
        let cpu_num = 3;
        unsafe {
            let mut cpu_set: libc::cpu_set_t = std::mem::zeroed();
            libc::CPU_ZERO(&mut cpu_set);
            libc::CPU_SET(cpu_num, &mut cpu_set);
            let ret = libc::sched_setaffinity(0, 1, &cpu_set as *const libc::cpu_set_t);
            assert_eq!(
                ret,
                0,
                "sched_setaffinity is expected to return 0, was {}: {:?}",
                ret,
                std::io::Error::last_os_error()
            );
        }
        let result = super::cpu::spreads_get_cpu_number();
        println!("CPU number: {}", result);
        assert_eq!(cpu_num as i32, result);
    }
}
