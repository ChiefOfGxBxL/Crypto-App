using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Runtime;
using System;

namespace App2
{
    [Activity(Label = "App2", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ISensorEventListener
    {
        EditText _editText;
        EditText _editText2;
        Button _getEntropyBtn;
        SensorManager _sensorManager;
        const SensorDelay delay = SensorDelay.Normal;

        private void initializeUIComponents()
        {
            _editText = FindViewById<EditText>(Resource.Id.editText1);
            _editText2 = FindViewById<EditText>(Resource.Id.editText2);
            _getEntropyBtn = FindViewById<Button>(Resource.Id.button1);

            _getEntropyBtn.Click += generateEntropyClickEvt;

            _editText.Text = "HI!! :)";
        }

        private void generateEntropyClickEvt(object sender, EventArgs e)
        {
            _editText2.Text = EntropyManager.GetBlockOfEntropyBytes();
        }

        private void setupSensors()
        {
            if(_sensorManager == null)
            {
                _sensorManager = (SensorManager)GetSystemService(SensorService);
            }

            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), delay);
            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.AmbientTemperature), delay);
            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Light), delay);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            initializeUIComponents();
            setupSensors();

            _editText2.Text = EntropyManager.GetBlockOfEntropyBytes();
        }

        protected override void OnPause()
        {
            base.OnPause();
            _sensorManager.UnregisterListener(this); // unregisters all sensors
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            //SensorStatus.Unreliable
        }

        public void OnSensorChanged(SensorEvent e)
        {
            _editText.Text = e.Sensor.Type + " " + e.Values[0] + " " + e.Values[1] + " " + e.Values[2]; // DEBUG ONLY
            EntropyManager.FeedData(e.Sensor.Type, e.Values);
        }
    }
}

