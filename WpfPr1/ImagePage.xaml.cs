using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows;
//using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Diagnostics;
using Leap;

namespace WpfPr1
{
    /// <summary>
    /// Interaction logic for ImagePage.xaml
    /// </summary>
    public partial class ImagePage : Page, ILeapEventDeligate
    {
        int cropFlag, cropOption, imgHt, imgWd, actHt, actWd;
        Rectangle r;
        Ellipse elp;
        Point prevPt, startPt;
        double posTopLeftX, posTopLeftY;
        Controller ctrl;
        Listener l;
        Button currentB;


        public delegate void myDeligate(int x, int y);
        public void myFunc(int x, int y)
        {
            Debug.WriteLine("this is invoked!");
            if (this.CheckAccess())
            {
                Debug.WriteLine("i grant");
                if (x == -1 && y == -1)
                {
                    Debug.WriteLine("gesture recognized");

                    if (currentB != null)
                    {
                        if (currentB.Name != null)
                            Debug.WriteLine("name" + currentB.Name);
                        else Debug.WriteLine("no button name");
                        Debug.WriteLine("good progress");
                        //System.Media.SystemSounds.Beep.Play();
                        currentB.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    }
                    else
                    {
                        Debug.WriteLine("ERROR NOT BUTTON");
                    }
                }
                else if(x >=0 && y >= 0)
                {
                    Debug.WriteLine("this is fun");
                    MouseCursor.MoveCursor(x, y);
                }
            }
            else
                Debug.WriteLine("not allowed");
        }
        public ImagePage(System.Windows.Media.ImageSource src, Controller ctrlparam)
        {
            Debug.WriteLine("in imagepage!!!");
            InitializeComponent();
            Debug.WriteLine("Line1");
            ctrl = ctrlparam;
            Debug.WriteLine("Line2");
            //ctrl = new Controller();
            //ctrl.SetPolicyFlags(Leap.Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES);
            //l = new Listener();
            //ctrl.AddListener(l);
            l = new LeapListener(this);
            ctrl.AddListener(l);
            mainImg.Source = src;
            Debug.WriteLine("Line3");
            BitmapImage bmp = src as BitmapImage;
            Debug.WriteLine("Line4");
            actHt = bmp.PixelHeight;
            actWd = bmp.PixelWidth;
            Debug.WriteLine("Line5");
            getDimensions(550, 550, actHt, actWd, ref imgHt, ref imgWd);
            Debug.WriteLine("Line6");
            //MessageBox.Show("h=" + imgHt + " w=" + imgWd);
            viewCanvas.Height = imgCanvas.Height = imgButton.Height = imgHt;
            viewCanvas.Width = imgCanvas.Width = imgButton.Width = imgWd;
            cropFlag = 0;       // Dormant State
            cropOption = 0;     // Invalid
            Debug.WriteLine("Line7");
        }

        public void move_on_button(object sender, MouseEventArgs e)
        {
            //int k = Int32.Parse(statusBlock.Text);
            //k++;
            //statusBlock.Text = "" + k;
            currentB = e.Source as Button;
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
            if (this.CheckAccess())
            {
                Debug.WriteLine("Access Granted");
            }
            else
            {
                Debug.WriteLine("Access Denied");
                //myDeligate myd = new myDeligate(myFunc);

                this.Dispatcher.BeginInvoke(new myDeligate(myFunc), new object[] { x, y });
                Debug.WriteLine("RRDONE");
            }
        }

