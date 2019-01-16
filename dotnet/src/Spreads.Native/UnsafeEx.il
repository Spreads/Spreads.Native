
.class public abstract auto ansi sealed beforefieldinit Spreads.Native.UnsafeEx
       extends [System.Runtime]System.Object
{
  
  .method private hidebysig static int32 CopyWrapper(void* source, native int sourceLength, void* destination, native int destinationLength, int32 clevel) cil managed aggressiveinlining
  {
        .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor() = ( 01 00 00 00 )
        .maxstack 3
        ldarg.2
		ldarg.0
        ldarg.1
        call int32 [mscorlib]System.IntPtr::op_Explicit(native int)
		conv.ovf.u4
        unaligned. 0x1
        cpblk
		ldarg.1
		call int32 [mscorlib]System.IntPtr::op_Explicit(native int)
        ret
  }

  .method private hidebysig static int32 CopyWrapper2(void* source, native int sourceLength, void* destination, native int destinationLength) cil managed aggressiveinlining
  {
        .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor() = ( 01 00 00 00 )
        .maxstack 3
        ldarg.2
		ldarg.0
        ldarg.1
        call int32 [mscorlib]System.IntPtr::op_Explicit(native int)
		conv.ovf.u4
        unaligned. 0x1
        cpblk
		ldarg.1
		call int32 [mscorlib]System.IntPtr::op_Explicit(native int)
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
    IL_000d:  callvirt   instance int32 class [System.Runtime]System.IComparable`1<!!T>::CompareTo(!0)
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
    IL_000d:  callvirt   instance bool class [System.Runtime]System.IEquatable`1<!!T>::Equals(!0)
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
    IL_0007:  callvirt   instance int32 [System.Runtime]System.Object::GetHashCode()
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
    IL_0007:  callvirt   instance void [System.Runtime]System.IDisposable::Dispose()
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

	.method public hidebysig static !!T Get<T>(
		object obj, uint8* offset) cil managed aggressiveinlining
	{
		.maxstack 2
		.locals ([0] uint8& addr)
		ldarg.0     // load the object
		stloc.0     // convert the object pointer to a byref
		ldloc.0     // load the object pointer as a byref
		ldarg.1     // load the offset
		add         // add the offset
		ldobj !!T   // load a T value from the computed address
		ret
	}

	.method public hidebysig static !!T& GetRef<T>(
		object obj, uint8* offset) cil managed aggressiveinlining
	{
		.maxstack 2
		.locals ([0] uint8& addr)
		ldarg.0     // load the object
		stloc.0     // convert the object pointer to a byref
		ldloc.0     // load the object pointer as a byref
		ldarg.1     // load the offset
		add         // add the offset
		ret
	}

    .method public hidebysig static void Set<T>(
        object obj, uint8* offset, !!T val) cil managed aggressiveinlining
    {
        .maxstack 2
        .locals ([0] uint8& addr)
        ldarg.0     // load the object
        stloc.0     // convert the object pointer to a byref
        ldloc.0     // load the object pointer as a byref
        ldarg.1     // load the offset
        add         // add the offset
        ldarg.2     // load the value to store
        stobj !!T   // store a T value to the computed address
        ret
    }

    .method public hidebysig static int32 ElemOffset<T>(!!T[] arr) cil managed
    {
        ldarg.0
        ldc.i4 0
        ldelema !!T
        ldarg.0
        sub
        ret
    }
} // end of class

.class private auto ansi sealed beforefieldinit System.Runtime.Versioning.NonVersionableAttribute
       extends [System.Runtime]System.Attribute
{
  .custom instance void [System.Runtime]System.AttributeUsageAttribute::.ctor(valuetype [System.Runtime]System.AttributeTargets) = ( 01 00 6C 00 00 00 02 00 54 02 0D 41 6C 6C 6F 77   // ..l.....T..Allow
                                                                                                                                     4D 75 6C 74 69 70 6C 65 00 54 02 09 49 6E 68 65   // Multiple.T..Inhe
                                                                                                                                     72 69 74 65 64 00 )                               // rited.
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 
    .maxstack  1
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method NonVersionableAttribute::.ctor

}

