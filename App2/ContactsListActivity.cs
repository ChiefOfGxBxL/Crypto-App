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
using System.IO;
using System.Threading.Tasks;

namespace App2
{
    [Activity(Label = "Contacts")]
    public class ContactsListActivity : Activity
    {
        ListView _listView;
        EditText _newContactTxt;
        Button _newContactBtn;

        List<string> items;
        ArrayAdapter adapter;

        ContactBook cb;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ContactsList);
            await Task.Delay(10);

            _listView = FindViewById<ListView>(Resource.Id.listView1);
            _newContactTxt = FindViewById<EditText>(Resource.Id.editText1);
            _newContactBtn = FindViewById<Button>(Resource.Id.button1);

            _newContactBtn.Click += _newContactBtn_Click;

            // Create a new contact
            cb = new ContactBook(GetExternalFilesDir(null).ToString());

            // Create your application here
            items = cb.GetContacts();

            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, items);
            _listView.Adapter = adapter;
            _listView.ItemClick += contactListViewItemClick;

            // Any additional contacts should go through the adapter
        }

        private void _newContactBtn_Click(object sender, EventArgs e)
        {
            // Add to contacts list
            cb.AddContact(_newContactTxt.Text);

            items.Add(_newContactTxt.Text);
            adapter.NotifyDataSetChanged(); // refresh the listview

            // Clear textbox
            _newContactTxt.Text = "";

            // Notify user of success
            Toast.MakeText(ApplicationContext, "Added new contact!", ToastLength.Long).Show();
        }

        private void contactListViewItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // View this contact by creating a new intent and starting the activity
            var contactIntent = new Intent(this, typeof(ViewContactActivity));
            contactIntent.PutExtra("contact", items[e.Position].ToString()); // pass along the name of the contact
            StartActivity(contactIntent);
        }
    }
}