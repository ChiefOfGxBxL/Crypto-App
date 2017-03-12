using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Runtime;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Android.Content;

namespace App2
{
    [Activity(Label = "Crypto App", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ISensorEventListener
    {
        
        Button _btn2;
        Button _nfcStartBtn;
        SensorManager _sensorManager;
        const SensorDelay delay = SensorDelay.Normal;
        List<string> fileName = new List<string>();

        private void initializeUIComponents()
        {
            _btn2 = FindViewById<Button>(Resource.Id.button2);
            _nfcStartBtn = FindViewById<Button>(Resource.Id.nfcButton);
            _btn2.Click += _btn2_Click;
            _nfcStartBtn.Click += _nfcStart;
        }

        private void _btn2_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(ContactsListActivity));
            StartActivity(intent);
        }

        private void _nfcStart(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(NfcActivity));
            StartActivity(intent);
        }

        private void registerSensors()
        {
            if(_sensorManager == null)
            {
                _sensorManager = (SensorManager)GetSystemService(SensorService);
            }

            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), delay);
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
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
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
            //CONSTANTLY WRITES MAY WANT TO CHANGE THIS
            for (int i = 0; i < e.Values.Count; i++)
                writeToFloatBin(e.Sensor.Type.ToString() + ".bin", e.Values[i]);

            EntropyManager.FeedData(e.Sensor.Type, e.Values);
        }
    }
}

