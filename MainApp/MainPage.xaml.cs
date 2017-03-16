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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win10IoTCoreIoTHubLabs
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


        int numLoops = 20;

        public MainPage()
        {
            // ...

            IoTGPIO.InitGPIO();
#if LESSON8 || LESSON9
            InitDHT22();

#endif
            if (IoTGPIO.OutPin != null)
            {

                var t = Task.Run(() => Loop());
                t.Wait();
            }

            Application.Current.Exit();
        }

        public async Task Loop()
        {
            for (int i = 0; i <  numLoops; i++)
            {

#if (!LESSON2) && (!LESSON5) && (!LESSON6)  && (!LESSON7)
                IoTGPIO.LEDOn();
#endif

#if LESSON1
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

#if (!LESSON2) && (!LESSON5) && (!LESSON6) && (!LESSON7)
                IoTGPIO.LEDOff();
#endif
                //Pause 600 mS for all
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(600));

            }
        }
    }
}
