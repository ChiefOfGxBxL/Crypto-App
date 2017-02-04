using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Runtime;
using System;
using System.Text;

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
        String data;

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

        private void setupSensors()
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
        /// writes data to a file
        /// creates a file if not found appends otherwise 
        /// path is in Android/data/App2.App2/files folder
        /// actual path might be different
        /// </summary>
        /// <param name="fileName">file name to write to</param>
        /// <param name="writeData">data to write to file</param>
        private void writeToFile( String fileName, String writeData)
        {
            //I use the java stuff because I couldn't declare a C# file for some reason
            Java.IO.File file = new Java.IO.File(GetExternalFilesDir(null), fileName); 
            try
            {
                Java.IO.FileOutputStream os = new Java.IO.FileOutputStream(file, true);
                os.Write(Encoding.ASCII.GetBytes(writeData));
                os.Close();
                
            }catch(Java.IO.IOException e)
            {
                Console.WriteLine("File IO Error: {0}", e.ToString());
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            initializeUIComponents();
            setupSensors();

            _editText1.Text = EntropyManager.GetBlockOfEntropyBytes();
           
        }

        protected override void OnPause()
        {
            base.OnPause();
            writeToFile("sensor_data.txt", data); // write data to file
            _sensorManager.UnregisterListener(this); // unregisters all sensors
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            //SensorStatus.Unreliable
        }

        public void OnSensorChanged(SensorEvent e)
        {
            _textView.Text = e.Sensor.Type + " " + e.Values[0] + " " + e.Values[1] + " " + e.Values[2]; // DEBUG ONLY
            data += e.Values[0] + "\n" + e.Values[1] + "\n" + e.Values[2] + "\n"; //Stores data for writing on close
            EntropyManager.FeedData(e.Sensor.Type, e.Values);
        }
    }
}