        public static void getDimensions(int maxHt, int maxWd, int ht, int wd, ref int paramHt, ref int paramWd) 
        {
            paramWd = maxWd;
            paramHt = (ht * maxWd) / wd;
            if (paramHt > maxHt)
            {
                paramHt = maxHt;
                paramWd = (wd * maxHt) / ht;
            }
        }
        private void BackClick(object sender, RoutedEventArgs e)
        {
            MediaPlayer mp1 = new MediaPlayer();
            mp1.Open(new Uri(@"..\..\glass_ping.mp3", UriKind.Relative));
            mp1.Volume = 0.09;
            mp1.Play();
            this.NavigationService.Navigate(new MainPage());
        }
        private Point getPt()
        {
            return Mouse.GetPosition(mainPanel);
        }
        private void MouseTrack(object sender, MouseEventArgs e)
        {
            // do SetCursorPos(x, y); - somewhere outside MouseTrack()
            Point p = getPt();
            Point buttonTopLeft = imgButton.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
            double errX, errY;
            errX = errY = 0;
            statusLabel.Text = "x: " + p.X + " y: " + p.Y;
            if (cropFlag != 0)
            {
                if (cropOption == 1)     // Rectangle
                {
                    imgCanvas.Children.Clear();
                    if (p.X < startPt.X)
                        errX = 2;
                    else
                        errX = -2;
                    if (p.Y < startPt.Y)
                        errY = 2;
                    else
                        errY = 0;
                    double wd = Math.Abs(startPt.X - p.X);
                    double ht = Math.Abs(startPt.Y - p.Y);
                    Point topLeftPt = new Point(Math.Min(startPt.X, p.X), Math.Min(startPt.Y, p.Y));
                    Rect rectDim = new Rect(topLeftPt, new Size(wd, ht));
                    r = new Rectangle();
                    r.Height = ht;
                    r.Width = wd;
                    r.Fill = Brushes.SkyBlue;
                    r.Stroke = Brushes.DeepSkyBlue;
                    r.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    r.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    posTopLeftX = rectDim.Left - buttonTopLeft.X + errX;
                    posTopLeftY = rectDim.Top - buttonTopLeft.Y + errY;
                    Canvas.SetTop(r, posTopLeftY);
                    Canvas.SetLeft(r, posTopLeftX);
                    r.Opacity = 0.5;
                    imgCanvas.Children.Add(r);
                }
                else if (cropOption == 2)
                {
                    imgCanvas.Children.Clear();
                    errX = 2;
                    errY = 2;
                    elp = new Ellipse();
                    elp.Width = Math.Abs(p.X - startPt.X);
                    elp.Height = Math.Abs(p.Y - startPt.Y);
                    elp.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    elp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    elp.Fill = System.Windows.Media.Brushes.SkyBlue;
                    elp.Opacity = 0.5;
                    elp.Stroke = Brushes.DeepSkyBlue;
                    Point tempPt = new Point(Math.Min(p.X, startPt.X), Math.Min(p.Y, startPt.Y));
                    posTopLeftX = tempPt.X - buttonTopLeft.X + errX;
                    posTopLeftY = tempPt.Y - buttonTopLeft.Y + errY;
                    Canvas.SetTop(elp, posTopLeftY);
                    Canvas.SetLeft(elp, posTopLeftX);
                    imgCanvas.Children.Add(elp);
                }
                else if (cropOption == 3)
                {
                    errX = 0;
                    errY = 18;
                    if (System.Math.Abs(p.X - startPt.X) <= 4 && System.Math.Abs(p.Y - startPt.Y) <= 4)
                    {
                        if (cropFlag == 2)
                        {
                            cropFlag = 0;
                            Line newLine = new Line();
                            newLine.X1 = p.X - buttonTopLeft.X + errX;
                            newLine.Y1 = p.Y - buttonTopLeft.Y + errY;
                            newLine.X2 = startPt.X - buttonTopLeft.X + errX;
                            newLine.Y2 = startPt.Y - buttonTopLeft.Y + errY;
                            newLine.Stroke = System.Windows.Media.Brushes.IndianRed;
                            newLine.StrokeThickness = 2;
                            imgCanvas.Children.Add(newLine);
                        }
                    }
                    else if (cropFlag == 1)
                        cropFlag = 2;
                    if (cropFlag != 0)
                    {
                        Line newLine = new Line();
                        newLine.X1 = prevPt.X - buttonTopLeft.X + errX;
                        newLine.Y1 = prevPt.Y - buttonTopLeft.Y + errY;
                        newLine.X2 = p.X - buttonTopLeft.X + errX;
                        newLine.Y2 = p.Y - buttonTopLeft.Y + errY;
                        newLine.Stroke = System.Windows.Media.Brushes.IndianRed;
                        newLine.StrokeThickness = 2;
                        imgCanvas.Children.Add(newLine);
                        prevPt.X = p.X;
                        prevPt.Y = p.Y;
                    }
                }
            }
        }

        private void MouseTrackEnd(object sender, MouseEventArgs e)
        {
            statusLabel.Text = "";
        }

        private void CropFlagSetter(object sender, RoutedEventArgs e)
        {
            if (cropFlag != 0)      // Active State
            {
                if (cropFlag == 1)
                {
                    if (cropOption == 1)        // Rectangle
                    {
                        try
                        {
                            Button b = e.OriginalSource as Button;
                            System.Windows.Controls.Image img = b.Content as System.Windows.Controls.Image;
                            string src = img.Source.ToString();
                            src = src.Replace('/', '\\');
                            src = src.Substring(8);
                            BitmapImage bi = new BitmapImage();
                            bi.BeginInit();
                            bi.UriSource = new Uri(src, UriKind.Absolute);
                            bi.DecodePixelHeight = imgHt;
                            bi.DecodePixelWidth = imgWd;
                            bi.EndInit();
                            Int32Rect cropRect = new Int32Rect((int)posTopLeftX, (int)posTopLeftY, (int)r.Width, (int)r.Height);
                            CroppedBitmap cb = new CroppedBitmap(bi, cropRect);
                            int ch, cw;
                            ch = cw = 0;
                            ImagePage.getDimensions(Math.Min(250, (int)r.Height), Math.Min(250, (int)r.Width), (int)r.Height, (int)r.Width, ref ch, ref cw);
                            //ch = (int)r.Height;
                            //cw = (int)r.Width;
                            cropImgButton.Height = ch;
                            cropImgButton.Width = cw;
                            croppedImg.Source = cb;
                            //int top, left;
                            //top = (int)((cropCanvas.ActualHeight - cropImgButton.ActualHeight) / 2) - 70;
                            //left = (int)((cropCanvas.ActualWidth - cropImgButton.ActualWidth) / 2) - 90;
                            //Canvas.SetTop(cropImgButton, top);
                            //Canvas.SetLeft(cropImgButton, left);
                        }
                        catch (Exception e1)
                        {
                            statusLabel.Text += " Exception Thrown | Stack Trace : " + e1.StackTrace;
                        }
                    }
                }
                else if (cropFlag == 2)
                {
                    imgCanvas.Children.Clear();
                }
                cropFlag = 0;
            }
            else            
                cropFlag = 1;           // Make state Active

            prevPt = getPt();
            startPt = getPt();
        }

        private void selectOpt(object sender, RoutedEventArgs e)
        {
            MediaPlayer mp1 = new MediaPlayer();
            mp1.Open(new Uri(@"..\..\glass_ping.mp3", UriKind.Relative));
            mp1.Volume = 0.09;
            mp1.Play();
            imgCanvas.Children.Clear();
            Button b = e.OriginalSource as Button;
            if (b.Name.Equals("RectB"))
                cropOption = 1;
            else if (b.Name.Equals("OvalB"))
                cropOption = 2;
            else if (b.Name.Equals("ArbitB"))
                cropOption = 3;
        }

        private void myevent(object sender, MouseEventArgs e)
        {

            var p = Mouse.GetPosition(mainPanel);
            statusLabel.Text = "PageX=" + p.X + " PageY=" + p.Y;
        }
    }
}
