using System;
using System.IO;
using System.Text;
using Control.VisualPlanner.Platforms.Common.Abstractions;
using SkiaSharp;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace Control.VisualPlanner.Platforms.Common
{
    public class SvgPlannerItem : PlannerItem
    {
        private readonly SKSvg _svg;

        public SvgPlannerItem(string svgString)
        {
            if (string.IsNullOrWhiteSpace(svgString))
                throw new ArgumentNullException($"{nameof(svgString)} must not be null or whitespace");

            _svg = new SKSvg();
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(svgString));
            _svg.Load(memoryStream);
        }

        public override void Draw(SKCanvas canvas, SKPaint paint)
        {
            var transform = SKMatrix.MakeIdentity();

            var scaleX = Width / _svg.Picture.CullRect.Width;
            var scaleY = Height / _svg.Picture.CullRect.Height;
            var scaleToSvgSize = SKMatrix.MakeScale(scaleX, scaleY);

            var scaleToProp = GetScaleMatrix();

            var rotate = GetRotationMatrix();
            var translate = GetTranslationMatrix();

            transform = transform.PostConcat(scaleToSvgSize);
            transform = transform.PostConcat(scaleToProp);
            transform = transform.PostConcat(translate);
            transform = transform.PostConcat(rotate);

            canvas.DrawPicture(_svg.Picture, ref transform);
        }
    }
}