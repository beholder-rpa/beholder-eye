namespace beholder_eye_win.DXGI
{
    using SharpGen.Runtime;

    public partial class IDXGIOutput
    {
        public void FindClosestMatchingMode(ComObject concernedDevice, ModeDescription modeToMatch, out ModeDescription closestMatch)
        {
            FindClosestMatchingMode(ref modeToMatch, out closestMatch, concernedDevice);
        }

        public ModeDescription[] GetDisplayModeList(Format format, DisplayModeEnumerationFlags flags)
        {
            int count = 0;
            GetDisplayModeList(format, (int)flags, ref count, null);
            var result = new ModeDescription[count];
            if (count > 0)
            {
                GetDisplayModeList(format, (int)flags, ref count, result);
            }
            return result;
        }
    }
}
