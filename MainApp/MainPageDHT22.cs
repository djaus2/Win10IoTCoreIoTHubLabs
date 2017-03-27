using System;
using System.Collections.Generic;
using System.Linq;
using Sensors.Dht;
//using Sensors.OneWire.Common;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Win10IoTCoreIoTHubLabs
{
    public sealed partial class MainPage
    {
        //Source: https://www.hackster.io/porrey/dht11-dht22-temperature-sensor-077790
#if LESSON6 || LESSON7
        GpioPin OneWirePin = null;
        const int DHTPIN = 17;
        private IDht _dht = null;
        private List<int> _retryCount = new List<int>();
        private DateTimeOffset _startedAt = DateTimeOffset.MinValue;

        private ReaderWriterLockSlim dhtlock = null;

        public void InitDHT22()
        {
            dhtlock = new ReaderWriterLockSlim();
            GpioController controller = GpioController.GetDefault();

            if (controller != null)
            {
                OneWirePin = GpioController.GetDefault().OpenPin(DHTPIN, GpioSharingMode.Exclusive);
                _dht = new Dht22(OneWirePin, GpioPinDriveMode.Input);
                _startedAt = DateTimeOffset.Now;

            }
        }

        public void StopDHT22()
        {

            // ***
            // *** Dispose the pin.
            // ***
            if (OneWirePin != null)
            {
                OneWirePin.Dispose();
                OneWirePin = null;
            }

            // ***
            // *** Set the Dht object reference to null.
            // ***
            _dht = null;


        }

        public  string GetTempReadingsAsJSon()
        {
            JObject o = JObject.FromObject(Reading);
            string json = o.ToString();
            return json;
        }


        DhtReading _reading;
        DhtReading Reading
        {
            get
            {
                dhtlock.EnterReadLock();
                try
                {
                    return _reading;
                }
                finally
                {
                    dhtlock.ExitReadLock();
                }
            }

            set
            {
                dhtlock.EnterWriteLock();
                try
                {
                    _reading = value;
                }
                finally
                {
                    dhtlock.ExitWriteLock();
                }
            }
        }
         
        private async Task ReadTempHumidity()
        {
            if (dhtlock == null)
                InitDHT22();
            Reading  = new DhtReading();
            int val = this.TotalAttempts;
            this.TotalAttempts++;
            
            Reading = await _dht.GetReadingAsync().AsTask();

            _retryCount.Add(Reading.RetryCount);

            if (Reading.IsValid)
            {
                this.TotalSuccess++;
                this.Temperature = Convert.ToSingle(Reading.Temperature);
                this.Humidity = Convert.ToSingle(Reading.Humidity);
                this.LastUpdated = DateTimeOffset.Now;
            }
        }

        public string PercentSuccess
        {
            get
            {
                string returnValue = string.Empty;

                int attempts = this.TotalAttempts;

                if (attempts > 0)
                {
                    returnValue = string.Format("{0:0.0}%", 100f * (float)this.TotalSuccess / (float)attempts);
                }
                else
                {
                    returnValue = "0.0%";
                }

                return returnValue;
            }
        }

        private int _totalAttempts = 0;
        public int TotalAttempts
        {
            get
            {
                return _totalAttempts;
            }
            set
            {
                _totalAttempts = value;
            }
        }

        private int _totalSuccess = 0;
        public int TotalSuccess
        {
            get
            {
                return _totalSuccess;
            }
            set
            {
                _totalSuccess = value;
            }
        }

        private float _humidity = 0f;
        public float Humidity
        {
            get
            {
                return _humidity;
            }

            set
            {
                _humidity = value;
            }
        }

        public string HumidityDisplay
        {
            get
            {
                return string.Format("{0:0.0}% RH", this.Humidity);
            }
        }

        private float _temperature = 0f;
        public float Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
            }
        }

        public string TemperatureDisplay
        {
            get
            {
                return string.Format("{0:0.0} °C", this.Temperature);
            }
        }

        private DateTimeOffset _lastUpdated = DateTimeOffset.MinValue;
        public DateTimeOffset LastUpdated
        {
            get
            {
                return _lastUpdated;
            }
            set
            {
                _lastUpdated = value;
            }
        }

        public string LastUpdatedDisplay
        {
            get
            {
                string returnValue = string.Empty;

                TimeSpan elapsed = DateTimeOffset.Now.Subtract(this.LastUpdated);

                if (this.LastUpdated == DateTimeOffset.MinValue)
                {
                    returnValue = "never";
                }
                else if (elapsed.TotalSeconds < 60d)
                {
                    int seconds = (int)elapsed.TotalSeconds;

                    if (seconds < 2)
                    {
                        returnValue = "just now";
                    }
                    else
                    {
                        returnValue = string.Format("{0:0} {1} ago", seconds, seconds == 1 ? "second" : "seconds");
                    }
                }
                else if (elapsed.TotalMinutes < 60d)
                {
                    int minutes = (int)elapsed.TotalMinutes == 0 ? 1 : (int)elapsed.TotalMinutes;
                    returnValue = string.Format("{0:0} {1} ago", minutes, minutes == 1 ? "minute" : "minutes");
                }
                else if (elapsed.TotalHours < 24d)
                {
                    int hours = (int)elapsed.TotalHours == 0 ? 1 : (int)elapsed.TotalHours;
                    returnValue = string.Format("{0:0} {1} ago", hours, hours == 1 ? "hour" : "hours");
                }
                else
                {
                    returnValue = "a long time ago";
                }

                return returnValue;
            }
        }

        public int AverageRetries
        {
            get
            {
                int returnValue = 0;

                if (_retryCount.Count() > 0)
                {
                    returnValue = (int)_retryCount.Average();
                }

                return returnValue;
            }
        }

        public string AverageRetriesDisplay
        {
            get
            {
                return string.Format("{0:0}", this.AverageRetries);
            }
        }

        public string SuccessRate
        {
            get
            {
                string returnValue = string.Empty;

                double totalSeconds = DateTimeOffset.Now.Subtract(_startedAt).TotalSeconds;
                double rate = this.TotalSuccess / totalSeconds;

                if (rate < 1)
                {
                    returnValue = string.Format("{0:0.00} seconds/reading", 1d / rate);
                }
                else
                {
                    returnValue = string.Format("{0:0.00} readings/sec", rate);
                }

                return returnValue;
            }
        }
#endif

    }
}
