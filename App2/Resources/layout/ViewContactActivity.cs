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

namespace App2.Resources.layout
{
    [Activity(Label = "ViewContactActivity")]
    public class ViewContactActivity : Activity
    {
        Button getMessagesBtn;
        Button generateEntropyBtn;
        TextView messageText;
        EditText sendMessageText;
        Button sendMessageBtn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewContact);

            // Set the global UI variables here
            getMessagesBtn = FindViewById<Button>(Resource.Id.button1);
            generateEntropyBtn = FindViewById<Button>(Resource.Id.button2);
            messageText = FindViewById<TextView>(Resource.Id.textView1);
            sendMessageText = FindViewById<EditText>(Resource.Id.editText1);
            sendMessageBtn = FindViewById<Button>(Resource.Id.button3);


        }
    }
}