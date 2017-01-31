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
        // private _sensorManager

        public AccelerometerSensor()
        {
            // TODO: follow the tutorial below to listen to the accelerometer data
            throw new NotImplementedException();
        }

        public string getCurrentValue()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void OnAccuracyChanged(Android.Hardware.Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            throw new NotImplementedException();
        }

        public void OnSensorChanged(SensorEvent e)
        {
            throw new NotImplementedException();
        }
    }
}