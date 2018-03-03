extern crate cmake;

fn main() {
    // TODO deduplicate code, only MinGW line for Windows, check if on MSVC
    if cfg!(windows) && cfg!(target_env = "gnu") {
        cmake::Config::new("c-blosc")
            .generator("MinGW Makefiles")
            .define("BUILD_STATIC", "ON")
            .define("BUILD_SHARED", "ON")
            .define("BUILD_TESTS", "OFF")
            .define("BUILD_BENCHMARKS", "OFF")
            .define("DEACTIVATE_AVX2", "OFF")
            .define("DEACTIVATE_LZ4", "OFF")
            .define("DEACTIVATE_SNAPPY", "ON")
            .define("DEACTIVATE_ZLIB", "ON")
            .define("DEACTIVATE_ZSTD", "OFF")
            .define("PREFER_EXTERNAL_LZ4", "OFF")
            .define("PREFER_EXTERNAL_SNAPPY", "OFF")
            .define("PREFER_EXTERNAL_ZLIB", "OFF")
            .define("PREFER_EXTERNAL_ZSTD", "OFF")
            .define("CMAKE_BUILD_TYPE", "Release")
            .build();
    } else {
        cmake::Config::new("c-blosc")
            .define("BUILD_STATIC", "ON")
            .define("BUILD_SHARED", "ON")
            .define("BUILD_TESTS", "OFF")
            .define("BUILD_BENCHMARKS", "OFF")
            .define("DEACTIVATE_AVX2", "OFF")
            .define("DEACTIVATE_LZ4", "OFF")
            .define("DEACTIVATE_SNAPPY", "ON")
            .define("DEACTIVATE_ZLIB", "ON")
            .define("DEACTIVATE_ZSTD", "OFF")
            .define("PREFER_EXTERNAL_LZ4", "OFF")
            .define("PREFER_EXTERNAL_SNAPPY", "OFF")
            .define("PREFER_EXTERNAL_ZLIB", "OFF")
            .define("PREFER_EXTERNAL_ZSTD", "OFF")
            .define("CMAKE_BUILD_TYPE", "Release")
            .build();
    }
}
