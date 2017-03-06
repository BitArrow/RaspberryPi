using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
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
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ReadButtons
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GpioPin redPin;
        private GpioPin greenPin;
        private GpioPin bluePin;
        private GpioPinValue pinValue;
        private SolidColorBrush whiteBrush = new SolidColorBrush(Windows.UI.Colors.White);
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush blueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
        private SolidColorBrush blackBrush = new SolidColorBrush(Windows.UI.Colors.Black);

        public MainPage()
        {
            InitializeComponent();
            InitGPIO();
        }

        private void SwitchValue(GpioPinValue val, GpioPin pin, Ellipse led, SolidColorBrush brush, GpioPinValueChangedEventArgs e)
        {
            if (pin != null)
                pin.Write(val);

            // need to invoke UI updates on the UI thread because this event
            // handler gets invoked on a separate thread.
            var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    led.Fill = blackBrush;
                    GpioStatus.Text = "Button Released";
                }
                else
                {
                    led.Fill = brush;
                    GpioStatus.Text = "Button Pressed";
                }
            });
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                redPin = null;
                greenPin = null;
                bluePin = null;
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            List<int> buttons = new List<int>() { (int)Button.White, (int)Button.Red, (int)Button.Green, (int)Button.Blue };

            foreach (var pinNum in buttons)
            {
                GpioPin btn = gpio.OpenPin(pinNum);

                // Set a debounce timeout to filter out switch bounce noise from a button press
                btn.DebounceTimeout = TimeSpan.FromMilliseconds(20);

                // Register for the ValueChanged event so our buttonPin_ValueChanged 
                // function is called when the button is pressed
                btn.ValueChanged += buttonPin_ValueChanged;
            }

            redPin = gpio.OpenPin((int)Led.Red);
            greenPin = gpio.OpenPin((int)Led.Green);
            bluePin = gpio.OpenPin((int)Led.Blue);

            pinValue = GpioPinValue.High;

            redPin.Write(pinValue);
            greenPin.Write(pinValue);
            bluePin.Write(pinValue);

            redPin.SetDriveMode(GpioPinDriveMode.Output);
            greenPin.SetDriveMode(GpioPinDriveMode.Output);
            bluePin.SetDriveMode(GpioPinDriveMode.Output);

            GpioStatus.Text = "GPIO pin initialized correctly.";

        }

        private void buttonPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            int pinNum = sender.PinNumber;
            GpioPinValue pinVal = GpioPinValue.Low;
            // toggle the state of the LED every time the button is pressed
            if (e.Edge == GpioPinEdge.RisingEdge)
            {
                pinVal = GpioPinValue.Low;
            }
            else
            {
                pinVal = GpioPinValue.High;
            }

            switch (pinNum)
            {
                case ((int)Button.White):
                    SwitchValue(pinVal, null, WhiteBTN, whiteBrush, e);
                    break;
                case ((int)Button.Red):
                    SwitchValue(pinVal, redPin, RedBTN, redBrush, e);
                    break;
                case ((int)Button.Green):
                    SwitchValue(pinVal, greenPin, GreenBTN, greenBrush, e);
                    break;
                case ((int)Button.Blue):
                    SwitchValue(pinVal, bluePin, BlueBTN, blueBrush, e);
                    break;
            }
        }
    }

    /// <summary>
    /// Enum for button pin values
    /// </summary>
    enum Button
    {
        White = 12,
        Red = 16,
        Green = 20,
        Blue = 21
    }
    
    /// <summary>
    /// Enum for led pin values
    /// </summary>
    enum Led
    {
        Red = 5,
        Green = 6,
        Blue = 13
    }
}
