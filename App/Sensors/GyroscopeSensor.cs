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
    class GyroscopeSensor : ISensor, ISensorEventListener
    {
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

        public object getCurrentValue()
        {
            throw new NotImplementedException();
        }

        public void startListener() { }
        public void pauseListener() { }

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