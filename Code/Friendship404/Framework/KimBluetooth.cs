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

using Java.Util;

namespace Friendship404.Framework
{
    class KimBluetooth : Activity
    {
        private BluetoothAdapter mBluetoothAdapter = null;
        private BluetoothSocket btSocket = null;

        private Stream outStream = null;
        private Stream inStream = null;

        private Java.Lang.String dataToSend;
        private static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

        private string outsideData = "";
        public static string address = "00:21:13:02:32:0E";
        public string resultText = "";

        private bool ConnectionStats = false;

        public void ConnectionStart()
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

                ConnectionStats = true;

            }
            catch (Exception e)
            {
                Toast.MakeText(
                      this,
                      "Conexão não estabilecida \nERRO: " + e.Message,
                      ToastLength.Short
                      ).Show();
            }

        }

        public void ConnectionEnd()
        {
            try
            {
                btSocket.Close();
                ConnectionStats = false;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Connect error: " + ex.Message, ToastLength.Long).Show();
            }
        }

        private void DataByteSend(Java.Lang.String data)
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

        public void DataByteReceive()
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
                                outsideData = outsideData + valor;
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

    }
}