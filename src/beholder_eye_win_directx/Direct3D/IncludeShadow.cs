namespace beholder_eye_win.Direct3D
{
    using SharpGen.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Shadow callback for <see cref="Include"/>.
    /// </summary>
    internal class IncludeShadow : CppObjectShadow
    {
        private static readonly IncludeVtbl _vtbl = new IncludeVtbl();
        private readonly Dictionary<IntPtr, Frame> _frames = new Dictionary<IntPtr, Frame>(DefaultIntPtrComparer);

        private struct Frame
        {
            public Frame(Stream stream, GCHandle handle)
            {
                Stream = stream;
                Handle = handle;
            }

            public readonly Stream Stream;
            public readonly GCHandle Handle;

            public void Close()
            {
                if (Handle.IsAllocated)
                    Handle.Free();
            }
        }

        /// <summary>
        /// Return a pointer to the unmanaged version of this callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <returns>A pointer to a shadow c++ callback</returns>
        public static IntPtr ToIntPtr(Include callback)
        {
            return ToCallbackPtr<Include>(callback);
        }

        /// <summary>
        /// Internal Include Callback
        /// </summary>
        private class IncludeVtbl : CppObjectVtbl
        {
            public IncludeVtbl() : base(2)
            {
                AddMethod(new OpenDelegate(OpenImpl));
                AddMethod(new CloseDelegate(CloseImpl));
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate Result OpenDelegate(IntPtr thisPtr, IncludeType includeType, IntPtr fileNameRef, IntPtr pParentData, ref IntPtr dataRef, ref int bytesRef);

            private static Result OpenImpl(IntPtr thisPtr, IncludeType includeType, IntPtr fileNameRef, IntPtr pParentData, ref IntPtr dataRef, ref int bytesRef)
            {
                unsafe
                {
                    try
                    {
                        var shadow = ToShadow<IncludeShadow>(thisPtr);
                        var callback = (Include)shadow.Callback;

                        Stream stream = null;
                        Stream parentStream = null;

                        if (shadow._frames.ContainsKey(pParentData))
                        {
                            parentStream = shadow._frames[pParentData].Stream;
                        }

                        stream = callback.Open(includeType, Marshal.PtrToStringAnsi(fileNameRef), parentStream);
                        if (stream == null)
                            return Result.Fail;

                        GCHandle handle;

                        //if (stream is DataStream)
                        //{
                        //    // Magic shortcut if we happen to get a DataStream
                        //    var data = (DataStream)stream;
                        //    dataRef = data.PositionPointer;
                        //    bytesRef = (int)(data.Length - data.Position);
                        //    handle = new GCHandle();
                        //}
                        //else
                        {
                            // Read the stream into a byte array and pin it
                            byte[] data = ReadStream(stream);
                            handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                            dataRef = handle.AddrOfPinnedObject();
                            bytesRef = data.Length;
                        }

                        shadow._frames.Add(dataRef, new Frame(stream, handle));

                        return Result.Ok;
                    }
                    catch (SharpGenException exception)
                    {
                        return exception.ResultCode.Code;
                    }
                    catch (Exception)
                    {
                        return Result.Fail;
                    }
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate Result CloseDelegate(IntPtr thisPtr, IntPtr pData);

            private static Result CloseImpl(IntPtr thisPtr, IntPtr pData)
            {
                try
                {
                    var shadow = ToShadow<IncludeShadow>(thisPtr);
                    var callback = (Include)shadow.Callback;

                    if (shadow._frames.TryGetValue(pData, out var frame))
                    {
                        shadow._frames.Remove(pData);
                        callback.Close(frame.Stream);
                        frame.Close();
                    }
                    return Result.Ok;
                }
                catch (SharpGenException exception)
                {
                    return exception.ResultCode.Code;
                }
                catch (Exception)
                {
                    return Result.Fail;
                }
            }
        }

        protected override CppObjectVtbl Vtbl => _vtbl;

        private static byte[] ReadStream(Stream stream)
        {
            int readLength = 0;
            return ReadStream(stream, ref readLength);
        }

        private static byte[] ReadStream(Stream stream, ref int readLength)
        {
            Debug.Assert(stream != null);
            Debug.Assert(stream.CanRead);
            int count = readLength;
            Debug.Assert(count <= (stream.Length - stream.Position));
            if (count == 0)
            {
                readLength = (int)(stream.Length - stream.Position);
            }

            count = readLength;

            Debug.Assert(count >= 0);
            if (count == 0)
                return new byte[0];

            byte[] buffer = new byte[count];
            int bytesRead = 0;
            if (count > 0)
            {
                do
                {
                    bytesRead += stream.Read(buffer, bytesRead, readLength - bytesRead);
                } while (bytesRead < readLength);
            }

            return buffer;
        }

        public static readonly IEqualityComparer<IntPtr> DefaultIntPtrComparer = new IntPtrComparer();

        internal class IntPtrComparer : EqualityComparer<IntPtr>
        {
            public override bool Equals(IntPtr x, IntPtr y)
            {
                return x == y;
            }

            public override int GetHashCode(IntPtr obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
