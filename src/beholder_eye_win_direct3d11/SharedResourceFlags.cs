namespace beholder_eye_win.Direct3D11
{
    using System;

    [Flags]
    public enum SharedResourceFlags : uint
    {
        None = 0,
        Write = 1,
        Read = 0x80000000,
        GenericWrite = 0x40000000,
        GenericRead = 0x80000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000
    }
}
