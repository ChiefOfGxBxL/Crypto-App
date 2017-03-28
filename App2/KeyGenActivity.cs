using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Hardware;
using System.IO;
using System.Threading;

namespace App2
{
    [Activity(Label = "KeyGenActivity")]
    public class KeyGenActivity : Activity, ISensorEventListener
    {

        SensorManager _sensorManager;
        const SensorDelay delay = SensorDelay.Normal;
        List<string> fileName = new List<string>();
        Android.Widget.ProgressBar progBar;
        TextView text;
        Button finishedGen;
        Button startGen;
        int progStatus = 0;
        int oldStatus = 0;
        bool recording;
        string contactName; // which contact we are generating the pad for
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.KeyGen);
            
            
            text = FindViewById<TextView>(Resource.Id.textView1);
            progBar = FindViewById<ProgressBar>(Resource.Id.progBar);
            finishedGen = FindViewById<Button>(Resource.Id.button1);
            startGen = FindViewById<Button>(Resource.Id.button2);
            SetProgressBarIndeterminate(false);
            progBar.Progress = 0;
            progBar.Max = 100;
            finishedGen.Enabled = false;
            startGen.Click += StartGen_Click;
            finishedGen.Click += _nfcStart;

            // Get the contact name passed in through the intent when ViewContactActivity launches this intent
            contactName = Intent.GetStringExtra("contactName"); // stored in key "contactName"

            Toast.MakeText(ApplicationContext, contactName, ToastLength.Long).Show();
        }

        private void StartGen_Click(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                text.Text = "Start Shaking";
            });
            
            Thread.Sleep(1000);
            recording = true;
            registerSensors();
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (progBar.Progress < 100)
                _sensorManager.UnregisterListener(this); // unregisters all sensors
        }
        

        protected override void OnResume()
        {
            base.OnResume();
            if (progBar.Progress < 100 && _sensorManager == null)
                registerSensors(); //reset sensors on resume
        }

        private void _nfcStart(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(NfcActivity));
            StartActivity(intent);
        }


        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            //SensorStatus.Unreliable
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (!recording) return;
            EntropyManager.FeedData(e.Sensor.Type, e.Values);
            
            progStatus = (EntropyManager.GetSbSize() * 100 )/ EntropyManager.GetTotalSbSize();

            RunOnUiThread(() =>
            {
                progBar.IncrementProgressBy(progStatus-oldStatus);
            });
            oldStatus = progStatus;

            if (progBar.Progress >= 100)
            {
                finishedGen.Enabled = true;
                _sensorManager.UnregisterListener(this);
            }
        }



        private void registerSensors()
        {
            if (_sensorManager == null)
            {
                _sensorManager = (SensorManager)GetSystemService(SensorService);
            }

            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), delay);
            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Gyroscope), delay);
        }

    }
}