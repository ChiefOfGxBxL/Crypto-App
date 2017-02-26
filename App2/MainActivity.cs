using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Runtime;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace App2
{
    [Activity(Label = "App2", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ISensorEventListener
    {
        
        TextView _textView;
        EditText _editText1;
        Button _getEntropyBtn;
        SensorManager _sensorManager;
        const SensorDelay delay = SensorDelay.Normal;
        List<string> fileName = new List<string>();

        private void initializeUIComponents()
        {
            _textView = FindViewById<TextView>(Resource.Id.textView1);
            _editText1 = FindViewById<EditText>(Resource.Id.editText1);
            _getEntropyBtn = FindViewById<Button>(Resource.Id.button1);

            _getEntropyBtn.Click += generateEntropyClickEvt;

            _textView.Text = "HI!! :)";
        }

        private void generateEntropyClickEvt(object sender, EventArgs e)
        {
            _editText1.Text = EntropyManager.GetBlockOfEntropyBytes();
        }

        private void registerSensors()
        {
            if(_sensorManager == null)
            {
                _sensorManager = (SensorManager)GetSystemService(SensorService);
            }

            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), delay);
            //_sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.AmbientTemperature), delay);
            //_sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Light), delay);
            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Gyroscope), delay);
        }


        /// <summary>
        /// writes floats to a binary file
        /// creates a file if not found appends otherwise 
        /// path is in Android/data/App2.App2/files folder
        /// actual path might be different
        /// </summary>
        /// <param name="fileName">file name to write to</param>
        /// <param name="writeData">data to write to file</param>
        private void writeToFloatBin( String fileName,  float writeData)
        {
            string path = GetExternalFilesDir(null).ToString();
            string file = Path.Combine(path, fileName);
            using (var binWriter = new BinaryWriter(new FileStream(file, FileMode.Append)))
            {
                binWriter.Write(writeData);
                binWriter.Close();
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            initializeUIComponents();
            registerSensors();

            _editText1.Text = EntropyManager.GetBlockOfEntropyBytes();
           
        }

        protected override void OnPause()
        {
            base.OnPause();

            _sensorManager.UnregisterListener(this); // unregisters all sensors
        }

        protected override void OnResume()
        {
            base.OnResume();
            registerSensors(); //reset sensors on resume
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            //SensorStatus.Unreliable
        }

        public void OnSensorChanged(SensorEvent e)
        {
            _textView.Text = e.Sensor.Type + " " + e.Values[0] + " " + e.Values[1] + " " + e.Values[2]; // DEBUG ONLY


            //CONSTANTLY WRITES MAY WANT TO CHANGE THIS
            for (int i = 0; i < e.Values.Count; i++)
                writeToFloatBin(e.Sensor.Type.ToString() + ".bin", e.Values[i]);

            EntropyManager.FeedData(e.Sensor.Type, e.Values);
        }
    }
}

