using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPr1
{
    public class PointInSpaceTime
    {
        public DateTime At { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public PointInSpaceTime()
        {
            this.At = DateTime.Now;
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }
    }
}
