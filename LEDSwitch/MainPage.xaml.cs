using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LEDSwitch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private const int RED_PIN = 5;
        private const int GREEN_PIN = 6;
        private const int BLUE_PIN = 13;
        private GpioPin redPin;
        private GpioPin greenPin;
        private GpioPin bluePin;
        private GpioPinValue pinValue;
        private DispatcherTimer timer;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush blueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
        private SolidColorBrush blackBrush = new SolidColorBrush(Windows.UI.Colors.Black);

        public MainPage()
        {
            InitializeComponent();
            InitGPIO();
        }

        private void SwitchToggle(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = sender as ToggleSwitch;
            string sName = ts.Name;

            switch (sName)
            {
                case ("tsRed"):
                    if (ts.IsOn)
                        SwitchValue(GpioPinValue.Low, redPin, RedLED, redBrush);
                    else
                        SwitchValue(GpioPinValue.High, redPin, RedLED, blackBrush);
                    break;
                case ("tsGreen"):
                    if (ts.IsOn)
                        SwitchValue(GpioPinValue.Low, greenPin, GreenLED, greenBrush);
                    else
                        SwitchValue(GpioPinValue.High, greenPin, GreenLED, blackBrush);
                    break;
                case ("tsBlue"):
                    if (ts.IsOn)
                        SwitchValue(GpioPinValue.Low, bluePin, BlueLED, blueBrush);
                    else
                        SwitchValue(GpioPinValue.High, bluePin, BlueLED, blackBrush);
                    break;
            }
        }

        private void SwitchValue(GpioPinValue val, GpioPin pin, Ellipse led, SolidColorBrush brush)
        {
            pin.Write(val);
            led.Fill = brush;
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

            redPin = gpio.OpenPin(RED_PIN);
            greenPin = gpio.OpenPin(GREEN_PIN);
            bluePin = gpio.OpenPin(BLUE_PIN);

            pinValue = GpioPinValue.High;

            redPin.Write(pinValue);
            greenPin.Write(pinValue);
            bluePin.Write(pinValue);

            redPin.SetDriveMode(GpioPinDriveMode.Output);
            greenPin.SetDriveMode(GpioPinDriveMode.Output);
            bluePin.SetDriveMode(GpioPinDriveMode.Output);

            GpioStatus.Text = "GPIO pin initialized correctly.";

        }
    }
}
