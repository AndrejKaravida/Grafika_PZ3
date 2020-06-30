using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3.Models
{
    public class Point
    {
        string name;
        string swithState;
        double x;
        double y;
        long id;
        int approximationX;
        int approximationY;


        public Point(double x, double y, long id, string name, string state)
        {
            this.x = x;
            this.y = y;
            this.id = id;
            this.name = name;
            this.swithState = state;
        }

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public long Id { get => id; set => id = value; }
        public int ApproximationX { get => approximationX; set => approximationX = value; }
        public int ApproximationY { get => approximationY; set => approximationY = value; }
        public string Name { get => name; set => name = value; }
        public string SwithState { get => swithState; set => swithState = value; }
    }
}
