namespace beholder_eye_win.DXGI
{
    using SharpGen.Runtime;
    using System;
    using System.Runtime.InteropServices;

    public partial class InfoQueueFilterDescription
    {
        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        public InfoQueueMessageCategory[] Categories { get; set; }

        /// <summary>
        /// Gets or sets the severities.
        /// </summary>
        public InfoQueueMessageSeverity[] Severities { get; set; }

        /// <summary>
        /// Gets or sets the ids.
        /// </summary>
        public int[] Ids { get; set; }

        #region Marshal
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        internal struct __Native
        {
            public int NumCategories;
            public IntPtr PCategoryList;
            public int NumSeverities;
            public IntPtr PSeverityList;
            public int NumIDs;
            public IntPtr PIDList;

            internal void __MarshalFree()
            {
                if (PCategoryList != IntPtr.Zero)
                    Marshal.FreeHGlobal(PCategoryList);
                if (PSeverityList != IntPtr.Zero)
                    Marshal.FreeHGlobal(PSeverityList);
                if (PIDList != IntPtr.Zero)
                    Marshal.FreeHGlobal(PIDList);
            }
        }

        internal unsafe void __MarshalFree(ref __Native @ref)
        {
            @ref.__MarshalFree();
        }

        internal unsafe void __MarshalFrom(ref __Native @ref)
        {
            Categories = new InfoQueueMessageCategory[@ref.NumCategories];
            if (@ref.NumCategories > 0)
            {
                MemoryHelpers.Read(@ref.PCategoryList, Categories, 0, @ref.NumCategories);
            }

            Severities = new InfoQueueMessageSeverity[@ref.NumSeverities];
            if (@ref.NumSeverities > 0)
            {
                MemoryHelpers.Read(@ref.PSeverityList, Severities, 0, @ref.NumSeverities);
            }

            Ids = new int[@ref.NumIDs];
            if (@ref.NumIDs > 0)
            {
                MemoryHelpers.Read(@ref.PIDList, Ids, 0, @ref.NumIDs);
            }
        }

        internal unsafe void __MarshalTo(ref __Native @ref)
        {
            @ref.NumCategories = Categories?.Length ?? 0;
            @ref.PCategoryList = Interop.AllocToPointer(Categories);
            @ref.NumSeverities = Severities?.Length ?? 0;
            @ref.PSeverityList = Interop.AllocToPointer(Severities);
            @ref.NumIDs = Ids?.Length ?? 0;
            @ref.PIDList = Interop.AllocToPointer(Ids);
        }
        #endregion
    }
}
