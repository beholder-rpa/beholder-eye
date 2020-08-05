namespace beholder_eye
{
    using System.Drawing;

    public interface IFocusArea
    {
        Rectangle GetScreenRegion();
    }

    public interface IFocusArea<T, TU> : IFocusArea
        where T : IFocusAreaProcessor<TU>
    {
        /// <summary>
        /// Returns a value that is used to determine focus area uniqueness
        /// </summary>
        /// <remarks>
        /// Return a date/time string if reducing FPS is needed or a canonical identifier of the frame. If a empty or null string is returned, the focus area is not processed.
        /// </remarks>
        /// <param name="focusAreaBitmap"></param>
        /// <returns></returns>
        int GetFocusAreaKey(Bitmap focusAreaBitmap);

        /// <summary>
        /// Returns an instance of a focus area processor for the focus area.
        /// </summary>
        /// <returns></returns>
        T GetFocusAreaProcessor();
    }
}
