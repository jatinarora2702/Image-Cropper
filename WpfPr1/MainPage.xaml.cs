using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using Leap;

namespace WpfPr1
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    
    public partial class MainPage : Page, ILeapEventDeligate
    {
        public static List<string> picsList;
        Controller ctrl1 = new Controller();    // Will throw exception if Leap Motion Sensor is not connected
        double prevTime, currTime, timeDif;
        LeapListener l;
        Button currentB;

        public delegate void myDeligate(int x, int y);
        public void myFunc(int x, int y)
        {
            //Debug.WriteLine("this is invoked!");
            if (this.CheckAccess())
            {
                Debug.WriteLine("i grant");
                if (x == -1 && y == -1)
                {
                    Debug.WriteLine("gesture recognized");
                   
                    if (currentB != null)
                    {
                        Debug.WriteLine("good progress");
                        //System.Media.SystemSounds.Beep.Play();
                        currentB.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    }
                    else
                    {
                        Debug.WriteLine("ERROR NOT BUTTON");
                    }
                }
                else
                    MouseCursor.MoveCursor(x, y);
            }
            else
            {
                Debug.WriteLine("not allowed");
                this.Dispatcher.BeginInvoke(new myDeligate(myFunc), new object[] {x, y});
            }
                
        }
        public MainPage()
        {
            InitializeComponent();
            l = new LeapListener(this);
            ctrl1.AddListener(l);
            ctrl1.SetPolicyFlags(Leap.Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES);
            ctrl1.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
            ctrl1.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
            ctrl1.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
            ctrl1.EnableGesture(Gesture.GestureType.TYPEKEYTAP);
            populatePicsList();
            Button b = null;
            BitmapImage bi = null;
            System.Windows.Controls.Image img = null;
            int h, w;
            h = w = 0;
            //subTitleBlock.Text = "";
            //BitmapImage bi = new BitmapImage(new Uri(@"..\..\" + picsList[0], UriKind.Relative));
            for (int i = 0; i < picsList.Count; i++)
            {
                b = new Button();
                bi = new BitmapImage(new Uri(picsList[i]));
                img = new System.Windows.Controls.Image();
                img.Source = bi;
                b.Content = img;
                Thickness margin = b.Margin;
                margin.Left = margin.Right = margin.Top = margin.Bottom = 10;
                b.Margin = margin;
                bi = img.Source as BitmapImage;
                h = bi.PixelHeight;
                w = bi.PixelWidth;
                w = (w * 200) / h;
                h = 200;
                b.Height = h;
                b.Width = w;
                img.Stretch = Stretch.Fill;
                b.Click += new RoutedEventHandler(Button_Click);
                b.MouseMove += new MouseEventHandler(move_on_button);
                imgPanel.Children.Add(b);
            }
        }

        public void move_on_button(object sender, MouseEventArgs e)
        {

            int k = Int32.Parse(statusBlock.Text);
            k++;
            statusBlock.Text = "" + k;
            currentB = e.Source as Button;
            Dispatcher.BeginInvoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            if (currentB != null)
            {
                var img = currentB.Content as System.Windows.Controls.Image;
                if (img != null)
                {
                    Debug.WriteLine("INVOKED : " + img.Source);
                }
                else
                    Debug.WriteLine("img is NULL");
                
            }
            else
                Debug.WriteLine("currB is not a button");
            
        }
        public void leapEventNotification(string s, int x, int y)
        {
            if (x == -1 && y == -1)
            {
                Debug.WriteLine("GESTURE is HEREEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEe");
            }
            if (this.CheckAccess())
            {
                Debug.WriteLine("Access Granted");
            }
            else
            {
                Debug.WriteLine("Access Denied");
                //myDeligate myd = new myDeligate(myFunc);

                this.Dispatcher.BeginInvoke(new myDeligate(myFunc), new object[] { x, y });
            }
        }
        public void populatePicsList()
        {
            picsList = new List<string>();
            string path;
            string currLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path = System.IO.Path.GetFullPath(System.IO.Path.Combine(currLoc, @"..\..\images\"));
            DirectoryInfo di = new DirectoryInfo(path);
            if (di != null)
            {
                FileInfo[] subFi = di.GetFiles();
                //subTitleBlock.Text = "" + subFi.Length;
                for (int i = 0; i < subFi.Length; i++)
                {
                    picsList.Add(subFi[i].FullName);
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("navigating33");
            MediaPlayer mp1 = new MediaPlayer();
            mp1.Open(new Uri(@"..\..\glass_ping.mp3", UriKind.Relative));
            mp1.Volume = 0.09;
            mp1.Play();
            Debug.WriteLine("navigating11");
            Button b = e.OriginalSource as Button;
            var img = b.Content as System.Windows.Controls.Image;
            Debug.WriteLine("navigating22");
            if (img == null)
            {
                MessageBox.Show("ERROR");
            }
            else
                //MessageBox.Show("DONE");
            ctrl1.RemoveListener(l);
            Debug.WriteLine("navigating66");
            //ctrl1.Dispose();
            Debug.WriteLine("navigating");
            if (this.NavigationService == null)
            {
                Debug.WriteLine("navigation-service is null!!");
            }
            else
                this.NavigationService.Navigate(new ImagePage(img.Source, ctrl1));
        }

        public delegate void leapDeligate();
        private void leapEvent(object sender, MouseEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal , new leapDeligate(setLeapMouse));
        }
        public PointInSpaceTime convertToPointInSpaceTime(Leap.Vector v)
        {
            var width = System.Windows.SystemParameters.PrimaryScreenWidth;
            var height = System.Windows.SystemParameters.PrimaryScreenHeight;
            var x = (v.x * width);
            var y = (v.y * height);
            return new PointInSpaceTime()
            {
                X = (int)x,
                Y = (int)y,
                At = DateTime.Now
            };
        }
        public void setLeapMouse()
        {
            int temp = Int32.Parse(statusBlock.Text);
            temp++;
            statusBlock.Text = "" + temp;
            var currFrame = ctrl1.Frame();
            currTime = currFrame.Timestamp;
            timeDif = currTime - prevTime;
            if (timeDif > 10000)
            {
                if (currFrame.Hands.Count > 0)
                {
                    Finger finger = currFrame.Fingers[0];
                    if (!currFrame.Hands.IsEmpty)
                    {
                        var tipVelocity = (int)finger.TipVelocity.Magnitude;
                        if (tipVelocity > 15)   // to stabilise the pointer
                        {
                            var normalizedVector = currFrame.InteractionBox.NormalizePoint(currFrame.Fingers.Frontmost.StabilizedTipPosition);
                            var tempPt = this.convertToPointInSpaceTime(normalizedVector);
                            var xScreenIntersect = tempPt.X;
                            var yScreenintersect = tempPt.Y;
                            if (xScreenIntersect.ToString() != "NaN")
                            {
                                MouseCursor.MoveCursor(xScreenIntersect, yScreenintersect);
                            }
                        }
                    }
                }
                prevTime = currTime;
            }
        }

        private void page_mouse_move(object sender, MouseEventArgs e)
        {
            var p = Mouse.GetPosition(mainPanel);
            //statusBlock.Text = "page XX:" + p.X + " page YY:" + p.Y;
        }

    }
}
