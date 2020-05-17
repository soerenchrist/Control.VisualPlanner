using SkiaSharp;

namespace Control.VisualPlanner.Platforms.Common.Helper
{
    public static class Helpers
    {
        public static SKPaint Copy(this SKPaint paint)
        {
            return new SKPaint
            {
                Color = paint.Color,
                Style = paint.Style,
                StrokeWidth = paint.StrokeWidth,
                StrokeCap = paint.StrokeCap
            };
        }
    }
}