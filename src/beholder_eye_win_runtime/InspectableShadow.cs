namespace SharpGen.Runtime
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Internal IInspectable Callback
    /// </summary>
    public class InspectableShadow : ComObjectShadow
    {

        protected class InspectableVtbl : ComObjectVtbl
        {
            public InspectableVtbl()
                : base(3)
            {
                unsafe
                {
                    AddMethod(new GetIidsDelegate(GetIids));
                    AddMethod(new GetRuntimeClassNameDelegate(GetRuntimeClassName));
                    AddMethod(new GetTrustLevelDelegate(GetTrustLevel));
                }
            }

            //        virtual HRESULT STDMETHODCALLTYPE GetIids( 
            ///* [out] */ __RPC__out ULONG *iidCount,
            ///* [size_is][size_is][out] */ __RPC__deref_out_ecount_full_opt(*iidCount) IID **iids) = 0;

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private unsafe delegate int GetIidsDelegate(IntPtr thisPtr, int* iidCount, IntPtr* iids);
            private unsafe static int GetIids(IntPtr thisPtr, int* iidCount, IntPtr* iids)
            {
                try
                {
                    var shadow = ToShadow<InspectableShadow>(thisPtr);
                    var callback = (IInspectable)shadow.Callback;

                    var container = callback.Shadow;

                    var countGuids = container.Guids.Length;

                    // Copy GUIDs deduced from Callback
                    iids = (IntPtr*)Marshal.AllocCoTaskMem(IntPtr.Size * countGuids);
                    *iidCount = countGuids;

                    MemoryHelpers.CopyMemory((IntPtr)iids, new ReadOnlySpan<IntPtr>(container.Guids));
                }
                catch (Exception exception)
                {
                    return (int)Result.GetResultFromException(exception);
                }
                return Result.Ok.Code;
            }

            //virtual HRESULT STDMETHODCALLTYPE GetRuntimeClassName( 
            //    /* [out] */ __RPC__deref_out_opt HSTRING *className) = 0;

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private unsafe delegate int GetRuntimeClassNameDelegate(IntPtr thisPtr, IntPtr* className);
            private unsafe static int GetRuntimeClassName(IntPtr thisPtr, IntPtr* className)
            {
                try
                {
                    var shadow = ToShadow<InspectableShadow>(thisPtr);
                    var callback = (IInspectable)shadow.Callback;
                    // Use the name of the callback class

                    var name = callback.GetType().FullName;
                    Win32.WinRTStrings.WindowsCreateString(name, (uint)name.Length, out IntPtr str);
                    *className = str;
                }
                catch (Exception exception)
                {
                    return (int)Result.GetResultFromException(exception);
                }
                return Result.Ok.Code;
            }

            //virtual HRESULT STDMETHODCALLTYPE GetTrustLevel( 
            //    /* [out] */ __RPC__out TrustLevel *trustLevel) = 0;
            private enum TrustLevel
            {
                BaseTrust = 0,
                PartialTrust = (BaseTrust + 1),
                FullTrust = (PartialTrust + 1)
            };

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int GetTrustLevelDelegate(IntPtr thisPtr, IntPtr trustLevel);
            private static int GetTrustLevel(IntPtr thisPtr, IntPtr trustLevel)
            {
                try
                {
                    var shadow = ToShadow<InspectableShadow>(thisPtr);
                    var callback = (IInspectable)shadow.Callback;
                    // Write full trust
                    Marshal.WriteInt32(trustLevel, (int)TrustLevel.FullTrust);
                }
                catch (Exception exception)
                {
                    return (int)Result.GetResultFromException(exception);
                }
                return Result.Ok.Code;
            }

        }

        protected override CppObjectVtbl Vtbl { get; } = new InspectableVtbl();
    }
}
