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
    // An EntropySource represents a "flavor" of generating random noise for pads.
    // For instance, one entropy source could derive all of its random bytes purely
    // from an accelerometer, while another may exclusively use a gyroscope, while
    // another may use a mix-and-match of these two sensors plus a PRNG.
    //
    // Essentially this will allow us to experiment with different sources of entropy.
    // By adhering to this interface, we can swap different sources easily.
    public interface IEntropySource
    {
        string generateNoiseBytesOfLength(int length);
    }
}