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

namespace App
{
    // e.g. AccelerometerSensor, GyroscopeSensor
    public interface ISensor
    {
        // need a generic data type to handle all sensors
        // e.g. accelerometer might want a tuple/triple, 
        // while gyroscope might have a special class, etc.
        object getCurrentValue();

        /// <summary>
        /// Instructs the listener to begin listening for sensor data.
        /// </summary>
        void startListener();

        /// <summary>
        /// Orders the listener to stop listening for sensor changes.
        /// </summary>
        void pauseListener();
    }
}