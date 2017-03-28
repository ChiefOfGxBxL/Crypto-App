using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Hardware;
using System.IO;
using System.Threading;
using Android.Nfc;
using System.Text;

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
        List<string> fileName = new List<string>();
        Android.Widget.ProgressBar progBar;
        TextView text;
        Button finishedGen;
        Button startGen;
        int progStatus = 0;
        int oldStatus = 0;
        bool recording;
        string contactName; // which contact we are generating the pad for

        const Int32 MESSAGE_SENT = 1;
        public NfcAdapter _nfcAdapter;
        private const string Mime = "padbook/nfc";
        private readonly nfcHandler _Handler;
        private string key;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.KeyGen);
            
            
            text = FindViewById<TextView>(Resource.Id.textView1);
            progBar = FindViewById<ProgressBar>(Resource.Id.progBar);
            finishedGen = FindViewById<Button>(Resource.Id.button1);
            startGen = FindViewById<Button>(Resource.Id.button2);
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
            // Get the contact name passed in through the intent when ViewContactActivity launches this intent
            contactName = Intent.GetStringExtra("contactName"); // stored in key "contactName"

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

        protected override void OnPause()
        {
            base.OnPause();

            if (progBar.Progress < 100)
                _sensorManager.UnregisterListener(this); // unregisters all sensors
        }
        

        protected override void OnResume()
        {
            base.OnResume();
            if (progBar.Progress < 100 && _sensorManager == null)
                registerSensors(); //reset sensors on resume

            if (NfcAdapter.ActionNdefDiscovered == Intent.Action)
            {
                ProcessIntent(Intent);
            }
        }
        protected override void OnNewIntent(Intent intent)
        {
            Intent = intent;
        }

        void ProcessIntent(Intent intent)
        {
            IParcelable[] rawMsg = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
            NdefMessage msg = (NdefMessage)rawMsg[0];
            text.Text = (Encoding.UTF8.GetString(msg.GetRecords()[0].GetPayload()));

        }
        public NdefMessage CreateNdefMessage(NfcEvent evt)
        {
            NdefRecord mimeRec = new NdefRecord(
                NdefRecord.TnfMimeMedia, Encoding.UTF8.GetBytes(Mime),
                new byte[0], Encoding.UTF8.GetBytes(key));

            NdefMessage msg = new NdefMessage(new NdefRecord[] { mimeRec });
            return msg;
        }

        public void OnNdefPushComplete(NfcEvent evt)
        {
            _Handler.ObtainMessage(MESSAGE_SENT).SendToTarget();

        }
        //NFC STUFF
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
        private void StartNfc_Click(object sender, EventArgs e)
        {
            key = EntropyManager.GetBlockOfEntropyBytes();
            _nfcAdapter.SetNdefPushMessageCallback(this, this);
            _nfcAdapter.SetOnNdefPushCompleteCallback(this, this);
            text.Text = "Move Your Phone Near Your Friend's";

        }

        //LISTENER STUFF
        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            //SensorStatus.Unreliable
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (!recording) return;
            EntropyManager.FeedData(e.Sensor.Type, e.Values);
            
            progStatus = (EntropyManager.GetSbSize() * 100 )/ EntropyManager.GetTotalSbSize();

            RunOnUiThread(() =>
            {
                progBar.IncrementProgressBy(progStatus-oldStatus);
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