namespace beholder_eye_win.DXGI
{
    using SharpGen.Runtime;
    using System;

    public partial class IDXGIInfoQueue
    {
        public unsafe InfoQueueMessage GetMessage(Guid producer, long messageIndex)
        {
            PointerSize messageSize = 0;
            GetMessage(producer, messageIndex, IntPtr.Zero, ref messageSize);

            if (messageSize == 0)
            {
                return new InfoQueueMessage();
            }

            var messagePtr = stackalloc byte[messageSize];
            GetMessage(producer, messageIndex, new IntPtr(messagePtr), ref messageSize);

            var message = new InfoQueueMessage();
            message.__MarshalFrom(ref *(InfoQueueMessage.__Native*)messagePtr);
            return message;
        }


        public unsafe InfoQueueFilter GetStorageFilter(Guid producer)
        {
            var sizeFilter = PointerSize.Zero;
            GetStorageFilter(producer, IntPtr.Zero, ref sizeFilter);

            if (sizeFilter == 0)
            {
                return null;
            }
            var filter = stackalloc byte[(int)sizeFilter];
            GetStorageFilter(producer, (IntPtr)filter, ref sizeFilter);

            var queueNative = new InfoQueueFilter();
            queueNative.__MarshalFrom(ref *(InfoQueueFilter.__Native*)filter);

            return queueNative;
        }

        public unsafe InfoQueueFilter GetRetrievalFilter(Guid producer)
        {
            var sizeFilter = PointerSize.Zero;
            GetRetrievalFilter(producer, IntPtr.Zero, ref sizeFilter);

            if (sizeFilter == 0)
            {
                return null;
            }
            var filter = stackalloc byte[(int)sizeFilter];
            GetRetrievalFilter(producer, (IntPtr)filter, ref sizeFilter);

            var queueNative = new InfoQueueFilter();
            queueNative.__MarshalFrom(ref *(InfoQueueFilter.__Native*)filter);

            return queueNative;
        }
    }
}
