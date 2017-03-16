using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using IoTHubTPMLib;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TesterApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            textBoxRecv.Text = "";
            SendIsRunning = false;
            RecvIsRunning = false;
        }

        private async void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            await AzureIoTHub.SendDeviceToCloudMessageAsyncNotTPM(textBoxSend.Text);
        }

        int count = 0;
        private async void buttonRecv_Click(object sender, RoutedEventArgs e)
        {
            await GetMsgs();
        }

        private async Task GetMsgs()
        {
            string msg = await AzureIoTHub.ReceiveCloudToDeviceMessageAsyncNotTPM();
            await DisplayText(msg);
        }


        private async Task DisplayText(string msg)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                textBoxRecv.Text = (count++).ToString() +" " + msg + "\r\n" + textBoxRecv.Text;
            });
        }

        private async Task DisplayText2(string msg)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                this.textBoxSend.Text = msg;
            });
        }

        private bool RecvIsRunning = false;
        private async void buttonRun_Click(object sender, RoutedEventArgs e)
        {
            count = 0;
            RecvIsRunning = !RecvIsRunning;
            while (RecvIsRunning)
            {
                await Task.Run(async () =>
                {
                    await DisplayText("Receiver is running.");
                    while (RecvIsRunning)
                        await GetMsgs();
                    await DisplayText("Receiver is stopping.");
                });
            }

        }

        private bool SendIsRunning = false;
        int num = 0;
        private async void buttonSendRun_Click(object sender, RoutedEventArgs e)
        {

            SendIsRunning = !SendIsRunning;
            while (SendIsRunning)
            {
                await Task.Run(async () =>
                {
                    if (num++ > 10)
                        num = 0;
                    await DisplayText2(num.ToString());
                    string dt = DateTime.Now.ToString();
                    string jsn = "{\"Time\":\"" + dt + "\"," + "\"num\":" + num.ToString() + "}";
                    await AzureIoTHub.SendDeviceToCloudMessageAsyncNotTPM(jsn);
                    await Task.Delay(TimeSpan.FromSeconds(1));

                });
            }

        }


        private string Last = "0";
        private void textBoxVal_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb != null)
            {
                string valstr = tb.Text;
                uint val;
                bool res =  uint.TryParse(valstr, out val);
                if (!res)
                    tb.Text = Last;
                Last = tb.Text;
            }
        }

        private void textBoxConnectionString_TextChanged(object sender, TextChangedEventArgs e)
        {
            AzureIoTHub.DeviceConnectionString = textBoxConnectionString.Text;
        }
    }
}

