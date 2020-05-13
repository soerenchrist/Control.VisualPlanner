﻿
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("PluginVisualPlanner")]
[assembly: ExportEffect(typeof(Control.VisualPlanner.Platforms.IOS.TouchEffect), "TouchEffect")]
namespace Control.VisualPlanner.Platforms.IOS
{
    public class TouchEffect : PlatformEffect
    {
        private UIView _view;
        private TouchRecognizer _touchRecognizer;

        protected override void OnAttached()
        {
            // Get the iOS UIView corresponding to the Element that the effect is attached to
            _view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the .NET Standard library
            var effect = (Common.TouchEffect)Element.Effects.FirstOrDefault(e => e is Common.TouchEffect);

            if (effect != null && _view != null)
            {
                // Create a TouchRecognizer for this UIView
                _touchRecognizer = new TouchRecognizer(Element, _view, effect);
                _view.AddGestureRecognizer(_touchRecognizer);
            }
        }

        protected override void OnDetached()
        {
            if (_touchRecognizer != null)
            {
                // Clean up the TouchRecognizer object
                _touchRecognizer.Detach();

                // Remove the TouchRecognizer from the UIView
                _view.RemoveGestureRecognizer(_touchRecognizer);
            }
        }
    }
}
