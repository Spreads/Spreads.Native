# Spreads.Native/Unsafe

**Spreads' native dependencies and low-level IL methods.**

## [Spreads.Native](https://www.nuget.org/packages/Spreads.Native)

### Native compression

[Spreads.Native.Compression](http://docs.dataspreads.io/spreads/libs/native/api/Spreads.Native.Compression.html) class exposes methods from [Blosc](https://github.com/Blosc/c-blosc/): 
* SIMD-optimized shuffle/unshuffle.
* Compression: LZ4, Zstd, Zlib/GZip/Deflate compression/decompression. 
Currently works on Windows x64/x86 and Linux x64 (tested on WSL & Docker Ubuntu). Targets `netstandard2.0`.

### Mimalloc

Full [mimalloc](https://github.com/microsoft/mimalloc) API in .NET.

### Cpu.GetCurrentCoreId method

Equivalent of [Thread.GetCurrentProcessorId](https://docs.microsoft.com/en-us/dotnet/api/system.threading.thread.getcurrentprocessorid?view=netcore-3.1) method
that works on .NET Standard 2.0 and guarantees that the returned value could be used directly
as an index in arrays with Cpu.CoreCount length. This allows to avoid expensive modulo 
operation in the most common use cases of per-core data structures.

## [Spreads.Unsafe](https://www.nuget.org/packages/Spreads.Unsafe)

### UnsafeEx

[UnsafeEx](http://docs.dataspreads.io/spreads/libs/native/api/Spreads.Native.UnsafeEx.html) class contains unsafe IL helper methods that we cannot implement in C#.

### Constrained generic calls without constraints

Generic methods ending with `Constrained` emit a constrained call to instance methods of known interfaces on instances of a generic type `T` 
without a type constraint `where T : IKnownInterface<T>`.

For example, calling the `IComparable<T>.CompareTo` method is implemented like this:


```
  .method public hidebysig static int32 CompareToConstrained<T>(!!T& left, !!T& right) cil managed aggressiveinlining
  {
        .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor() = ( 01 00 00 00 )
        .maxstack 8
        ldarg.0
        ldarg.1
        ldobj !!T
        constrained. !!T
        callvirt instance int32 class [System.Runtime]System.IComparable`1<!!T>::CompareTo(!0)
        ret 
  } // end of method Unsafe::CompareToConstrained
```

In addition to the `IComparable<T>` interface there are `IEquatable<T>` and the following custom ones in `Spreads` namespace:

[`IDelta<T>`](http://docs.dataspreads.io/spreads/libs/native/api/Spreads.Native.IDelta-1.html)
```
public interface IDelta<T>
{
    T AddDelta(T delta);
    T GetDelta(T other);
}

```
[`IInt64Diffable<T>`](http://docs.dataspreads.io/spreads/libs/native/api/Spreads.Native.IInt64Diffable-1.html)

```
public interface IInt64Diffable<T> : IComparable<T>
{
    T Add(long diff);
    long Diff(T other);
}

```


#### `KeyComparer<T>`
---------------------

The main use case and sample usage is [`KeyComparer<T>`](http://docs.dataspreads.io/spreads/api/Spreads.KeyComparer-1.html). 
A benchmark shows that the unsafe `CompareToConstrained` method and the `KeyComparer<T>` that uses it are c.2x faster than the `Comparer<T>.Default`
when called via the `IComparer<T>` interface and are c.1.6x faster when the default comparer is called directly as a class.


[ComparerInterfaceAndCachedConstrainedComparer](https://github.com/Spreads/Spreads/blob/11625d1632ec5b8ce62c40c4215b1e6e48a6998d/tests/Spreads.Core.Tests/Collections/KeyComparerTests.cs#L15)


 Case                |    MOPS |  Elapsed |   GC0 |   GC1 |   GC2 |  Memory 
------------         |--------:|---------:|------:|------:|------:|--------:
Unsafe               |  403.23 |   248 ms |   0.0 |   0.0 |   0.0 | 0.000 MB
KeyComparer*         |  396.83 |   252 ms |   0.0 |   0.0 |   0.0 | 0.000 MB
Default              |  255.75 |   391 ms |   0.0 |   0.0 |   0.0 | 0.000 MB
Interface            |  211.42 |   473 ms |   0.0 |   0.0 |   0.0 | 0.000 MB


\* `KeyComparer<T>` uses the [JIT compile-time constant optimization](https://github.com/dotnet/corefx/blob/master/src/System.Numerics.Vectors/src/System/Numerics/Vector.cs#L14-L20) 
for known types and falls back to the `Unsafe.CompareToConstrained` method for types that implement `IComparable<T>` interface.
On .NET 4.6.1 there is no visible difference with and without the special cases: `Unsafe.CompareToConstrained` 
performs as fast as the `if (typeof(T) == typeof(Int32)) { ... }` pattern. See the discussion [here](https://github.com/Spreads/Spreads/issues/100#issuecomment-298184971) and
implementation with comments [here](https://github.com/Spreads/Spreads/blob/62639cea51a3df0010501e3dcba8d7a85f2e3022/src/Spreads.Core/KeyComparer.cs#L177-L226) explaining why 
the special cases could be needed on some platforms.


Unsafe methods could only be called on instances of a generic type `T` when the type implements a relevant interface. `KeyComparer<T>`
has a `static readonly` field that (in theory) allows to use the same JIT optimization mentioned above:

```
private static readonly bool IsIComparable = typeof(IComparable<T>).GetTypeInfo().IsAssignableFrom(typeof(T));

public int Compare(T x, T y)
{
    ...
    if (IsIComparable) // JIT compile-time constant 
    {
    return Unsafe.CompareToConstrained(ref x, ref y);
    }
    ...
}

```

But even if such optimization breaks in this particular case (see the linked discussion) then checking a static bool field is 
still much cheaper than a virtual call, especially given that its value is constant for the lifetime of a program and branch 
prediction should be 100% effective.

#### FastDictionary
---------------

Another use case is [`FastDictionary<TKey,TValue>`](http://docs.dataspreads.io/spreads/api/Spreads.Collections.Generic.FastDictionary-2.html) 
that uses unsafe methods via [`KeyEqualityComparer<T>`](http://docs.dataspreads.io/spreads/api/Spreads.KeyEqualityComparer-1.html), 
which is very similar to `KeyComparer<T>` above. FastDictionay is a rewrite of [`S.C.G.Dictionary<TKey,TValue>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2) that avoids virtual calls
to an equality comparer.

A benchmark for `<int,int>` types shows that `FastDictionary<int,int>` is c.70% faster than `S.C.G.Dictionary<int,int>`:

[CompareSCGAndFastDictionaryWithInts](https://github.com/Spreads/Spreads/blob/8de8e7c5077002fd3d212bb8b2331e3802554e1f/tests/Spreads.Core.Tests/Collections/FastDictionaryTests.cs#L17)


 Case                |    MOPS |  Elapsed |   GC0 |   GC1 |   GC2 |  Memory
---------------      |--------:|---------:|------:|------:|------:|--------:
FastDictionary       |  120.48 |   415 ms |   0.0 |   0.0 |   0.0 | 0.000 MB
Dictionary           |   71.63 |   698 ms |   0.0 |   0.0 |   0.0 | 0.000 MB


Such implementation is much simpler than one with an additoinal generic parameter for a comparer, as recently discussed in this [blog post](https://ayende.com/blog/177377/fast-dictionary-and-struct-generic-arguments).
It is also more flexible than constraining `TKey` to `where TKey : IEquatable<TKey>` and gives the same performance.

Another benchmark with a key as a [custom 16-bytes `Symbol` struct](http://docs.dataspreads.io/spreads/api/Spreads.DataTypes.Symbol.html) shows c.50% performance gain:


[CompareSCGAndFastDictionaryWithSymbol](https://github.com/Spreads/Spreads/blob/8de8e7c5077002fd3d212bb8b2331e3802554e1f/tests/Spreads.Core.Tests/Collections/FastDictionaryTests.cs#L65)

 Case                |    MOPS |  Elapsed |   GC0 |   GC1 |   GC2 |  Memory
---------------      |--------:|---------:|------:|------:|------:|--------:
FastDictionary       |   63.69 |   157 ms |   0.0 |   0.0 |   0.0 | 0.000 MB
Dictionary           |   43.29 |   231 ms |   0.0 |   0.0 |   0.0 | 0.000 MB

## License

MPL 2.0. See the [license file](https://github.com/Spreads/Spreads.Native/blob/master/LICENSE.txt) and [third-party licenses](https://github.com/Spreads/Spreads.Native/blob/master/LICENSE.Dependencies.txt).
