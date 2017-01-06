using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Leap;
using System.Diagnostics;
using System.Windows.Media;

namespace WpfPr1
{

    public interface ILeapEventDeligate
    {
        void leapEventNotification(string s, int x, int y);
    }
    class LeapListener : Listener
    {
        long currTime, prevTime, timeDif;
        ILeapEventDeligate iled;

        public LeapListener(ILeapEventDeligate param) {
            this.iled = param;
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
        public override void OnFrame(Controller ctrl)
        {
            //base.OnFrame(ctrl);
            
            //MessageBox.Show("On Frame");
            Debug.WriteLine("on frame");
            Frame currFrame = ctrl.Frame();

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
                        GestureList gl = currFrame.Gestures();
                        int flag = 0;
                        for (int i = 0; i < gl.Count; i++)
                        {
                            flag = 1;
                            System.Media.SystemSounds.Beep.Play();
                            break;
                            if (gl[i].Type == Gesture.GestureType.TYPE_SCREEN_TAP)
                            {
                                flag = 1;
                                Debug.WriteLine("gesture is gesture");
                                System.Media.SystemSounds.Beep.Play();
                            }
                        }
                        if (flag == 1)
                        {
                            this.iled.leapEventNotification("On Frame", -1, -1);
                        }
                        
                        if (flag == 0 && tipVelocity > 15)   // to stabilise the pointer
                        {
                            var normalizedVector = currFrame.InteractionBox.NormalizePoint(currFrame.Fingers.Frontmost.StabilizedTipPosition);
                            var tempPt = this.convertToPointInSpaceTime(normalizedVector);
                            var xScreenIntersect = tempPt.X;
                            var yScreenintersect = tempPt.Y;
                            if (xScreenIntersect.ToString() != "NaN")
                            {
                                this.iled.leapEventNotification("On Frame", xScreenIntersect, yScreenintersect);
                                //MouseCursor.MoveCursor(xScreenIntersect, yScreenintersect);
                            }
                        }
                    }
                }
                prevTime = currTime;
            }
            //this.iled.leapEventNotification("On Frame");
        }
        public override void OnInit(Controller ctrl)
        {
            //base.OnInit(arg0);
            //MessageBox.Show("Initialized");
            Debug.WriteLine("intialized");
        }
        public override void OnConnect(Controller ctrl)
        {
            //base.OnConnect(arg0);
            //MessageBox.Show("Connected");
            Debug.WriteLine("Connected");
        }
        public override void OnDisconnect(Controller ctrl)
        {
            //base.OnDisconnect(arg0);
            //MessageBox.Show("Disconnected");
            Debug.WriteLine("Disconnected");
        }
        // may add OnInit, OnConnect, OnDisconnect, OnExit
    }
}
