using Android.App;
using Android.OS;
using System;

namespace Control.VisualPlanner.Platforms.Droid
{
    public static class Config
    {
        private static Activity currentActivity;
        private static Bundle currentBundle;

        public static Activity CurrentActivity {
            get => currentActivity ?? throw new NotImplementedException("Android wasn't initialized! Make sure your MainActivity > OnCreate() contains Plugin.InputKit.Platforms.Droid.Config.Init(this, savedInstanceState)");
            private set => currentActivity = value;
        }

        public static Bundle CurrentBundle {
            get => currentBundle ?? throw new NotImplementedException("Android wasn't initialized! Make sure your MainActivity > OnCreate() contains Plugin.InputKit.Platforms.Droid.Config.Init(this, savedInstanceState)");
            set => currentBundle = value;
        }

        public static void Init(Activity activity, Bundle bundle)
        {
            CurrentActivity = activity;
            CurrentBundle = bundle;
            Console.WriteLine("VisualPlanner initialized");
        }
    }
}