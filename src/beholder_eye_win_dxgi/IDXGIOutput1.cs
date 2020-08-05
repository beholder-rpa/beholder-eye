namespace beholder_eye_win.DXGI
{
    using SharpGen.Runtime;

    public partial class IDXGIOutput1
    {
        public void FindClosestMatchingMode1(ComObject concernedDevice, ModeDescription1 modeToMatch, out ModeDescription1 closestMatch)
        {
            FindClosestMatchingMode1(ref modeToMatch, out closestMatch, concernedDevice);
        }

        public ModeDescription1[] GetDisplayModeList1(Format format, DisplayModeEnumerationFlags flags)
        {
            int count = 0;
            GetDisplayModeList1(format, (int)flags, ref count, null);
            var result = new ModeDescription1[count];
            if (count > 0)
            {
                GetDisplayModeList1(format, (int)flags, ref count, result);
            }

            return result;
        }
    }
}
