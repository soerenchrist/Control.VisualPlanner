using SkiaSharp;

namespace Control.VisualPlanner.Platforms.Common.Models
{
    public class Arrow
    {
        public SKPoint StartPoint { get; }
        public SKPoint EndPoint { get; }
        public SKPaint Paint { get; }

        public Arrow(SKPoint startPoint, SKPoint endPoint, SKPaint paint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Paint = paint;
        }
    }
}