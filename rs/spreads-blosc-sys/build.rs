extern crate bindgen;
extern crate cmake;

// use std::env;
// use std::path::PathBuf;

fn main() {
    // let bindings = bindgen::Builder::default()
    //     .header("blosc_wrapper.h")
    //     .use_core()
    //     .ctypes_prefix("libc")
    //     .whitelist_function(".*compress.*")
    //     .whitelist_function(".*shuffle.*")
    //     .whitelist_function(".*nthreads.*")
    //     .rustfmt_bindings(true)
    //     .generate()
    //     .expect("Unable to generate bindings");

    // // Write the bindings to the $OUT_DIR/bindings.rs file.
    // let out_path = PathBuf::from(env::var("CARGO_MANIFEST_DIR").unwrap());
    // bindings
    //     .write_to_file(out_path.join("src/lib.rs"))
    //     .expect("Couldn't write bindings!");

    let mut config = cmake::Config::new("c-blosc");

    let mut final_config = 
        config.generator("Ninja")
        .define("CMAKE_C_COMPILER", "clang")
        .define("CMAKE_CXX_COMPILER", "clang")
        .define("CMAKE_C_COMPILER_ID", "Clang")
        .define("CMAKE_CXX_COMPILER_ID", "Clang")
        .define("CMAKE_C_FLAGS", "-v -flto=thin -fuse-ld=lld -O3")
        .define("CMAKE_CXX_FLAGS", " -v -flto=thin -fuse-ld=lld -O3")
        // Blosc defines below
        .define("BUILD_STATIC", "ON")
        .define("BUILD_SHARED", "OFF")
        .define("BUILD_TESTS", "OFF")
        .define("BUILD_BENCHMARKS", "OFF")
        .define("DEACTIVATE_AVX2", "OFF")
        .define("DEACTIVATE_LZ4", "OFF")
        .define("DEACTIVATE_SNAPPY", "ON")
        .define("DEACTIVATE_ZLIB", "OFF")
        .define("DEACTIVATE_ZSTD", "OFF")
        .define("PREFER_EXTERNAL_LZ4", "OFF")
        .define("PREFER_EXTERNAL_SNAPPY", "OFF")
        .define("PREFER_EXTERNAL_ZLIB", "OFF")
        .define("PREFER_EXTERNAL_ZSTD", "OFF");

    if cfg!(windows) {
        final_config = final_config
            .define("CMAKE_SYSTEM_NAME", "Generic")
            .define("CMAKE_SYSTEM_PROCESSOR", "AMD64")
            .define("WIN32", "true")
            .static_crt(true);       
    }

    let dst = final_config.build();

    let dir = format!("{}/lib", dst.display());

    let mut libname = "blosc";

    if cfg!(windows) {
        let src = format!("{}/libblosc.a", dir);
        let dest = format!("{}/libblosc.lib", dir);
        let msg = format!("Should copy file: {} to {}", src, dest);
        std::fs::rename(src, dest).expect(&msg);
        libname = "libblosc";
    }
    
    println!("cargo:rustc-link-search=native={}", dir);
    println!("cargo:rustc-link-lib={}", libname);
}
