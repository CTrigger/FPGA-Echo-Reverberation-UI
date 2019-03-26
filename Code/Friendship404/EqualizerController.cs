using System;
using System.IO;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;
using Android.Content.PM;

using Java.Util;

namespace Friendship404
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait,Label = "EqualizerController",Icon = "@drawable/Equalizer")]
    public class EqualizerController : Activity
    {
 
        //View
        RadioButton Echo, Reverberation;
        Switch BluetoothConnection;
        SeekBar SBTimeBar;
        SeekBar SBIntensive;
        TextView txAtenuation;
        TextView txDistance;
        //ToggleButton tgConnect;
        //TextView Result;

        private string outsideData = "";
        private Java.Lang.String dataToSend;
        private BluetoothAdapter mBluetoothAdapter = null;
        private BluetoothSocket btSocket = null;
        private Stream outStream = null;
        private static string address = "00:21:13:02:32:0E";
        private static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        private Stream inStream = null;
        string resultText = "";

        int soundMode, soundAtenuation, soundDistance;
        char soundState;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Equalizer);

            // Create your application here
            Instance();
            CheckBt();
        }
        // Conversion tools
        private Java.Lang.String prepareData(char data)
        {
            return new Java.Lang.String(Convert.ToString(data));
        }

        // 1. Initialize the variables
        private void Instance()
        {
            Echo = (RadioButton)FindViewById(Resource.Id.rbtEcho);
            Reverberation = (RadioButton)FindViewById(Resource.Id.rbtReverberation);
            BluetoothConnection = (Switch)FindViewById(Resource.Id.switchConnect);
            SBTimeBar = (SeekBar)FindViewById(Resource.Id.sbTimeController);
            SBIntensive = (SeekBar)FindViewById(Resource.Id.sbIntensityController);
            txDistance = (TextView)FindViewById(Resource.Id.txDistance);
            txAtenuation = (TextView)FindViewById(Resource.Id.txAtenuation);

            BluetoothConnection.Checked = false;

            Echo.CheckedChange += Echo_CheckedChange;
            Reverberation.CheckedChange += Reverberation_CheckedChange;
            BluetoothConnection.CheckedChange += BluetoothConnection_CheckedChange;

            SBTimeBar.ProgressChanged += SBTimeBar_ProgressChanged;
            SBIntensive.ProgressChanged += SBIntensive_ProgressChanged;

        }

        //2. Connect/Disconnect
        private void BluetoothConnection_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (BluetoothConnection.Checked)
            {
                Connect();
                soundMode = 100;
                soundAtenuation = 10;
                soundDistance = 1;
                soundState = (char)(soundMode + soundAtenuation + soundDistance);
                dataToSend = prepareData(soundState);
                try
                {
                    writeData(dataToSend);
                }
                catch (Exception ex)
                {

                    Toast.MakeText(this, "Couldn't update the FPGA: " + ex.Message, ToastLength.Long).Show();
                }
                
            }
            else
            {
                try
                {
                    btSocket.Close();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Connect error: " + ex.Message, ToastLength.Long).Show();
                }

            }

        }

        // 2.1.  Must connect before stream data
        public void Connect()
        {
            //vincula dispositivo
            BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(address);
            Toast.MakeText(
                      this,
                      "Conexão em Andamento: " + device,
                      ToastLength.Short
                      ).Show();
            mBluetoothAdapter.CancelDiscovery();

            //Inicia socket de conexão
            try
            {
                btSocket = device.CreateRfcommSocketToServiceRecord(MY_UUID);
                btSocket.Connect();
                Toast.MakeText(
                      this,
                      "Conexão estabelecida",
                      ToastLength.Short
                      ).Show();

            }
            catch (Exception e)
            {
                Toast.MakeText(
                      this,
                      "Conexão não estabilecida \nERRO: " + e.Message,
                      ToastLength.Short
                      ).Show();

                //Caso conexão não seja possível estabelecer fecha conexão
                try
                {
                    btSocket.Close();
                }
                catch (Exception)
                {
                    Toast.MakeText(
                      this,
                      "Não é possível conectar",
                      ToastLength.Short
                      ).Show();
                    
                }

                Toast.MakeText(
                      this,
                      "Socket Encerrado",
                      ToastLength.Short
                      ).Show();
            }

        }

        // 3A. Event toogle, this will open conections send and receive
        private void Echo_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                SBTimeBar.Enabled = true;
                txAtenuation.Enabled = true;
                SBTimeBar.Progress = 0;
                SBIntensive.Enabled = true;
                txDistance.Enabled = true;
                SBIntensive.Progress = 0;

                soundMode = 100;
                soundAtenuation = 10;
                soundDistance = 1;
                soundState = (char)(soundMode + soundAtenuation + soundDistance);
                dataToSend = prepareData(soundState);
                writeData(dataToSend);

            }

        }

        // 3B. Event toogle, this will open conections send and receive
        private void Reverberation_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                SBTimeBar.Enabled = false;
                txDistance.Enabled = false;
                SBIntensive.Enabled = false;
                txAtenuation.Enabled = false;

        
                soundState = (char)(100);
                dataToSend = prepareData(soundState);
                writeData(dataToSend);

            }

        }

        private void SBTimeBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            soundDistance = e.Progress + 1;
            soundState = (char)(soundMode + soundAtenuation + soundDistance);
            dataToSend = prepareData(soundState);
            writeData(dataToSend);
            
        }

        private void SBIntensive_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            soundAtenuation = (e.Progress + 1) * 10;
            soundState = (char)(soundMode + soundAtenuation + soundDistance);
            dataToSend = prepareData(soundState);
            writeData(dataToSend);
        }

        // 3Z.1 Recebe Dados
        public void beginListenForData()
        {
            //Inicia socket de entrada 
            try
            {
                inStream = btSocket.InputStream;
            }
            catch (IOException ex)
            {
                Toast.MakeText(
                     this,
                     "Dados de entrada\nErro: " + ex.Message,
                     ToastLength.Short
                     ).Show();
            }

            //Inicia a Thread de Recepção
            Task.Factory.StartNew(() => {
                byte[] buffer = new byte[1024];
                int bytes;
                while (true)
                {
                    //Recebe os dados em Bytes
                    try
                    {
                        bytes = inStream.Read(buffer, 0, buffer.Length);
                        if (bytes > 0)
                        {
                            RunOnUiThread(() => {
                                string valor = System.Text.Encoding.ASCII.GetString(buffer);
                                outsideData = outsideData + "\n" + valor;
                            });
                        }
                    }

                    //Caso não haja mais dados para se receber encerra o evento de recepção
                    catch (Java.IO.IOException)
                    {
                        RunOnUiThread(() => {
                            outsideData = string.Empty;
                        });
                        break;
                    }
                }
            });
        }

        //3X.1. Envia Dados
        private void writeData(Java.Lang.String data)
        {
            //inicia socket de envio
            try
            {
                outStream = btSocket.OutputStream;
            }
            catch (Exception e)
            {
                Toast.MakeText(
                     this,
                     "Socket de saida\nErro: " + e.Message,
                     ToastLength.Short
                     ).Show();
            }

            Java.Lang.String message = data;

            byte[] msgBuffer = message.GetBytes();

            try
            {
                outStream.Write(msgBuffer, 0, msgBuffer.Length);
            }
            catch (Exception e)
            {
                Toast.MakeText(
                     this,
                     "Envio de dados\nErro: " + e.Message,
                     ToastLength.Short
                     ).Show();
            }
        }

        // 4. Check Bluetooth Status
        private void CheckBt()
        {
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (!mBluetoothAdapter.IsEnabled)
            {
                Toast.MakeText(this, "Bluetooth is desactivated",
                    ToastLength.Short).Show();
            }

            if (mBluetoothAdapter == null)
            {
                Toast.MakeText(this,
                    "Device not compatible with Bluetooth Adapter", ToastLength.Short)
                    .Show();
            }
        }

    }
}