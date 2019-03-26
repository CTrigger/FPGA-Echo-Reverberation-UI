using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Runtime;
using Android.Views;

using System;
using Android.Content.PM;

namespace Friendship404
{

    [Activity(ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Label = "Action Selection", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        ImageButton btBluetooth;
        ImageButton btEqualizer;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            btBluetooth = FindViewById<ImageButton>(Resource.Id.btBlutooth);
            btEqualizer = FindViewById<ImageButton>(Resource.Id.btEqualizer);

            btBluetooth.Click += BtBluetooth_Click;
            btEqualizer.Click += BtEqualizer_Click;
        }

        private void BtEqualizer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(EqualizerController));
            this.StartActivity(intent);
            this.OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
        }

        private void BtBluetooth_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(BluetoothController));
            this.StartActivity(intent);
            this.OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
        }
    }
}

