using System;
using System.Device.Gpio;
using Microsoft.Azure.Devices.Client;
using System.Text;

namespace rpitest
{
    class Program
    {

        private static readonly string connectionString = "[Connnection String]";
        private static DeviceClient deviceClient;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            deviceClient = DeviceClient.CreateFromConnectionString(connectionString);
            
            GpioController controller = new GpioController(PinNumberingScheme.Board);
            var pin = 10;
            var buttonPin = 26;
            var buttonPressed = false;
            
            controller.OpenPin(pin, PinMode.Output);
            controller.OpenPin(buttonPin, PinMode.InputPullUp);            

            try
            {
                while (true)
                {
                    if (controller.Read(buttonPin) == false)
                    {
                        controller.Write(pin, PinValue.High);

                        if (buttonPressed == false)
                        {
                            buttonPressed = true;
                            SendDeviceToCloudMessageAsync();
                        }
                    }
                    else
                    {
                        controller.Write(pin, PinValue.Low);
                        buttonPressed = false;
                    }
                }
            }
            finally
            {
                controller.ClosePin(pin);
            }
        }

        private static async void SendDeviceToCloudMessageAsync()
        {
            var messageString = "Button Pressed";
            Message message = new Message(Encoding.ASCII.GetBytes(messageString));

            message.Properties.Add("buttonEvent", "true");

            await deviceClient.SendEventAsync(message);
            Console.WriteLine("Sending Message {0}", messageString);

        }
    }
}
