namespace beholder_eye_win.Direct3D11
{
    public partial class ID3D11Texture2D
    {
        /// <inheritdoc/>
        public override int CalculateSubResourceIndex(int mipSlice, int arraySlice, out int mipSize)
        {
            var desc = Description;
            mipSize = CalculateMipSize(mipSlice, desc.Height);
            return CalculateSubResourceIndex(mipSlice, arraySlice, desc.MipLevels);
        }
    }
}
