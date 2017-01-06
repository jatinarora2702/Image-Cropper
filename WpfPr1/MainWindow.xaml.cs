using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Leap;

namespace WpfPr1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer mp = new MediaPlayer();
        //Controller ctrl = new Controller();
        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {

            }
            
            //LeapListener listener = new LeapListener();
            //ctrl.AddListener(listener);
            //ctrl.SetPolicyFlags(Leap.Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES);

            mp.Open(new Uri(@"..\..\relaxing_music.mp3", UriKind.Relative));
            mp.Play();
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            mainFrame.Navigate(new MainPage());
            mainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            //ctrl.RemoveListener(listener);
            //ctrl.Dispose();
        }

    }
}
