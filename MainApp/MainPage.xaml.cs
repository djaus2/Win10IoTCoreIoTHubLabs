using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win10IoTCoreIoTHubLabs
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


       

        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

        public MainPage()
        {
            this.InitializeComponent();
            // ...
#if !LESSON1_0
            IoTGPIO.InitGPIO();
#if LESSON8 || LESSON9
            InitDHT22();
#endif
#endif
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int i = 0;
            i++;
#if !LESSON1_0
            if (IoTGPIO.OutPin != null)
            {
#endif

            var t = Task.Run(() => Loop());
#if !LESSON1_0
            }
#endif

        }


        int numLoops = 20;
        public async Task Loop()
        {
            for (int i = 0; i < numLoops; i++)
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    LED.Fill = redBrush;
                });

#if (!LESSON2) && (!LESSON5) && (!LESSON6) && (!LESSON7) && (!LESSON1_0)
                IoTGPIO.LEDOn();
#endif
#if LESSON1_0
                //Periodic flash simulated LED

#elif LESSON1_1
                //Periodic Flash LED only
#elif LESSON2
                //LED on when input is hi.
                int val = IoTGPIO.ReadInput();
                if (val==1)
                    IoTGPIO.LEDOn();
                else
                    IoTGPIO.LEDOff();  
#elif LESSON3
                //Send a string message to IoT Hub
                await Send(i);
#elif LESSON4
                //Recv a message from IoT Hub
                await recv();
#elif LESSON5
                //Send a message to IoT Hub when input is hi.
                int val = IoTGPIO.ReadInput();
                if (val==1)
                    await Send(i);              
#elif LESSON6
                //Get a command as a received message and action it.
                await doCommand();
#elif LESSON7
                //Get a command as a commande and action it.
                await doCommand2();
#elif LESSON8
                //Read temperature and humidity data from SHT15 sensor
                await ReadTemp();
#elif LESSON9
                //Send temperature and humidity data to IoT Hub
                await SendTemp();
#endif
                //Pause 400 mS for all
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(400));

                ///////////////
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    LED.Fill = grayBrush;
                });
#if (!LESSON2) && (!LESSON5) && (!LESSON6) && (!LESSON7) && (!LESSON1_0)
                IoTGPIO.LEDOff();
#endif
                //Pause 600 mS for all
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(600));

            }

            Application.Current.Exit();
        }


    }

}
