using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace Control.VisualPlanner.Platforms.Common.Abstractions
{
    public abstract class PlannerItem : INotifyPropertyChanged
    {
        #region Properties

        private float _x;
        private float _y;
        private float _width;
        private float _height;
        private float _scale = 1;
        private float _rotation;

        public float X
        {
            get => _x;
            set
            {
                _x = value;
                OnPropertyChanged();
            }
        }

        public float Y
        {
            get => _y;
            set
            {
                _y = value;
                OnPropertyChanged();
            }
        }

        public float Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        public float Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        public float Scale
        {
            get => _scale;
            set { 
                _scale = value;
                OnPropertyChanged();
            }
        }
        public float Rotation {
            get => _rotation;
            set {
                _rotation = value;
                OnPropertyChanged();
            }
        }
        public float ScaledWidth => Width * Scale;
        public float ScaledHeight => Height * Scale;

        public float CenterX => X + ScaledWidth / 2;
        public float CenterY => Y + ScaledHeight / 2;


        #endregion


        public abstract void Draw(SKCanvas canvas, SKPaint paint);

        public SKMatrix GetRotationMatrix()
        {
            var result = SKMatrix.MakeIdentity();
            var translate = SKMatrix.MakeTranslation(-CenterX, -CenterY);
            var rotate = SKMatrix.MakeRotationDegrees(Rotation);
            var translate2 = SKMatrix.MakeTranslation(CenterX, CenterY);
            result = result.PostConcat(translate);
            result = result.PostConcat(rotate);
            result = result.PostConcat(translate2);

            return result;
        }

        public SKMatrix GetTranslationMatrix()
        {
            var result = SKMatrix.MakeTranslation(X, Y);
            return result;
        }

        public SKMatrix GetScaleMatrix()
        {
            var result = SKMatrix.MakeScale(Scale, Scale);
            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}