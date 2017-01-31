using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Content;
using Android.Runtime;
using System;

namespace App
{
    [Activity(Label = "Crypto App", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        // Sensors
        AccelerometerSensor _accel;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            //initializeUIComponents();
            EditText _editText2 = FindViewById<EditText>(Resource.Id.editText1);
            _editText2.Text = "HI!! :)";
        }
    }
}
