namespace SharpGen.Runtime
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A COM Interface Callback
    /// </summary>
    public class ComObjectShadow : CppObjectShadow
    {
        private Result QueryInterface(Guid guid, out IntPtr output)
        {
            output = Callback.Shadow.Find(guid);

            if (output == IntPtr.Zero)
            {
                return Result.NoInterface.Code;
            }

            ((IUnknown)Callback).AddRef();

            return Result.Ok.Code;

        }

        protected override CppObjectVtbl Vtbl { get; } = new ComObjectVtbl(0);

        protected class ComObjectVtbl : CppObjectVtbl
        {
            public ComObjectVtbl(int numberOfCallbackMethods)
                : base(numberOfCallbackMethods + 3)
            {
                AddMethod(new QueryInterfaceDelegate(QueryInterfaceImpl));
                AddMethod(new AddRefDelegate(AddRefImpl));
                AddMethod(new ReleaseDelegate(ReleaseImpl));
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate int QueryInterfaceDelegate(IntPtr thisObject, IntPtr guid, out IntPtr output);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate uint AddRefDelegate(IntPtr thisObject);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate uint ReleaseDelegate(IntPtr thisObject);

            protected unsafe static int QueryInterfaceImpl(IntPtr thisObject, IntPtr guid, out IntPtr output)
            {
                var shadow = ToShadow<ComObjectShadow>(thisObject);

                return shadow.QueryInterface(*(Guid*)guid, out output).Code;
            }

            protected static uint AddRefImpl(IntPtr thisObject)
            {
                var shadow = ToShadow<ComObjectShadow>(thisObject);

                var obj = (IUnknown)shadow.Callback;

                return obj.AddRef();
            }

            protected static uint ReleaseImpl(IntPtr thisObject)
            {
                var shadow = ToShadow<ComObjectShadow>(thisObject);

                var obj = (IUnknown)shadow.Callback;

                return obj.Release();
            }
        }
    }
}
