namespace beholder_eye
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public static class BitmapExtensions
    {
        public static Color GetMostCommonColor(this Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            var colors = new List<Color>();
            for(int y = 0; y < bitmap.Height; y++)
            for(int x = 0; x < bitmap.Width; x++)
            {
                    colors.Add(bitmap.GetPixel(x, y));
            }

            return colors
                .GroupBy(c => c)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;
        }

        public static Color GetMostCommonColor(this Bitmap bitmap, Rectangle rect)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            var colors = new List<Color>();
            for (int y = rect.Y; y < rect.Y + rect.Height; y++)
                for (int x = rect.X; x < rect.X + rect.Width; x++)
                {
                    colors.Add(bitmap.GetPixel(x, y));
                }

            return colors
                .GroupBy(c => c)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;
        }
    }
}
