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
    [Activity(Label ="Nfc"), IntentFilter(new[] { "android.nfc.action.NDEF_DISCOVERED" })]
    public class NfcActivity : Activity
    {
        public NfcAdapter _nfcAdapter;
        private bool _inWriteMode;
        private TextView _text;
        private Button _writeBtn;
        private EditText _userInput;
        private const string Mime = "padbook/nfc";
       
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Nfc);
            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
            _userInput = FindViewById<EditText>(Resource.Id.editText1);
            _writeBtn = FindViewById<Button>(Resource.Id.button1);
            _text = FindViewById<TextView>(Resource.Id.textView1);

            _writeBtn.Click += WriteToTag;
            _text.Text = "No message";
            

        }

        private void WriteToTag(object sender, EventArgs args)
        {
            var view = (View)sender;
            if (view.Id == Resource.Id.button1)
            {
                var alert = new AlertDialog.Builder(this).Create();
                alert.SetMessage("Hold phones near each other");
                alert.SetTitle("NFC starting");
                alert.SetButton("OK", delegate
                {
                    EnableNfcWrite();
                });
                alert.Show();
            }
        }


        protected override void OnPause()
        {
            base.OnPause();
            if (_nfcAdapter != null)
            {
                _nfcAdapter.DisableForegroundDispatch(this);
            }
        }

        private String ParseUriRecord(NdefRecord rec) {
            byte[] payload = rec.GetPayload();
            return Encoding.ASCII.GetString(payload);
        }

        protected override void OnNewIntent(Intent intent)
        {
            var intentType = intent.Type ?? String.Empty;
            if (Mime.Equals(intentType) && NfcAdapter.ActionNdefDiscovered.Equals(intent.Action))
            {
                var rawMessage = Intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
                var msg = (NdefMessage)rawMessage[0];
                var record = msg.GetRecords()[0];
                var message = Encoding.ASCII.GetString(record.GetPayload());
                _text.Text = message;
                return;
            }
            Android.Util.Log.Debug("NFC: ", "Sending message");
            if (_inWriteMode)
            {
                _inWriteMode = false;
                var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;

                if (tag == null)
                {
                    return;
                }
                var payload = Encoding.ASCII.GetBytes(_userInput.Text.ToString());
                var mimeBytes = Encoding.ASCII.GetBytes(Mime);
                var appRecord = new NdefRecord(NdefRecord.TnfMimeMedia, mimeBytes, new byte[0], payload);
                var ndefMessage = new NdefMessage(new[] { appRecord });

                if (!tryWriteToTag(tag, ndefMessage))
                {
                    Android.Util.Log.Debug("writing: ", "cant write to tag trying to format");
                    tryFormatTag(tag, ndefMessage);
                }
            }
        }

        /// <summary>
        /// Sets up nfc to be able to write a message
        /// Shows a dialog if nfc isn't initialized or available
        /// </summary>
        public void EnableNfcWrite()
        {
            _inWriteMode = true;
            var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
           Android.Content.IntentFilter[] filters = { tagDetected };

            var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop);
            var pendingIntent = PendingIntent.GetActivity(this,    0, intent, 0);

            //notify user if nfc doesn't work
            if (_nfcAdapter == null)
            {
                var alert = new AlertDialog.Builder(this).Create();
                alert.SetMessage("NFC not supported or turned off on this device");
                alert.SetTitle("NFC unavailable");
                alert.SetButton("OK", delegate
                {
                    _inWriteMode = false;
                });
                alert.Show();
            }else
            {
                Android.Util.Log.Debug("Create: ", "Making intent for nfc");
                _nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
            }

        }


        private bool tryFormatTag(Tag tag, NdefMessage message)
        {
            var format = NdefFormatable.Get(tag);
            if (format == null)
            {
                var alert = new AlertDialog.Builder(this).Create();
                alert.SetMessage("Tag doesn't support NDEF format");
                alert.SetTitle("NDEF format not supported");
                alert.SetButton("OK", delegate
                {
                });
                alert.Show();
                return false;
            }
            else
            {
                try {
                    format.Connect();
                    format.Format(message);
                    return true;
                }catch (IOException ex)
                {
                    Android.Util.Log.Error("nfc: ", ex.ToString());
                    return false;
                }
                
            }
        }
        private bool tryWriteToTag(Tag tag, NdefMessage message)
        {
            var nDef = Ndef.Get(tag);
            if (nDef != null)
            {
                nDef.Connect();

                if (!nDef.IsWritable)
                {
                    //DisplayMessage("Tag is read-only");
                    //Tag is read only
                }

                int size = message.ToByteArray().Length;
                if (nDef.MaxSize < size)
                {
                    //DisplayMessage("Tag is too big");
                    //too big to write
                }

                nDef.WriteNdefMessage(message);
                nDef.Close();
                return true;
            }
            return false;
        }

    }
}