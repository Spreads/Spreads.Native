
// Metadata version: v4.0.30319

.class public abstract auto ansi sealed beforefieldinit Spreads.UnsafeEx
       extends [netstandard]System.Object
{
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

  .method public hidebysig static void CalliDataBlock<K,V>(object dlg, object target, int32 index, !!K& key, !!V& val, native int functionPtr) cil managed aggressiveinlining
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
	calli default void (object dlg, object target, int32 index, !!K& key, !!V& val)
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
    IL_000d:  callvirt   instance !0 class Spreads.IDelta`1<!!T>::AddDelta(!0)
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
    IL_000d:  callvirt   instance !0 class Spreads.IDelta`1<!!T>::GetDelta(!0)
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
    IL_0008:  callvirt   instance !0 class Spreads.IInt64Diffable`1<!!T>::Add(int64)
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
    IL_000d:  callvirt   instance int64 class Spreads.IInt64Diffable`1<!!T>::Diff(!0)
    IL_0012:  ret
  } // end of method Unsafe::DiffLongConstrained


  /////////////////////////////////////////////////////////////////////////////////////////////////////

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

  .method public hidebysig static int32 Ceq(float32 first, float32 second) cil managed aggressiveinlining
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

  .method public hidebysig static int32 Ceq(float64 first, float64 second) cil managed aggressiveinlining
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

  .method public hidebysig static int32 BoolAsInt(bool boolValue) cil managed aggressiveinlining
  {
    .custom instance void System.Runtime.Versioning.NonVersionableAttribute::.ctor()
             = {}
    //
    .maxstack 1
	ldarg.0
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

