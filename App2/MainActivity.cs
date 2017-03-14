using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Runtime;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Android.Content;
using System.Threading.Tasks;

namespace App2
{
    [Activity(Label = "Crypto App", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        
        Button _btn2;
        EditText _setNameText;
        Button _setNameBtn;

        private void initializeUIComponents()
        {
            _btn2 = FindViewById<Button>(Resource.Id.button2);
            _setNameText = FindViewById<EditText>(Resource.Id.editText1);
            _setNameBtn = FindViewById<Button>(Resource.Id.button3);

            _btn2.Click += _btn2_Click;
            _setNameBtn.Click += _setNameBtn_Click;
        }

        private void _setNameBtn_Click(object sender, EventArgs e)
        {
            Identity.SetUsername(GetExternalFilesDir(null).ToString(), _setNameText.Text);

            // Notify user of success
            Toast.MakeText(this.ApplicationContext, "Set username!", ToastLength.Long).Show();
        }

        private void _btn2_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(ContactsListActivity));
            StartActivity(intent);
        }



        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            await Task.Delay(10);

            initializeUIComponents();
            Identity.LoadUsername(GetExternalFilesDir(null).ToString());

            // Set username textbox
            _setNameText.Text = Identity.Username;
        }

        protected override void OnPause()
        {
            base.OnPause();
            
        }

        protected async override void OnResume()
        {
            base.OnResume();

            // The await is magically needed for the app to work on my emulator
            // Otherwise I get a System.NullReferenceException in OnResume or OnCreate
            // See https://forums.xamarin.com/discussion/comment/219774/#Comment_219774
            await Task.Delay(10);
        }


    }
}

