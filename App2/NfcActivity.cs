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
using Android.Nfc;
using Android.Nfc.Tech;
using System.IO;

namespace App2
{
    [Activity(Label = "Nfc", LaunchMode = Android.Content.PM.LaunchMode.SingleTop),
     IntentFilter(new[] { "android.nfc.action.NDEF_DISCOVERED" },
        Categories =new[] { "android.intent.category.DEFAULT" },
        DataMimeType ="padbook/nfc" )]

    public class NfcActivity : Activity, NfcAdapter.ICreateNdefMessageCallback, NfcAdapter.IOnNdefPushCompleteCallback
    {
        const Int32 MESSAGE_SENT = 1;
        public NfcAdapter _nfcAdapter;
        private TextView _text;
        private Button _writeBtn;
        private EditText _userInput;
        private const string Mime = "padbook/nfc";
        private readonly nfcHandler _Handler;

        //needed for callbacks
        class nfcHandler : Handler
        {
            Action<Message> handleMessage;
            public nfcHandler (Action<Message> handler)
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

        public NfcActivity()
        {
            _Handler = new nfcHandler(HandlerHandleMessage);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Nfc);

            _userInput = FindViewById<EditText>(Resource.Id.editText1);
            _writeBtn = FindViewById<Button>(Resource.Id.button1);
            _text = FindViewById<TextView>(Resource.Id.textView1);

            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            if (_nfcAdapter == null)
            {
                _text.Text = "NFC not available";
            }
            else
            {

                _writeBtn.Click += _writeBtn_Click;
                _text.Text = "No message";
            }

        }

        private void _writeBtn_Click(object sender, EventArgs e)
        {
            _nfcAdapter.SetNdefPushMessageCallback(this, this);
            _nfcAdapter.SetOnNdefPushCompleteCallback(this, this);
            
        }

        protected override void OnResume()
        {
            base.OnResume();

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
            _text.Text = Encoding.UTF8.GetString(msg.GetRecords()[0].GetPayload());
        }

        public NdefMessage CreateNdefMessage(NfcEvent evt)
        {
            NdefRecord mimeRec = new NdefRecord(
                NdefRecord.TnfMimeMedia, Encoding.UTF8.GetBytes(Mime),
                new byte[0], Encoding.UTF8.GetBytes(_userInput.Text));

            NdefMessage msg = new NdefMessage(new NdefRecord[] { mimeRec });
            return msg;
        }

        public void OnNdefPushComplete(NfcEvent evt)
        {
            _Handler.ObtainMessage( MESSAGE_SENT ).SendToTarget();
        }


    }
}