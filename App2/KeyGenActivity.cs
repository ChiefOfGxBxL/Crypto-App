using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Hardware;
using System.Threading;
using Android.Nfc;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace App2
{
    [Activity(Label = "KeyGenActivity", LaunchMode = Android.Content.PM.LaunchMode.SingleTop),
     IntentFilter(new[] { "android.nfc.action.NDEF_DISCOVERED" },
     Categories = new[] { "android.intent.category.DEFAULT" },
     DataMimeType = "padbook/nfc")]

    public class KeyGenActivity : Activity, ISensorEventListener, NfcAdapter.ICreateNdefMessageCallback, NfcAdapter.IOnNdefPushCompleteCallback
    {

        SensorManager _sensorManager;
        const SensorDelay delay = SensorDelay.Normal;
        Android.Widget.ProgressBar progBar;
        TextView text;
        Button finishedGen;
        Button startGen;
        int progStatus = 0;
        int oldStatus = 0;
        bool recording;
        string contactName; // which contact we are generating the pad for
        string myName; //your username for sending over nfc

        const Int32 MESSAGE_SENT = 1; //basically a #define
        public NfcAdapter _nfcAdapter;
        private const string Mime = "padbook/nfc";
        private readonly nfcHandler _Handler;
        private string key;
        private string[] keys;
        private PadManager pm;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.KeyGen);


            text = FindViewById<TextView>(Resource.Id.textView1);
            progBar = FindViewById<ProgressBar>(Resource.Id.progBar);
            finishedGen = FindViewById<Button>(Resource.Id.button1);
            startGen = FindViewById<Button>(Resource.Id.button2);

            pm = new PadManager(GetExternalFilesDir(null).ToString());

            SetProgressBarIndeterminate(false);
            progBar.Progress = 0;
            progBar.Max = 100;
            finishedGen.Enabled = false;
            startGen.Click += StartGen_Click;
            finishedGen.Click += StartNfc_Click;

            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
            if (_nfcAdapter == null)
            {
                text.Text = "NFC not available";
            }
            if (pm == null)
                text.Text = "Pad manager is null";

            // Get the contact name passed in through the intent when ViewContactActivity launches this intent
            contactName = Intent.GetStringExtra("contactName"); // stored in key "contactName"
            Identity.LoadUsername(GetExternalFilesDir(null).ToString());
            myName = Identity.Username;
            text.Text = contactName;

            Toast.MakeText(ApplicationContext, contactName, ToastLength.Long).Show();
        }

        private void StartGen_Click(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                text.Text = "Start Shaking";
            });

            Thread.Sleep(1000);
            recording = true;
            registerSensors();
        }

        private void StartNfc_Click(object sender, EventArgs e)
        {
            text.Text = contactName;
            Padbook friendBook = pm.GetPadbookForUsername(contactName);

            // The `keys` array is used for pads and nfc exchange
            // But GetBlockOfEntropyBytes destroys the data from the string builder,
            // and we need a copy of it for calculating compression... so keep `key` and `keys`
            key = EntropyManager.GetBlockOfEntropyBytes();
            keys = new string[] { key };

            friendBook.AppendPads(keys);

            // Compute quality of entropy by running compression
            string inputStr = key;
            byte[] compressed; // output byte array after compression

            using (var outStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outStream, CompressionMode.Compress))
                using (var inStream = new MemoryStream(Encoding.UTF8.GetBytes(inputStr)))
                    inStream.CopyTo(gzipStream);

                compressed = outStream.ToArray();
            }

            // Calculate compression ratio - use floats because % will be between 0-1, but we want the decimal accuracy
            float compressionRatio = ((float)inputStr.Length - (float)compressed.Length) / (float)inputStr.Length;
            // TODO: display compression ratio to user to indicate entropy quality
            // NOTE: ^ lower compression = better! :: "Compression ratio: " + compressionRatio*100 + "%"

            // NFC exchange
            _nfcAdapter.SetNdefPushMessageCallback(this, this);
            _nfcAdapter.SetOnNdefPushCompleteCallback(this, this);
            text.Text = "Move Your Phone Near Your Friend's";
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (progBar.Progress < 100)
                _sensorManager.UnregisterListener(this); // unregisters all sensors
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (NfcAdapter.ActionNdefDiscovered == Intent.Action)
            {
                ProcessIntent(Intent);
            }
            if (progBar.Progress < 100 && _sensorManager == null)
                registerSensors(); //reset sensors on resume
            if (pm == null)
            {
                pm = new PadManager(GetExternalFilesDir(null).ToString());
            }
        }
        //NEW INTENT STUFF FOR NFC
        protected override void OnNewIntent(Intent intent)
        {
            Intent = intent;
        }

        void ProcessIntent(Intent intent)
        {
            IParcelable[] rawMsg = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
            NdefMessage msg = (NdefMessage)rawMsg[0];
            string datagram = Encoding.UTF8.GetString(msg.GetRecords()[0].GetPayload());
            contactName = datagram.Split(',')[0];
            keys = datagram.Split(',')[1].Split('#'); //keys are seperated with a hash mark

            Padbook friendBook = pm.GetPadbookForUsername(contactName);
            friendBook.AppendPads(keys);
        }
        //NFC EVENTS
        public NdefMessage CreateNdefMessage(NfcEvent evt)
        {
            if (myName == null)
            {
                Identity.LoadUsername(GetExternalFilesDir(null).ToString());
                myName = Identity.Username;
            }
            string datagram = myName + ",";
            foreach (string e in keys)
            {
                datagram += e + "#";
            }
            NdefRecord mimeRec = new NdefRecord(
                NdefRecord.TnfMimeMedia, Encoding.UTF8.GetBytes(Mime),
                new byte[0], Encoding.UTF8.GetBytes(datagram));

            NdefMessage msg = new NdefMessage(new NdefRecord[] { mimeRec });
            return msg;
        }

        public void OnNdefPushComplete(NfcEvent evt)
        {
            _Handler.ObtainMessage(MESSAGE_SENT).SendToTarget();

        }
        //HANDLER FOR NFC ACTIVITY
        public KeyGenActivity()
        {
            _Handler = new nfcHandler(HandlerHandleMessage);
        }
        class nfcHandler : Handler
        {
            Action<Message> handleMessage;
            public nfcHandler(Action<Message> handler)
            {
                this.handleMessage = handler;
            }
            public override void HandleMessage(Message msg)
            {
                handleMessage(msg);
            }
        }
        protected void HandlerHandleMessage(Message msg)
        {
            switch (msg.What)
            {
                case MESSAGE_SENT:
                    Toast.MakeText(this.ApplicationContext, "Transfered", ToastLength.Long).Show();
                    break;
            }
        }


        //SENSOR LISTENER STUFF
        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            //SensorStatus.Unreliable
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (!recording) return;
            EntropyManager.FeedData(e.Sensor.Type, e.Values);

            progStatus = (EntropyManager.GetSbSize() * 100) / EntropyManager.GetTotalSbSize();

            RunOnUiThread(() =>
            {
                progBar.IncrementProgressBy(progStatus - oldStatus);
            });
            oldStatus = progStatus;

            if (progBar.Progress >= 100)
            {
                finishedGen.Enabled = true;
                _sensorManager.UnregisterListener(this);
            }
        }
        private void registerSensors()
        {
            if (_sensorManager == null)
            {
                _sensorManager = (SensorManager)GetSystemService(SensorService);
            }

            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), delay);
            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Gyroscope), delay);
        }

    }
}