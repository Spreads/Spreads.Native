// extern crate bindgen;
extern crate cmake;

fn main() {
    // let bindings = bindgen::Builder::default()
    //     .header("mimalloc_wrapper.h")
    //     .use_core()
    //     .ctypes_prefix("libc")
    //     .whitelist_function("mi_.*")
    //     .rustfmt_bindings(true)
    //     .generate()
    //     .expect("Unable to generate bindings");

    // // Write the bindings to the $OUT_DIR/bindings.rs file.
    // let out_path = std::path::PathBuf::from(std::env::var("CARGO_MANIFEST_DIR").unwrap());
    // bindings
    //     .write_to_file(out_path.join("src/mimalloc.rs"))
    //     .expect("Couldn't write bindings!");

    // https://microsoft.github.io/mimalloc/environment.html
    // https://github.com/purpleprotocol/mimalloc_rust/blob/master/libmimalloc-sys/build.rs
    let mut config = cmake::Config::new("mimalloc");

    let mut cfg = config
        .define("MI_OVERRIDE", "OFF")
        .define("MI_SECURE", "OFF")
        .define("MI_SECURE_FULL", "OFF")
        .define("MI_BUILD_TESTS", "OFF");

    if cfg!(feature = "secure") {
        cfg = cfg.define("MI_SECURE", "ON");
    }

    if cfg!(feature = "secure-full") {
        cfg = cfg.define("MI_SECURE_FULL", "ON");
    }

    // Inject MI_DEBUG=0
    // This set mi_option_verbose and mi_option_show_errors options to false.
    cfg = cfg.define("mi_defines", "MI_DEBUG=0");

    if cfg!(all(windows, target_env = "msvc")) {
        cfg = cfg.define("CMAKE_SH", "CMAKE_SH-NOTFOUND");

        // cc::get_compiler have /nologo /MD default flags that are cmake::Config
        // defaults to. Those flags prevents mimalloc from building on windows
        // extracted from default cmake configuration on windows
        if cfg!(debug_assertions) {
            // CMAKE_C_FLAGS + CMAKE_C_FLAGS_DEBUG
            cfg = cfg.cflag("/DWIN32 /D_WINDOWS /W3 /MDd /Zi /Ob0 /Od /RTC1");
        } else {
            // CMAKE_C_FLAGS + CMAKE_C_FLAGS_RELEASE
            cfg = cfg.cflag("/DWIN32 /D_WINDOWS /W3 /MD /O2 /Ob2 /DNDEBUG");
        }
    }

    let (out_dir, out_name) = if cfg!(all(windows, target_env = "msvc")) {
        if cfg!(debug_assertions) {
            if cfg!(feature = "secure") {
                ("./build/Debug", "mimalloc-static-secure-debug")
            } else {
                ("./build/Debug", "mimalloc-static-debug")
            }
        } else {
            // TODO this breaks when debug=true, the folder is RelWithDebugInfo
            if cfg!(feature = "secure") {
                ("./build/Release", "mimalloc-static-secure")
            } else {
                ("./build/Release", "mimalloc-static")
            }
        }
    } else {
        if cfg!(debug_assertions) {
            if cfg!(feature = "secure") {
                ("./build", "mimalloc-secure-debug")
            } else {
                ("./build", "mimalloc-debug")
            }
        } else {
            if cfg!(feature = "secure") {
                ("./build", "mimalloc-secure")
            } else {
                ("./build", "mimalloc")
            }
        }
    };

    // Build mimalloc-static
    let mut dst = cfg.build_target("mimalloc-static").build();
    dst.push(out_dir);

    println!("cargo:rustc-link-search=native={}", dst.display());
    println!("cargo:rustc-link-lib={}", out_name);
}
