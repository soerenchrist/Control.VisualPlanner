using System.Runtime.InteropServices.ComTypes;
using Control.VisualPlanner.Platforms.Common.Abstractions;
using SkiaSharp;

namespace Control.VisualPlanner.Platforms.Common
{
    public class PathPlannerItem : PlannerItem
    {
        private readonly SKPath _path;

        public PathPlannerItem(SKPath path)
        {
            _path = path;
        }

        public override void Draw(SKCanvas canvas, SKPaint paint)
        {
            if (_path == null) return;
            var transform = SKMatrix.MakeIdentity();

            var scale = GetScaleMatrix();
            var rotate = GetRotationMatrix();
            var translate = GetTranslationMatrix();

            transform = transform.PostConcat(scale);
            transform = transform.PostConcat(translate);
            transform = transform.PostConcat(rotate);

            _path.Transform(transform);

            canvas.DrawPath(_path, paint);
        }
    }
}