using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Control.VisualPlanner.Platforms.Common.Abstractions;
using Control.VisualPlanner.Platforms.Common.Models;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Control.VisualPlanner.Platforms.Common.Control
{
    public partial class VisualPlannerPanel
    {
        private static readonly SKPaint SelectedPaintDefault = new SKPaint
        {
            Color = SKColors.Blue
        };

        private static readonly SKPaint DefaultPaintValue = new SKPaint
        {
            Color = SKColors.Red
        };

        private static readonly SKPaint DrawPaintDefault = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 5f
        };

        #region ArrowModeProperties

        private SKPoint _arrowStartPoint;
        private SKPoint _arrowCurrentPoint;

        #endregion

        #region BackgroundSvg (Bindable string)

        public static readonly BindableProperty BackgroundSvgProperty =
            BindableProperty.Create(propertyName: nameof(BackgroundSvg),
                returnType: typeof(string),
                declaringType: typeof(VisualPlannerPanel),
                defaultValue: string.Empty,
                propertyChanged: BackgroundSvgPropertyChanged);

        private static void BackgroundSvgPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (!(bindable is VisualPlannerPanel panel))
                return;

            var stringValue = (string) newvalue;
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                panel._background = null;
                return;
            }

            panel._background = new SkiaSharp.Extended.Svg.SKSvg();
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(stringValue));
            panel._background.Load(memoryStream);
        }

        public string BackgroundSvg
        {
            get => (string) GetValue(BackgroundSvgProperty);
            set => SetValue(BackgroundSvgProperty, value);
        }

        #endregion BackgroundSvg (Bindable string)

        #region PlannerMode (Bindable bool)

        public static readonly BindableProperty PlannerModeProperty =
            BindableProperty.Create(nameof(PlannerMode),
                typeof(PlannerMode),
                typeof(VisualPlannerPanel),
                PlannerMode.Move);

        public PlannerMode PlannerMode
        {
            get => (PlannerMode) GetValue(PlannerModeProperty);
            set => SetValue(PlannerModeProperty, value);
        }

        #endregion PlannerMode (Bindable bool)

        #region Arrows (Bindable IEnumerable<Arrow>)

        public static readonly BindableProperty ArrowsProperty =
            BindableProperty.Create(propertyName: nameof(Arrows),
                returnType: typeof(IEnumerable<Arrow>),
                declaringType: typeof(VisualPlannerPanel),
                defaultValue: new ObservableCollection<Arrow>(),
                propertyChanged: GenericPropertyChanged);
        public ObservableCollection<Arrow> Arrows
        {
            get => (ObservableCollection<Arrow>) GetValue(ArrowsProperty);
            set => SetValue(ArrowsProperty, value);
        }
        #endregion
        
        #region Items (Bindable IEnumerable<PlannerElement>)

        public static readonly BindableProperty ItemsProperty =
            BindableProperty.Create(propertyName: nameof(Items),
                returnType: typeof(ObservableCollection<PlannerItem>),
                declaringType: typeof(VisualPlannerPanel),
                defaultValue: new ObservableCollection<PlannerItem>(),
                propertyChanged: GenericPropertyChanged);

        private static void GenericPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var panel = bindable as VisualPlannerPanel;
            if (panel == null) return;

            if (oldvalue is INotifyCollectionChanged observableOld)
                observableOld.CollectionChanged -= panel.Items_CollectionChanged;
            if (newvalue is INotifyCollectionChanged observableNew)
                observableNew.CollectionChanged += panel.Items_CollectionChanged;

            panel.CanvasView.InvalidateSurface();
        }


        public ObservableCollection<PlannerItem> Items
        {
            get => (ObservableCollection<PlannerItem>) GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        #endregion Items (Bindable IEnumerable<PlannerElement>)

        #region DrawingPath (Bindable SKPath)

        public static readonly BindableProperty DrawingPathProperty =
            BindableProperty.Create(propertyName: nameof(DrawingPath),
                returnType: typeof(SKPath),
                declaringType: typeof(VisualPlannerPanel),
                defaultValue: new SKPath(),
                propertyChanged: GenericPropertyChanged);

        public SKPath DrawingPath
        {
            get => (SKPath) GetValue(DrawingPathProperty);
            set => SetValue(DrawingPathProperty, value);
        }

        #endregion DrawingPath (Bindable SKPath)

        #region SelectedItem (Bindable PlannerElement)

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(propertyName: nameof(SelectedItem),
                returnType: typeof(PlannerItem),
                declaringType: typeof(VisualPlannerPanel),
                defaultValue: default,
                BindingMode.OneWayToSource);

        public PlannerItem SelectedItem
        {
            get => (PlannerItem) GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        #endregion SelectedItem (Bindable PlannerElement)

        #region SelectedPaint (Bindable SKPaint)

        public static readonly BindableProperty SelectedPaintProperty =
            BindableProperty.Create(propertyName: nameof(SelectedPaint),
                returnType: typeof(SKPaint),
                declaringType: typeof(VisualPlannerPanel),
                defaultValue: SelectedPaintDefault);

        public SKPaint SelectedPaint
        {
            get => (SKPaint) GetValue(SelectedPaintProperty);
            set => SetValue(SelectedPaintProperty, value);
        }

        #endregion SelectedPaint (Bindable SKPaint)

        #region DefaultPaint (Bindable SKPaint)

        public static readonly BindableProperty DefaultPaintProperty =
            BindableProperty.Create(propertyName: nameof(DefaultPaint),
                returnType: typeof(SKPaint),
                declaringType: typeof(VisualPlannerPanel),
                defaultValue: DefaultPaintValue);

        public SKPaint DefaultPaint
        {
            get => (SKPaint) GetValue(DefaultPaintProperty);
            set => SetValue(DefaultPaintProperty, value);
        }

        #endregion DefaultPaint (Bindable SKPaint)

        #region SelectedBackgroundColor (Bindable SKColor)

        public static readonly BindableProperty SelectedBackgroundColorProperty =
            BindableProperty.Create(propertyName: nameof(SelectedBackgroundColor),
                returnType: typeof(SKColor),
                declaringType: typeof(VisualPlannerPanel),
                defaultValue: SKColors.Blue);

        public SKColor SelectedBackgroundColor
        {
            get => (SKColor) GetValue(SelectedBackgroundColorProperty);
            set => SetValue(SelectedBackgroundColorProperty, value);
        }

        #endregion SelectedBackgroundColor (Bindable SKColor)
        
        #region DrawPaint (Bindable SKPaint)

        public static readonly BindableProperty DrawPaintProperty =
            BindableProperty.Create(propertyName: nameof(DrawPaint),
                returnType: typeof(SKPaint),
                declaringType: typeof(VisualPlannerPanel),
                defaultValue: DrawPaintDefault);

        public SKPaint DrawPaint
        {
            get => (SKPaint) GetValue(DrawPaintProperty);
            set => SetValue(DrawPaintProperty, value);
        }

        #endregion DrawPaint (Bindable SKPaint)
        

        private SkiaSharp.Extended.Svg.SKSvg _background;
        private SKCanvas _canvas;
        private SKSurface _surface;

        public VisualPlannerPanel()
        {
            InitializeComponent();
            Items.CollectionChanged += Items_CollectionChanged;
            Arrows.CollectionChanged += Items_CollectionChanged;
        }

        private void CanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            _surface = e.Surface;
            _canvas = _surface.Canvas;

            _canvas.Clear();
            DrawBackground();
            if (Items == null) return;
            foreach (var plannerItem in Items)
            {
                if (plannerItem.Equals(SelectedItem))
                {
                    DrawSelectionBackground(plannerItem);
                }
                plannerItem.Draw(_canvas, DefaultPaint);
            }


            if (DrawingPath != null)
                _canvas.DrawPath(DrawingPath, DrawPaint);

            DrawArrows();
        }

        private void DrawArrows()
        {
            if (_arrowCurrentPoint != default && _arrowStartPoint != default)
            {
                _canvas.DrawLine(_arrowStartPoint, _arrowCurrentPoint, DrawPaint);
            }

            foreach (var arrow in Arrows)
            {
                _canvas.DrawLine(arrow.StartPoint, arrow.EndPoint, arrow.Paint);
            }
        }

        private void DrawSelectionBackground(PlannerItem item)
        {
            var center = new SKPoint(item.CenterX, item.CenterY);
            var radius = ((item.ScaledWidth + item.ScaledHeight) / 2 + 50) / 2;
            var gradientColors =
                new[] {SelectedBackgroundColor, SKColors.Transparent};
            var background = SKShader.CreateRadialGradient(center, radius, gradientColors, null, SKShaderTileMode.Clamp);
            using var paint = new SKPaint
            {
                Shader = background
            };
            const float rectOffset = 50f;
            var rect = new SKRect(item.X - rectOffset,
                item.Y - rectOffset,
                item.X + item.ScaledWidth + rectOffset,
                item.Y + item.ScaledHeight + rectOffset);
            _canvas.DrawRect(rect, paint);
        }

        private void DrawBackground()
        {
            if (_background == null) return;
            var scaleX = CanvasView.CanvasSize.Width / _background.Picture.CullRect.Width;
            var scaleY = CanvasView.CanvasSize.Height / _background.Picture.CullRect.Height;
            var matrix = SKMatrix.MakeScale(scaleX, scaleY);

            // draw the svg
            _canvas.DrawPicture(_background.Picture, ref matrix);
        }

        private void TouchEffect_OnTouchAction(object sender, TouchActionEventArgs args)
        {
            switch (PlannerMode)
            {
                case PlannerMode.Move:
                    HandleMoveTouches(args);
                    break;
                case PlannerMode.Draw:
                    HandleDrawTouches(args);
                    break;
                case PlannerMode.Arrows:
                    HandleArrowTouches(args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleArrowTouches(TouchActionEventArgs args)
        {
            var point = ConvertToPixel(args.Location);
            var touchType = args.Type;
            switch (touchType)
            {
                case TouchActionType.Pressed:
                    _arrowStartPoint = new SKPoint(point.X, point.Y);
                    break;
                case TouchActionType.Moved:
                    _arrowCurrentPoint = new SKPoint(point.X, point.Y);
                    break;
                case TouchActionType.Released:
                    FinishCurrentArrow();
                    break;
            }
            CanvasView.InvalidateSurface();
        }

        private void FinishCurrentArrow()
        {
            if (_arrowStartPoint == default || _arrowCurrentPoint == default)
                return;
            
            Arrows.Add(new Arrow(_arrowStartPoint, _arrowCurrentPoint, DrawPaint));
            _arrowStartPoint = default;
            _arrowCurrentPoint = default;
        }

        private void HandleMoveTouches(TouchActionEventArgs args)
        {
            if (args.Type == TouchActionType.Pressed)
            {
                var element = GetFirstElementInBounds(args.Location);
                if (element == null)
                {
                    return;
                }
                
                SetSelectedItem(element);
                CanvasView.InvalidateSurface();
                return;
            }

            if (args.Type == TouchActionType.Moved)
            {
                PlannerItem elementToMove;
                // If an element is already selected, take this
                // else take the one in bounds
                var elementsInBounds = GetAllElementsInBounds(args.Location);
                switch (elementsInBounds.Count)
                {
                    case 0:
                        return;
                    case 1:
                        elementToMove = elementsInBounds.First();
                        break;
                    default:
                        elementToMove = elementsInBounds.Contains(SelectedItem) ? SelectedItem : elementsInBounds.First();
                        break;
                }

                var point = ConvertToPixel(args.Location);
                elementToMove.X = point.X - (elementToMove.ScaledWidth / 2);
                elementToMove.Y = point.Y - (elementToMove.ScaledHeight / 2);

                CanvasView.InvalidateSurface();
            }
        }

        private void HandleDrawTouches(TouchActionEventArgs args)
        {
            var point = ConvertToPixel(args.Location);
            var touchType = args.Type;
            if (DrawingPath == null) return;
            if (touchType == TouchActionType.Pressed)
            {
                DrawingPath.MoveTo(point);
                return;
            }

            if (touchType == TouchActionType.Moved)
            {
                DrawingPath.LineTo(point);
            }

            CanvasView.InvalidateSurface();
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CanvasView.InvalidateSurface();
        }


        private void SetSelectedItem(PlannerItem element)
        {
            if (SelectedItem is INotifyPropertyChanged oldItem)
                oldItem.PropertyChanged -= SelectedItemPropertyChanged;
            if (element is INotifyPropertyChanged newItem)
                newItem.PropertyChanged += SelectedItemPropertyChanged;
            SelectedItem = element;
        }

        private void SelectedItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CanvasView.InvalidateSurface();
        }

        private SKPoint ConvertToPixel(Point pt)
        {
            return new SKPoint((float)(CanvasView.CanvasSize.Width * pt.X / Width),
                (float)(CanvasView.CanvasSize.Height * pt.Y / Height));
        }
        
        private IList<PlannerItem> GetAllElementsInBounds(Point location)
        {
            const int threshold = 50;
            var point = ConvertToPixel(location);
            var items = new List<PlannerItem>();
            foreach (var element in Items)
            {
                var xLeft = element.X - threshold;
                var yTop = element.Y - threshold;
                var xRight = element.X + element.ScaledWidth + threshold;
                var yBottom = element.Y + element.ScaledHeight + threshold;

                if (point.X > xLeft && point.Y > yTop
                                    && point.X < xRight && point.Y < yBottom)
                    items.Add(element);
            }

            return items;
        }

        private PlannerItem GetFirstElementInBounds(Point location)
        {
            const int threshold = 50;
            var point = ConvertToPixel(location);
            foreach (var element in Items)
            {
                var xLeft = element.X - threshold;
                var yTop = element.Y - threshold;
                var xRight = element.X + element.ScaledWidth + threshold;
                var yBottom = element.Y + element.ScaledHeight + threshold;

                if (point.X > xLeft && point.Y > yTop
                                    && point.X < xRight && point.Y < yBottom)
                    return element;
            }

            return null;
        }
    }
}