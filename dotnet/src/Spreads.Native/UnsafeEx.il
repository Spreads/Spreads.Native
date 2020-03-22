
// Metadata version: v4.0.30319

.class public abstract auto ansi sealed beforefieldinit Spreads.Native.UnsafeEx
       extends [netstandard]System.Object
{
  
  .method private hidebysig static int32 CopyWrapper(void* source, native int sourceLength, void* destination, native int destinationLength, int32 clevel) cil managed aggressiveinlining
  {
        .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor() = ( 01 00 00 00 )
        .maxstack 3
        ldarg.2
		ldarg.0
        ldarg.1
        call int32 [netstandard]System.IntPtr::op_Explicit(native int)
		conv.ovf.u4
        unaligned. 0x1
        cpblk
		ldarg.1
		call int32 [netstandard]System.IntPtr::op_Explicit(native int)
        ret
  }

  .method private hidebysig static int32 CopyWrapper2(void* source, native int sourceLength, void* destination, native int destinationLength) cil managed aggressiveinlining
  {
        .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor() = ( 01 00 00 00 )
        .maxstack 3
        ldarg.2
		ldarg.0
        ldarg.1
        call int32 [netstandard]System.IntPtr::op_Explicit(native int)
		conv.ovf.u4
        unaligned. 0x1
        cpblk
		ldarg.1
		call int32 [netstandard]System.IntPtr::op_Explicit(native int)
        ret
  } 

  // same signature as for CalliCompress
  .method public hidebysig static native int CopyCompressMethod() cil managed aggressiveinlining
  {
		ldftn int32 class Spreads.Native.UnsafeEx::CopyWrapper (void* source, native int sourceLength, void* destination, native int destinationLength, int32 clevel)
		ret	
  }

  .method public hidebysig static native int CopyDecompressMethod() cil managed aggressiveinlining
  {
		ldftn int32 class Spreads.Native.UnsafeEx::CopyWrapper2 (void* source, native int sourceLength, void* destination, native int destinationLength)
		ret	
  }

  .method public hidebysig static int32 CalliCompress(void* source, native int sourceLength, void* destination, native int destinationLength, int32 clevel, native int functionPtr) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack  6
	ldarg.0
    ldarg.1
    ldarg.2
    ldarg.3
	ldarg 4
	ldarg 5
	calli int32 (uint8* source, native int sourceLength, uint8* destination, native int destinationLength, int32 clevel) 
	ret
  }

  .method public hidebysig static int32 CalliCompressUnmanagedCdecl(void* source, native int sourceLength, void* destination, native int destinationLength, int32 clevel, native int functionPtr) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack  6
	ldarg.0
    ldarg.1
    ldarg.2
    ldarg.3
	ldarg 4
	ldarg 5
	calli unmanaged cdecl int32 (void* source, native int sourceLength, void* destination, native int destinationLength, int32 clevel) 
	ret
  }

  .method public hidebysig static int32 CalliDecompress(void* source, native int sourceLength, void* destination, native int destinationLength, native int functionPtr) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack  5
	ldarg.0
    ldarg.1
    ldarg.2
    ldarg.3
	ldarg 4
	calli int32 (uint8* source, native int sourceLength, uint8* destination, native int destinationLength) 
	ret
  }

  .method public hidebysig static int32 CalliDecompressUnmanagedCdecl(void* source, native int sourceLength, void* destination, native int destinationLength, native int functionPtr) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack  5
	ldarg.0
    ldarg.1
    ldarg.2
    ldarg.3
	ldarg 4
	calli unmanaged cdecl int32 (uint8* source, native int sourceLength, uint8* destination, native int destinationLength) 
	ret
  }

  .method public hidebysig static void CalliShuffleUnshuffle(native int typeSize, native int blockSize, void* source, void* destination, native int functionPtr) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 5
	ldarg.0
    ldarg.1
    ldarg.2
    ldarg.3
	ldarg 4
	calli void (native int typeSize, native int blockSize, uint8* source, uint8* destination) 
	ret
  }

  
  .method public hidebysig static int32 CalliIntPtrPtrUintPptr(void* ptr1, void* ptr2, uint32 uint1, void** pptr1, native int functionPtr) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 5
	ldarg.0
    ldarg.1
    ldarg.2
    ldarg.3
	ldarg 4
	calli int32 (void* ptr1, void* ptr2, uint32 uint1, void** pptr1) 
	ret
  }

  
  .method public hidebysig static int32 CalliIntPtrUintPtrPtr(void* ptr1, uint32 uint1, void* ptr2, void* ptr3, native int functionPtr) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 5
	ldarg.0
    ldarg.1
    ldarg.2
    ldarg.3
	ldarg 4
	calli int32 (void* ptr1, uint32 uint1, void* ptr2, void* ptr3) 
	ret
  }

  .method public hidebysig static int32 CalliIntPtrUintPtrPtrUint(void* ptr1, uint32 uint1, void* ptr2, void* ptr3, uint32 uint2, native int functionPtr) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 6
	ldarg.0
    ldarg.1
    ldarg.2
    ldarg.3
	ldarg 4
	ldarg 5
	calli int32 (void* ptr1, uint32 uint1, void* ptr2, void* ptr3, uint32 uint2) 
	ret
  }

  .method public hidebysig static void CalliVoidPtr(void* ptr1, native int functionPtr) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
	calli void (void* ptr1) 
	ret
  }

  .method public hidebysig static int32 CalliIntPtr(void* ptr1, native int functionPtr) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
	calli int32 (void* ptr1) 
	ret
  }

  .method public hidebysig static int32 CalliIntPtrPtr(void* ptr1, void* ptr2, native int functionPtr) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 3
	ldarg.0
    ldarg.1
    ldarg.2
	calli int32 (void* ptr1, void* ptr2) 
	ret
  }

  .method public hidebysig static int32 CalliIntPtrPtrPtrInt(void* ptr1, void* ptr2, void* ptr3, int32 int1, native int functionPtr) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 5
	ldarg.0
    ldarg.1
    ldarg.2
	ldarg.3
	ldarg 4
	calli int32 (void* ptr1, void* ptr2, void* ptr3, int32 int1) 
	ret
  }

  .method public hidebysig static int32 CalliIntPtrPtrPtrUint(void* ptr1, void* ptr2, void* ptr3, uint32 uint1, native int functionPtr) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 5
	ldarg.0
    ldarg.1
    ldarg.2
	ldarg.3
	ldarg 4
	calli int32 (void* ptr1, void* ptr2, void* ptr3, uint32 uint1)
	ret
  }


  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

  .method public hidebysig static int32  CompareToConstrained<T>(!!T& left,
                                                                 !!T& right) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldobj      !!T
    IL_0007:  constrained. !!T
    IL_000d:  callvirt   instance int32 class [netstandard]System.IComparable`1<!!T>::CompareTo(!0)
    IL_0012:  ret
  } // end of method Unsafe::CompareToConstrained

  .method public hidebysig static bool  EqualsConstrained<T>(!!T& left,
                                                             !!T& right) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldobj      !!T
    IL_0007:  constrained. !!T
    IL_000d:  callvirt   instance bool class [netstandard]System.IEquatable`1<!!T>::Equals(!0)
    IL_0012:  ret
  } // end of method Unsafe::EqualsConstrained

  .method public hidebysig static int32  GetHashCodeConstrained<T>(!!T& obj) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  constrained. !!T
    IL_0007:  callvirt   instance int32 [netstandard]System.Object::GetHashCode()
    IL_000c:  ret
  } // end of method Unsafe::GetHashCodeConstrained

  .method public hidebysig static void  DisposeConstrained<T>(!!T& obj) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  constrained. !!T
    IL_0007:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
    IL_000c:  ret
  } // end of method Unsafe::DisposeConstrained

  .method public hidebysig static !!T  AddDeltaConstrained<T>(!!T& obj,
                                                              !!T& delta) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldobj      !!T
    IL_0007:  constrained. !!T
    IL_000d:  callvirt   instance !0 class Spreads.Native.IDelta`1<!!T>::AddDelta(!0)
    IL_0012:  ret
  } // end of method Unsafe::AddDeltaConstrained

  .method public hidebysig static !!T  GetDeltaConstrained<T>(!!T& obj,
                                                              !!T& other) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldobj      !!T
    IL_0007:  constrained. !!T
    IL_000d:  callvirt   instance !0 class Spreads.Native.IDelta`1<!!T>::GetDelta(!0)
    IL_0012:  ret
  } // end of method Unsafe::GetDeltaConstrained

  .method public hidebysig static !!T  AddLongConstrained<T>(!!T& obj,
                                                             int64 delta) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  constrained. !!T
    IL_0008:  callvirt   instance !0 class Spreads.Native.IInt64Diffable`1<!!T>::Add(int64)
    IL_000d:  ret
  } // end of method Unsafe::AddLongConstrained

  .method public hidebysig static int64  DiffLongConstrained<T>(!!T& left,
                                                                !!T& right) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldobj      !!T
    IL_0007:  constrained. !!T
    IL_000d:  callvirt   instance int64 class Spreads.Native.IInt64Diffable`1<!!T>::Diff(!0)
    IL_0012:  ret
  } // end of method Unsafe::DiffLongConstrained


  /////////////////////////////////////////////////////////////////////////////////////////////////////

  .method public hidebysig static native int GetterMethodPointer<T>() cil managed aggressiveinlining
  {
    .maxstack 1
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor() = ( 01 00 00 00 )
    ldftn object class Spreads.Native.UnsafeEx::GetAsObject<!!T> (object obj, native int offset, int32 index)
    ret	
  }

  .method public hidebysig static native int SetterMethodPointer<T>() cil managed aggressiveinlining
  {
    .maxstack 1
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor() = ( 01 00 00 00 )
    ldftn void class Spreads.Native.UnsafeEx::SetAsObject<!!T> (object obj, native int offset, int32 index, object val)
    ret	
  }

  .method public hidebysig static object GetIndirect(
    object obj, native int offset, int32 index, native int functionPtr) cil managed aggressiveinlining
  {
    .maxstack 4
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor() = ( 01 00 00 00 )
    ldarg.0
    ldarg.1
    ldarg.2
    ldarg.3
    calli default object (object obj, native int offset, int32 index)
    ret
  }

  .method public hidebysig static void SetIndirect(
   object obj, native int offset, int32 index,  object val, native int functionPtr) cil managed aggressiveinlining
  {
    .maxstack 5
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor() = ( 01 00 00 00 )
    ldarg.0
    ldarg.1
    ldarg.2
    ldarg.3
    ldarg 4
    calli default void (object obj, native int offset, int32 index, object val)
    ret
  }
  
  .method public hidebysig static int32 Ceq(int32 first, int32 second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    ceq
	ret
  }
  
  .method public hidebysig static int32 Cgt(int32 first, int32 second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    cgt
	ret
  }
  
  .method public hidebysig static int32 Clt(int32 first, int32 second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    clt
	ret
  }
  
  .method public hidebysig static int32 Ceq(int64 first, int64 second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    ceq
	ret
  }
  
  .method public hidebysig static int32 Cgt(int64 first, int64 second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    cgt
	ret
  }
  
  .method public hidebysig static int32 Clt(int64 first, int64 second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    clt
	ret
  }

  .method public hidebysig static int32 Ceq(native int first, native int second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    ceq
	ret
  }
  
  .method public hidebysig static int32 Cgt(native int first, native int second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    cgt
	ret
  }
  
  .method public hidebysig static int32 Clt(native int first, native int second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    clt
	ret
  }
  
  .method public hidebysig static bool CgtB(native int first, native int second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    cgt
	ret
  }
  
  .method public hidebysig static bool CltB(native int first, native int second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    clt
	ret
  }
  
  .method public hidebysig static void SkipInit<T> ([out] !!T& 'value') cil managed aggressiveinlining
  {
        .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor() = ( 01 00 00 00 )
        .maxstack 0
        ret
  }
  
  .method public hidebysig static native int Add(native int first, native int second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    add
	ret
  }
  
  .method public hidebysig static native int Add(native int first, int32 second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    add
	ret
  }
  
  .method public hidebysig static native int Sub(native int first, native int second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    sub
	ret
  }
  
  .method public hidebysig static native int Sub(native int first, int32 second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    sub
	ret
  }
  
  .method public hidebysig static native int Mul(native int first, native int second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    mul
	ret
  }
  
  .method public hidebysig static native int Mul(native int first, int32 second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    mul
	ret
  }
  
  .method public hidebysig static native int Neg(native int 'value') cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 1
	ldarg.0
    neg
	ret
  }
  
  .method public hidebysig static native int And(native int first, native int second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    and
	ret
  }
  
  .method public hidebysig static native int And(native int first, int32 second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    and
	ret
  }
  
  .method public hidebysig static native int Or(native int first, native int second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    or
	ret
  }
  
  .method public hidebysig static native int Or(native int first, int32 second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    or
	ret
  }
  
  .method public hidebysig static native int Xor(native int first, native int second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    xor
	ret
  }
  
  .method public hidebysig static native int Xor(native int first, int32 second) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    xor
	ret
  }
  
  .method public hidebysig static native int Not(native int 'value') cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 1
	ldarg.0
    not
	ret
  }
  
  .method public hidebysig static native int Shr(native int toBeShifted, native int shiftBy) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    shr
	ret
  }
  
  .method public hidebysig static native int Shr(native int toBeShifted, int32 shiftBy) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    shr
	ret
  }
  
  .method public hidebysig static native int ShrUn(native int toBeShifted, native int shiftBy) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    shr.un
	ret
  }
  
  .method public hidebysig static native int ShrUn(native int toBeShifted, int32 shiftBy) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    shr.un
	ret
  }
  
  .method public hidebysig static native int Shl(native int toBeShifted, native int shiftBy) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    shl
	ret
  }
  
  .method public hidebysig static native int Shl(native int toBeShifted, int32 shiftBy) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    // 
    .maxstack 2
	ldarg.0
    ldarg.1
    shl
	ret
  }
  
} // end of class

.class private auto ansi sealed beforefieldinit System.Runtime.Versioning.NonVersionableAttribute
       extends [netstandard]System.Attribute
{
  .custom instance void [netstandard]System.AttributeUsageAttribute::.ctor(valuetype [netstandard]System.AttributeTargets) = ( 01 00 6C 00 00 00 02 00 54 02 0D 41 6C 6C 6F 77   // ..l.....T..Allow
                                                                                                                                     4D 75 6C 74 69 70 6C 65 00 54 02 09 49 6E 68 65   // Multiple.T..Inhe
                                                                                                                                     72 69 74 65 64 00 )                               // rited.
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 
    .maxstack  1
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [netstandard]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method NonVersionableAttribute::.ctor

}

