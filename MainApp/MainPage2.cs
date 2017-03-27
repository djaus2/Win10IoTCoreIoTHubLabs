using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
//using IoTHubTPMLib;
//using Sensors.Dht;

namespace Win10IoTCoreIoTHubLabs
{
    public sealed partial class MainPage : Page
    {
#if (!LESSON1_1) && (!LESSON2) && (!LESSON1_0)
        private async Task Send(int num)
        {
            string jsn =  "{\"num\":" + num.ToString() + "}";
            await Send(jsn);
        }

        private async Task Send(string msg)
        {
            string dt = DateTime.Now.ToString();
            string jsn = "{\"Time\":\"" + dt + "\"," + msg.Substring(1);
            Debug.WriteLine("Sending: " + jsn);
            await AzureIoTHub.SendDeviceToCloudMessageAsyncUseTPM(msg);
        }

        private async Task Recv()
        {
            string reply = await GetMsgs();
            Debug.WriteLine("Recvd: " + reply);
        }

        private async Task<string> GetMsgs()
        {
            string msg = await AzureIoTHub.ReceiveCloudToDeviceMessageAsyncUseTPM();
            return msg;
        }
#endif

#if LESSON6 || LESSON7
#if LESSON6
        private  async Task doCommand()
        {
            Tuple < string,int> command = await GetCommand();
            string cmd = command.Item1;
            int val = command.Item2;
#elif LESSON7
         private  async Task doCommand2()
        {
            Tuple < string,int> command = await GetCommand();
            string cmd = command.Item1;
            int val = command.Item2;
#endif


            switch (cmd)
            {
                case "ledOn":
                    IoTGPIO.LEDOn();
                    break;
                case "ledOff":
                    IoTGPIO.LEDOff();
                    break;
                case "ledFlash":
                    if (val >0)
                        await IoTGPIO.LEDFlash(val);
                    break;
                case "buzzdOn":
                    break;
                case "buzzOff":
                    break;
                default:
                    Debug.WriteLine("Invalid comamnd: " + cmd);
                    break;
            }
        }

        public List<string> commands = new List<string> { "ledOn", "ledOff", "ledBlink", "buzzOn", "buzzOff" };

        private async Task<Tuple<string, int>> GetCommand()
        {
            var command = await AzureIoTHub.ReceiveCloudToDeviceCommandAsync();
            return command;
        }

#elif LESSON8
        public async Task ReadTemp()
        {
            await ReadTempHumidity();
            DhtReading reading = Reading;
            Debug.WriteLine("Temperature: " + reading.Temperature + " C, " +  "Humidity: " + reading.Humidity + ", IsValid: " + reading.IsValid +", TimedOut: " + reading.TimedOut +", RetryCount: " + reading.RetryCount);
        }
#elif LESSON9
        public async Task SendTemp()
        {
            await ReadTempHumidity();
            string msg = GetTempReadingsAsJSon();
            await Send(msg);
        }
#endif
    }
}
