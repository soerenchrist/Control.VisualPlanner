using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("PluginVisualPlanner")]
[assembly: ExportEffect(typeof(Control.VisualPlanner.Platforms.Droid.TouchEffect), "TouchEffect")]
namespace Control.VisualPlanner.Platforms.Droid
{
    public class TouchEffect : Xamarin.Forms.Platform.Android.PlatformEffect
    {
        Android.Views.View _view;
        Element _formsElement;
        Common.TouchEffect _libTouchEffect;
        bool capture;
        Func<double, double> _fromPixels;
        readonly int[] _twoIntArray = new int[2];

        private static readonly Dictionary<Android.Views.View, TouchEffect> ViewDictionary =
            new Dictionary<Android.Views.View, TouchEffect>();

        private static readonly Dictionary<int, TouchEffect> IdToEffectDictionary =
            new Dictionary<int, TouchEffect>();

        protected override void OnAttached()
        {
            // Get the Android View corresponding to the Element that the effect is attached to
            _view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the .NET Standard library
            Common.TouchEffect touchEffect =
                (Common.TouchEffect)Element.Effects.
                    FirstOrDefault(e => e is Common.TouchEffect);

            if (touchEffect != null && _view != null)
            {
                ViewDictionary.Add(_view, this);

                _formsElement = Element;

                _libTouchEffect = touchEffect;

                // Save fromPixels function
                _fromPixels = _view.Context.FromPixels;

                // Set event handler on View
                _view.Touch += OnTouch;
            }
        }

        protected override void OnDetached()
        {
            if (ViewDictionary.ContainsKey(_view))
            {
                ViewDictionary.Remove(_view);
                _view.Touch -= OnTouch;
            }
        }

        void OnTouch(object sender, Android.Views.View.TouchEventArgs args)
        {
            // Two object common to all the events
            Android.Views.View senderView = sender as Android.Views.View;
            MotionEvent motionEvent = args.Event;

            // Get the pointer index
            int pointerIndex = motionEvent.ActionIndex;

            // Get the id that identifies a finger over the course of its progress
            int id = motionEvent.GetPointerId(pointerIndex);


            senderView.GetLocationOnScreen(_twoIntArray);
            Point screenPointerCoords = new Point(_twoIntArray[0] + motionEvent.GetX(pointerIndex),
                                                  _twoIntArray[1] + motionEvent.GetY(pointerIndex));


            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    FireEvent(this, id, Common.TouchActionType.Pressed, screenPointerCoords, true);

                    IdToEffectDictionary.Add(id, this);

                    capture = _libTouchEffect.Capture;
                    break;

                case MotionEventActions.Move:
                    // Multiple Move events are bundled, so handle them in a loop
                    for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                    {
                        id = motionEvent.GetPointerId(pointerIndex);

                        if (capture)
                        {
                            senderView.GetLocationOnScreen(_twoIntArray);

                            screenPointerCoords = new Point(_twoIntArray[0] + motionEvent.GetX(pointerIndex),
                                                            _twoIntArray[1] + motionEvent.GetY(pointerIndex));

                            FireEvent(this, id, Common.TouchActionType.Moved, screenPointerCoords, true);
                        }
                        else
                        {
                            CheckForBoundaryHop(id, screenPointerCoords);

                            if (IdToEffectDictionary[id] != null)
                            {
                                FireEvent(IdToEffectDictionary[id], id, Common.TouchActionType.Moved, screenPointerCoords, true);
                            }
                        }
                    }
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                    if (capture)
                    {
                        FireEvent(this, id, Common.TouchActionType.Released, screenPointerCoords, false);
                    }
                    else
                    {
                        CheckForBoundaryHop(id, screenPointerCoords);

                        if (IdToEffectDictionary[id] != null)
                        {
                            FireEvent(IdToEffectDictionary[id], id, Common.TouchActionType.Released, screenPointerCoords, false);
                        }
                    }
                    IdToEffectDictionary.Remove(id);
                    break;

                case MotionEventActions.Cancel:
                    if (capture)
                    {
                        FireEvent(this, id, Common.TouchActionType.Cancelled, screenPointerCoords, false);
                    }
                    else
                    {
                        if (IdToEffectDictionary[id] != null)
                        {
                            FireEvent(IdToEffectDictionary[id], id, Common.TouchActionType.Cancelled, screenPointerCoords, false);
                        }
                    }
                    IdToEffectDictionary.Remove(id);
                    break;
            }
        }

        void CheckForBoundaryHop(int id, Point pointerLocation)
        {
            TouchEffect touchEffectHit = null;

            foreach (Android.Views.View view in ViewDictionary.Keys)
            {
                // Get the view rectangle
                try
                {
                    view.GetLocationOnScreen(_twoIntArray);
                }
                catch // System.ObjectDisposedException: Cannot access a disposed object.
                {
                    continue;
                }
                Rectangle viewRect = new Rectangle(_twoIntArray[0], _twoIntArray[1], view.Width, view.Height);

                if (viewRect.Contains(pointerLocation))
                {
                    touchEffectHit = ViewDictionary[view];
                }
            }

            if (touchEffectHit != IdToEffectDictionary[id])
            {
                if (IdToEffectDictionary[id] != null)
                {
                    FireEvent(IdToEffectDictionary[id], id, Common.TouchActionType.Exited, pointerLocation, true);
                }
                if (touchEffectHit != null)
                {
                    FireEvent(touchEffectHit, id, Common.TouchActionType.Entered, pointerLocation, true);
                }
                IdToEffectDictionary[id] = touchEffectHit;
            }
        }

        void FireEvent(TouchEffect touchEffect, int id, Common.TouchActionType actionType, Point pointerLocation, bool isInContact)
        {
            // Get the method to call for firing events
            Action<Element, Common.TouchActionEventArgs> onTouchAction = touchEffect._libTouchEffect.OnTouchAction;

            // Get the location of the pointer within the view
            touchEffect._view.GetLocationOnScreen(_twoIntArray);
            double x = pointerLocation.X - _twoIntArray[0];
            double y = pointerLocation.Y - _twoIntArray[1];
            Point point = new Point(_fromPixels(x), _fromPixels(y));

            // Call the method
            onTouchAction(touchEffect._formsElement,
                new Common.TouchActionEventArgs(id, actionType, point, isInContact));
        }
    }
}