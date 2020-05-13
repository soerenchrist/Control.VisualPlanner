using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using Control.VisualPlanner.Platforms.Common.Abstractions;
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

        #region DrawMode (Bindable bool)

        public static readonly BindableProperty DrawModeProperty =
            BindableProperty.Create(nameof(DrawMode),
                typeof(bool),
                typeof(VisualPlannerPanel),
                false);

        public bool DrawMode
        {
            get => (bool) GetValue(DrawModeProperty);
            set => SetValue(DrawModeProperty, value);
        }

        #endregion DrawMode (Bindable bool)

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
        }

        private void CanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            _surface = e.Surface;
            _canvas = _surface.Canvas;

            _canvas.Clear();
            DrawBackground(e.Info.Width, e.Info.Height);
            if (Items == null) return;
            foreach (var plannerItem in Items)
            {
                var paint = plannerItem.Equals(SelectedItem) ? SelectedPaint : DefaultPaint;
                plannerItem.Draw(_canvas, paint);
            }


            if (DrawingPath == null) return;
            _canvas.DrawPath(DrawingPath, DrawPaint);
        }

        private void DrawBackground(int width, int height)
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
            if (DrawMode)
            {
                Draw(args.Type, ConvertToPixel(args.Location));
                return;
            }

            if (args.Type == TouchActionType.Pressed)
            {
                var element = GetElementInBounds(args.Location);
                if (element == null)
                {
                    return;
                }

                ;
                SetSelectedItem(element);
                CanvasView.InvalidateSurface();
                return;
            }

            if (args.Type == TouchActionType.Moved)
            {
                var selectedElement = GetElementInBounds(args.Location);
                if (selectedElement == null) return;
                var point = ConvertToPixel(args.Location);
                selectedElement.X = point.X - (selectedElement.ScaledWidth / 2);
                selectedElement.Y = point.Y - (selectedElement.ScaledHeight / 2);

                CanvasView.InvalidateSurface();
            }
        }

        private void Draw(TouchActionType argsType, SKPoint point)
        {
            if (DrawingPath == null) return;
            if (argsType == TouchActionType.Pressed)
            {
                DrawingPath.MoveTo(point);
                return;
            }

            if (argsType == TouchActionType.Moved)
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

        private PlannerItem GetElementInBounds(Point location)
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