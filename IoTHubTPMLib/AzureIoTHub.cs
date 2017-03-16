using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Devices.Tpm;
using Microsoft.Azure.Devices.Client;
//using Newtonsoft;
using Newtonsoft.Json.Linq;

namespace IoTHubTPMLib
{
    public static class AzureIoTHub
    {
        //If Not Using TPM:
        public static string DeviceConnectionString { get; set; } =
            "HostName=<HostName.usr.azure-devices.net;DeviceId=MyDevice;SharedAccessKey=XXXXXX";

        //
        // This sample assumes the device has been connected to Azure with the IoT Dashboard
        //
        // Refer to http://aka.ms/azure-iot-hub-vs-cs-wiki for more information on Connected Service for Azure IoT Hub




        public static async Task SendDeviceToCloudMessageAsyncNotTPM(string msg)
        {
            ////Not using TPM
            var deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Amqp);

            var message = new Message(Encoding.ASCII.GetBytes(msg));

            await deviceClient.SendEventAsync(message);
        }

        public static async Task<string> ReceiveCloudToDeviceMessageAsyncNotTPM()
        {
            
            var deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Amqp);

            while (true)
            {
                var receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    await deviceClient.CompleteAsync(receivedMessage);
                    return messageData;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public static async Task SendDeviceToCloudMessageAsyncUseTPM(string msg)
        {
            TpmDevice myDevice = new TpmDevice(0); // Use logical device 0 on the TPM
            string hubUri = myDevice.GetHostName();
            string deviceId = myDevice.GetDeviceId();
            string sasToken = myDevice.GetSASToken();

            var deviceClient = DeviceClient.Create(
                hubUri,
                AuthenticationMethodFactory.
                    CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);

            //Not using TPM
            //var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);

            var message = new Message(Encoding.ASCII.GetBytes(msg));

            await deviceClient.SendEventAsync(message);
        }

        public static async Task<string> ReceiveCloudToDeviceMessageAsyncUseTPM()
        {
            TpmDevice myDevice = new TpmDevice(0); // Use logical device 0 on the TPM by default
            string hubUri = myDevice.GetHostName();
            string deviceId = myDevice.GetDeviceId();
            string sasToken = myDevice.GetSASToken();

            //Use TPM
            var deviceClient = DeviceClient.Create(
                hubUri,
                AuthenticationMethodFactory.
                    CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);

            //Not using TPM
            //var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);

            while (true)
            {
                var receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    await deviceClient.CompleteAsync(receivedMessage);
                    return messageData;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

//#if LESSON4
        public static async Task<Tuple<string,int>> ReceiveCloudToDeviceCommandAsync()
        {
            string msg = await ReceiveCloudToDeviceMessageAsyncUseTPM();
            JObject o = JObject.Parse(msg);
            string cmd = (string)o["cmd"];
            int val = (int) o["val"];
            return Tuple.Create(cmd, val);

        }
//#endif

    }
}


