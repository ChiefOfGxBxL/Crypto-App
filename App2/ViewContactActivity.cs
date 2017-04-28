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
using System.Net;
using System.Security.Cryptography;
using System.IO;

namespace App2
{
    [Activity(Label = "View Contact")]
    public class ViewContactActivity : Activity
    {
        Button getMessagesBtn;
        Button generateEntropyBtn;
        TextView messageText;
        EditText sendMessageText;
        Button sendMessageBtn;

        string contactName; // stores which user we are currently viewing
        WebClient wc; // for accessing API endpoints (retrieving and sending messages)

        PadManager pm;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewContact);

            // Get passed information
            string[] splitName = Intent.GetStringExtra("contact").Split('/');
            contactName = splitName[splitName.Length - 1];
            //Toast.MakeText(ApplicationContext, contactName, ToastLength.Long).Show();

            // Set the global UI variables here
            getMessagesBtn = FindViewById<Button>(Resource.Id.button1);
            generateEntropyBtn = FindViewById<Button>(Resource.Id.button2);
            messageText = FindViewById<TextView>(Resource.Id.textView1);
            sendMessageText = FindViewById<EditText>(Resource.Id.editText1);
            sendMessageBtn = FindViewById<Button>(Resource.Id.button3);

            getMessagesBtn.Click += GetMessagesBtn_Click;
            sendMessageBtn.Click += SendMessageBtn_Click;
            generateEntropyBtn.Click += GenKeyBtn_Click;

            // Instantiate the WebClient we will be using to make requests to the API
            wc = new WebClient();

            // Initialize pad manager
            pm = new PadManager(GetExternalFilesDir(null).ToString());
            Toast.MakeText(ApplicationContext, contactName, ToastLength.Long).Show();
        }

        private void SendMessageBtn_Click(object sender, EventArgs e)
        {
            bool msgSent = SendMessage(sendMessageText.Text);
            if (!msgSent)
            {
                Toast.MakeText(ApplicationContext, "Error sending message", ToastLength.Long).Show();
            }
        }

        private void GetMessagesBtn_Click(object sender, EventArgs e)
        {
            messageText.Text = RetrieveMessage();
        }

        private void GenKeyBtn_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(KeyGenActivity));
            intent.PutExtra("contactName", contactName);
            StartActivity(intent);
        }

        /// <summary>
        /// Retrieves a message from the server, and if it exists decrypts and displays it
        /// </summary>
        private string RetrieveMessage()
        {
            // endpoint: GET /messages/:to/:from; in this case, we want to find a message to me, from the user we are viewing
            string url = "http://safe-inlet-16663.herokuapp.com/messages/" + Identity.Username + "/" + contactName;
            string msg = wc.DownloadString(url);

            // Debug:
            //Toast.MakeText(ApplicationContext, url, ToastLength.Long).Show();

            // Decrypt message by using pad from padbook
            // If no pad exists, returns empty string
            Padbook pb = pm.GetPadbookForUsername(contactName);
            long pbSize = pb.BytesLeft();
            AesManaged aes = new AesManaged();
            KeySizes[] ks = aes.LegalKeySizes;
            string decryptedMsg = "";
            if (pbSize >= (aes.LegalKeySizes[0].MaxSize + aes.LegalBlockSizes[0].MaxSize/8) * 2)
            { 
                string pad = pb.GetNextPad();
            

                if (pad != null && pad.Length > 0)
                    decryptedMsg = Encoding.UTF8.GetString(Crypto.XorStrings(msg, pad)); // Decrypt message using pad
            }
            else // use AES
            {
                List<byte> key = new List<byte>();
                List<byte> IV = new List<byte>();
                List<string> pads = new List<string>();
                while (key.Count * sizeof(byte) < 128)
                {
                    pads.Add(pb.GetNextPad());
                    byte[] f = Encoding.UTF8.GetBytes(pads.Last());
                    foreach (byte b in f)
                    {
                        key.Add(b);
                    }
                }
                while (IV.Count * sizeof(byte) < 128 / 8)
                {
                    pads.Add(pb.GetNextPad());
                    byte[] f = Encoding.UTF8.GetBytes(pads.Last());
                    foreach (byte b in f)
                    {
                        IV.Add(b);
                    }
                }
                //resave the pads so AES can be used again
                pb.AppendPads(pads.ToArray());
                aes.Key = key.ToArray();
                aes.IV = IV.ToArray();
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Encoding.UTF8.GetBytes(msg)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            decryptedMsg = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return decryptedMsg;
        }

        /// <summary>
        /// Sends the specified (unencrypted) message to the user we are currently viewing
        /// </summary>
        /// <param name="msg">Unencrypted string to send</param>
        /// <returns>Boolean: true if successful, false otherwise</returns>
        private bool SendMessage(string msg)
        {
            if (msg == null || msg.Length == 0) return false; // must have a message to send

            // Encrypt message using pad
            Padbook pb = pm.GetPadbookForUsername(contactName);
            long pbSize = pb.BytesLeft();
            AesManaged aes = new AesManaged();
            KeySizes[] ks = aes.LegalKeySizes;
            string encryptedMsg;
            if (pbSize >= (aes.LegalKeySizes[0].MaxSize + aes.LegalBlockSizes[0].MaxSize / 8) * 2)
            {
                Toast.MakeText(ApplicationContext, "Using Pads", ToastLength.Long).Show();
                string pad = pb.GetNextPad();
                encryptedMsg = msg;
                encryptedMsg = Encoding.UTF8.GetString(Crypto.XorStrings(msg, pad));
            }
            else // Use AES
            {
                Toast.MakeText(ApplicationContext, "Using AES", ToastLength.Long).Show();
                List<byte> key = new List<byte>();
                List<byte> IV = new List<byte>();
                List<string> pads = new List<string>();
                while (key.Count * sizeof(byte) < 128)
                {
                    pads.Add(pb.GetNextPad());
                    byte[] f = Encoding.UTF8.GetBytes(pads.Last());
                    foreach (byte b in f)
                    {
                        key.Add(b);
                    }
                }
                while (IV.Count * sizeof(byte) < 128 / 8)
                {
                    pads.Add(pb.GetNextPad());
                    byte[] f = Encoding.UTF8.GetBytes(pads.Last());
                    foreach (byte b in f)
                    {
                        IV.Add(b);
                    }
                }
                //resave the pads so AES can be used again
                pb.AppendPads(pads.ToArray());
                aes.Key = key.ToArray();
                aes.IV = IV.ToArray();
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            swEncrypt.Write(msg);
                        }
                        encryptedMsg = Encoding.UTF8.GetString(msEncrypt.ToArray());
                    }
                }
            }

            string url = string.Format(
                "http://safe-inlet-16663.herokuapp.com/messages/new?to={0}&from={1}&content={2}",
                contactName, // to this contact
                Identity.Username, // from current user
                encryptedMsg // message to send along
            );

            // Debug:
            Toast.MakeText(ApplicationContext, url, ToastLength.Long).Show();

            string response = wc.DownloadString(url); // message success will have an empty reponse body

            if (response == null || response == "")
            {
                // Success!
                return true;
            }
            else
            {
                // Error sending message
                Toast.MakeText(ApplicationContext, "Error sending message", ToastLength.Long).Show();
                return false;
            }
        }
    }
}