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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewContact);

            // Get passed information
            string[] splitName = Intent.GetStringExtra("contact").Split('/');
            contactName = splitName[splitName.Length - 1];
            Toast.MakeText(ApplicationContext, contactName, ToastLength.Long).Show();

            // Set the global UI variables here
            getMessagesBtn = FindViewById<Button>(Resource.Id.button1);
            generateEntropyBtn = FindViewById<Button>(Resource.Id.button2);
            messageText = FindViewById<TextView>(Resource.Id.textView1);
            sendMessageText = FindViewById<EditText>(Resource.Id.editText1);
            sendMessageBtn = FindViewById<Button>(Resource.Id.button3);

            getMessagesBtn.Click += GetMessagesBtn_Click;
            sendMessageBtn.Click += SendMessageBtn_Click;

            // Instantiate the WebClient we will be using to make requests to the API
            wc = new WebClient();
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

        /// <summary>
        /// Retrieves a message from the server, and if it exists decrypts and displays it
        /// </summary>
        private string RetrieveMessage()
        {
            // endpoint: GET /messages/:to/:from; in this case, we want to find a message to me, from the user we are viewing
            string url = "http://safe-inlet-16663.herokuapp.com/messages/" + Identity.Username + "/" + contactName;
            string msg = wc.DownloadString(url);

            // Debug:
            Toast.MakeText(ApplicationContext, url, ToastLength.Long).Show();

            // TODO: decrypt message using corresponding pad
            var decryptedMsg = msg; // Decrypt here..

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

            string encryptedMsg = msg; // TODO: encrypt the message here using the pad

            string url = string.Format(
                "http://safe-inlet-16663.herokuapp.com/messages/new?to={0}&from={1}&content={2}",
                contactName, // to this contact
                Identity.Username, // from current user
                encryptedMsg // message to send along
            );

            // Debug:
            Toast.MakeText(ApplicationContext, url, ToastLength.Long).Show();

            string response = "";//wc.DownloadString(url); // message success will have an empty reponse body

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