using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware;

namespace App
{
    public class AccelerometerSensor : ISensor, ISensorEventListener
    {
        private SensorManager _sensorManager;
        private float[] _sensorData = new float[3];
        Func<object, bool> _dataCallback;

        public AccelerometerSensor(Context context, Func<object, bool> callback = null)
        {
            Console.WriteLine("Console -- create accelerometer");
            System.Diagnostics.Debug.Write("Debug -- create accelerometer");
            _sensorManager = (SensorManager)context.GetSystemService(Context.SensorService); ;
            _dataCallback = callback;
        }

        public void startListener()
        {
            _sensorManager.RegisterListener(this,
                _sensorManager.GetDefaultSensor(SensorType.Accelerometer),
                SensorDelay.Normal);

            Console.Clear();
            Console.WriteLine("Listening on accelerometer");
        }

        public void pauseListener()
        {
            _sensorManager.UnregisterListener(this);
        }

        public object getCurrentValue()
        {
            return _sensorData;
        }

        public IntPtr Handle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            // _sensorManager.Dispose();
        }

        public void OnAccuracyChanged(Android.Hardware.Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {

        }

        public void OnSensorChanged(SensorEvent e)
        {
            // Update the local copy of the sensor data, which may be retrieved any time via getCurrentValue()
            _sensorData[0] = e.Values[0];
            _sensorData[1] = e.Values[1];
            _sensorData[2] = e.Values[2];

            if (_dataCallback != null)
            {
                Console.WriteLine("Found data");
                _dataCallback(_sensorData);
            }
        }
    }
}