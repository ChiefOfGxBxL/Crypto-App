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

namespace App2
{
    [Activity(Label = "Contacts")]
    public class ContactsListActivity : ListActivity
    {
        ListView _listView;
        List<string> items;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ContactsList);

            _listView = FindViewById<ListView>(Resource.Id.listView1);

            // Create a new contact
            ContactBook cb = new ContactBook(GetExternalFilesDir(null).ToString());

            // Create your application here
            items = cb.GetContacts();

            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, items);
            _listView.Adapter = adapter;

            // Any additional contacts should go through the adapter
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var t = items[position];

            // View this contact by creating a new intent and starting the activity

        }
    }
}