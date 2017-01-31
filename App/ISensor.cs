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
        // TODO: is string an appropriate way to represent data?
        // the problem is coming up with a type to handle all sensors
        // e.g. accelerometer might want a tuple/triple, while gyroscope
        // might have a special class, etc. we'll have to make our own class
        // to uniform this across sensors.
        string getCurrentValue();
    }
}