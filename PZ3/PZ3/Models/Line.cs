using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3.Models
{
    public class Line
    {
        long startPoint;
        long endPoint;

        public Line(long start, long end)
        {
            startPoint = start;
            endPoint = end;
        }
        public long StartPoint { get => startPoint; set => startPoint = value; }
        public long EndPoint { get => endPoint; set => endPoint = value; }
    }
}
