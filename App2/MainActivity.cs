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
using System.Threading.Tasks;

namespace App2
{
    [Activity(Label = "Crypto App", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ISensorEventListener
    {
        
        Button _btn2;
        Button _nfcStartBtn;
        EditText _setNameText;
        Button _setNameBtn;
        SensorManager _sensorManager;
        const SensorDelay delay = SensorDelay.Normal;
        List<string> fileName = new List<string>();

        private void initializeUIComponents()
        {
            _btn2 = FindViewById<Button>(Resource.Id.button2);
            _nfcStartBtn = FindViewById<Button>(Resource.Id.nfcButton);
            _setNameText = FindViewById<EditText>(Resource.Id.editText1);
            _setNameBtn = FindViewById<Button>(Resource.Id.button3);

            _btn2.Click += _btn2_Click;
            _nfcStartBtn.Click += _nfcStart;
            _setNameBtn.Click += _setNameBtn_Click;
        }

        private void _setNameBtn_Click(object sender, EventArgs e)
        {
            // Write name to me.txt
            string metxtFileLoc = Path.Combine(GetExternalFilesDir(null).ToString(), "me.txt");

            // Create and use StreamWriter to output the text to the file
            StreamWriter sw = new StreamWriter(metxtFileLoc, false);
            sw.Write(_setNameText.Text);
            sw.Close();

            // Notify user of success
            Toast.MakeText(this.ApplicationContext, "Set username!", ToastLength.Long).Show();
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

        private string loadUsername()
        {
            string metxtFileLoc = Path.Combine(GetExternalFilesDir(null).ToString(), "me.txt");

            if (!File.Exists(metxtFileLoc)) return "";

            StreamReader sr = new StreamReader(metxtFileLoc);
            string username = sr.ReadToEnd();
            sr.Close();

            return username;
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

            // Set username textbox
            _setNameText.Text = loadUsername();
        }

        protected override void OnPause()
        {
            base.OnPause();

            _sensorManager.UnregisterListener(this); // unregisters all sensors
        }

        protected async override void OnResume()
        {
            base.OnResume();

            // The await is magically needed for the app to work on my emulator
            // Otherwise I get a System.NullReferenceException in OnResume or OnCreate
            // See https://forums.xamarin.com/discussion/comment/219774/#Comment_219774
            await Task.Delay(10);

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

