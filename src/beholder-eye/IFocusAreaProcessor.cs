namespace beholder_eye
{
    using System.Drawing;

    public interface IFocusAreaProcessor<T>
    {
        /// <summary>
        /// Returns a data object that is the result of processing the the specified focus area bitmap.
        /// </summary>
        /// <param name="focusAreaBitmap"></param>
        /// <returns></returns>
        T ProcessFocusArea(Bitmap focusAreaBitmap);
    }
}
