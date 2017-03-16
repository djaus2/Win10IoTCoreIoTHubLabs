using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Devices.Gpio;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Win10IoTCoreIoTHubLabs
{
    public static class IoTGPIO
    {
        private const int LED_PIN = 21;
        private static GpioPinValue pinValue;
        public static GpioPin OutPin = null;

#if LESSON2
        private const int INPUT_PIN = 26;
        public static GpioPin InPin = null;
#endif

        public static void LEDOn()
        {
            pinValue = GpioPinValue.High;
            OutPin.Write(pinValue);
            Debug.WriteLine("LED On");
        }

        public static void LEDOff()
        {
            pinValue = GpioPinValue.Low;
            OutPin.Write(pinValue);
            Debug.WriteLine("LED Off");
        }

#if LESSON2
        public static int ReadInput()
        {
            pinValue = InPin.Read();
            if (pinValue == GpioPinValue.High)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
#endif

#if LESSON4
        public static async Task LEDFlash(int msPeriod)
        {
            LEDOn();
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(msPeriod));
            LEDOff();
        }
#endif

        public static void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                OutPin = null;
                Debug.WriteLine("There is no GPIO controller on this device.");
                return;
            }

            OutPin = gpio.OpenPin(LED_PIN);
            pinValue = GpioPinValue.High;
            OutPin.Write(pinValue);
            OutPin.SetDriveMode(GpioPinDriveMode.Output);
#if LESSON2
            InPin = gpio.OpenPin(INPUT_PIN);;
            InPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
#endif
            Debug.WriteLine("GPIO pin initialized correctly.");

        }
    }
}
