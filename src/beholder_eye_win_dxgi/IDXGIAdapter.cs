namespace beholder_eye_win.DXGI
{
    using SharpGen.Runtime;
    using System;
    using System.Collections.Generic;

    public partial class IDXGIAdapter
    {
        public IDXGIOutput[] EnumOutputs()
        {
            var outputs = new List<IDXGIOutput>();
            while (true)
            {
                var result = EnumOutputs(outputs.Count, out var output);
                if (result == ResultCode.NotFound || output == null)
                {
                    break;
                }

                outputs.Add(output);
            }

            return outputs.ToArray();
        }

        public bool CheckInterfaceSupport<T>() where T : ComObject
        {
            return CheckInterfaceSupport(typeof(T), out var userModeVersion);
        }

        public bool CheckInterfaceSupport<T>(out long userModeVersion) where T : ComObject
        {
            return CheckInterfaceSupport(typeof(T), out userModeVersion);
        }

        public bool CheckInterfaceSupport(Type type, out long userModeDriverVersion)
        {
            return CheckInterfaceSupport(type.GUID, out userModeDriverVersion).Success;
        }
    }
}
