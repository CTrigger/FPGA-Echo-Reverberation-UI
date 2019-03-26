using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;
using Android.Content.PM;

namespace Friendship404
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Label = "BluetoothController", Icon = "@drawable/Bluetooth")]
    public class BluetoothController : Activity, Android.Views.View.IOnClickListener
    {
        Button  listBtn, change_Bluetooth_Name, display_Name;
        //Button onBtn, offBtn,visibleBtn;
        BluetoothAdapter blue;              // Bluetooth adapter class variable

        ListView list_Of_Devices;           // list view for paired devices
        EditText bluetoothName;            // user bluetooth name edit text

        Switch swBluetoothController;
        Switch swBluetoothDiscovery;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BluetoothControllers);

            // Create your application here
            blue = BluetoothAdapter.DefaultAdapter;
            initialize();
        }

        private void initialize()
        {
            //onBtn = (Button)FindViewById(Resource.Id.onB);
            //offBtn = (Button)FindViewById(Resource.Id.offB);
            //visibleBtn = (Button)FindViewById(Resource.Id.visibleD);
            listBtn = (Button)FindViewById(Resource.Id.pairD);
            bluetoothName = (EditText)FindViewById(Resource.Id.name);
            change_Bluetooth_Name = (Button)FindViewById(Resource.Id.nameBtn);
            display_Name = (Button)FindViewById(Resource.Id.showName);
            list_Of_Devices = (ListView)FindViewById(Resource.Id.list_devices);

            //switch
            swBluetoothController = (Switch)FindViewById(Resource.Id.swBluetoothController);
            swBluetoothDiscovery = (Switch)FindViewById(Resource.Id.swBlutoothDicovery);

            if (blue.IsEnabled)
            {
                swBluetoothController.Checked = true;
                if (blue.IsDiscovering == true)
                {
                    swBluetoothDiscovery.Checked = true;
                }
                else
                {
                    swBluetoothDiscovery.Checked = false;
                }
            }
            else
            {
                swBluetoothController.Checked = false;
                swBluetoothDiscovery.Checked = false;
            }

            //onBtn.SetOnClickListener(this);
            //offBtn.SetOnClickListener(this);
            //visibleBtn.SetOnClickListener(this);
            listBtn.SetOnClickListener(this);
            change_Bluetooth_Name.SetOnClickListener(this);
            display_Name.SetOnClickListener(this);
            //switch
            swBluetoothController.SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.swBluetoothController:
                    if (blue.IsEnabled == false)
                    {
                        //Intent onBlue = new Intent(BluetoothAdapter.ActionRequestEnable);
                        //StartActivityForResult(onBlue, 0);
                        blue.Enable();
                     
                        Toast.MakeText(this, "Bluetooth Enabled", ToastLength.Short).Show();

                    }
                    else
                    {
                        try
                        {
                            blue.Disable();
                        }
                        catch (Exception e)
                        {
                            Toast.MakeText(this, e.Message, ToastLength.Short).Show();
                            throw;
                        }
                        
                        Toast.MakeText(this, "Bluetooth Disabled", ToastLength.Short).Show();

                    }

                    break;

                case Resource.Id.swBlutoothDicovery:
                    if (blue.IsDiscovering == false)
                    {
                        //Intent discovery = new Intent(BluetoothAdapter.ActionRequestDiscoverable);
                        //StartActivityForResult(discovery, 0);
                        blue.StartDiscovery();
                        Toast.MakeText(this, "Is Visible", ToastLength.Short).Show();
                        //swBluetoothDiscovery.Checked = true;
                    }
                    else
                    {
                        blue.CancelDiscovery();
                        Toast.MakeText(this, "Not Visible", ToastLength.Short).Show();
                        //swBluetoothDiscovery.Checked = false;
                    }

                    break;


                case Resource.Id.pairD:

                    ArrayList list = new ArrayList();
                    foreach (BluetoothDevice bt in blue.BondedDevices)
                    {
                        list.Add(bt.Name);
                    }

                    ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, list);
                    list_Of_Devices.SetAdapter(adapter);
                    break;

                case Resource.Id.nameBtn:
                    if (!bluetoothName.Text.ToString().Equals(""))
                    {
                        String n = bluetoothName.Text.ToString();
                        blue.SetName(n);
                    }
                    else
                    {
                        Toast.MakeText(this, "Please enter name", ToastLength.Short).Show();
                    }
                    break;

                case Resource.Id.showName:
                    Toast.MakeText(this, blue.Name.ToString(), ToastLength.Short).Show();
                    break;
            }
        }
    }
}