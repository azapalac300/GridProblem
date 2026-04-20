using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
//using 

namespace GridProblem
{

    public class Point
    {

        //Use Mathf.Round to remove floating point errors and set Precision to the number of digits we want to round to
        //Since the solution needs to be precise to 1 decimal place, 1 is the mimimum needed to be safe, but we will use 5 just in case. 
        public int Precision { get { return 5; } }
        public float X, Y;
        public Point(float x, float y)
        {
            X = MathF.Round(x, Precision);
            Y = MathF.Round(y, Precision);
        }

        public float Distance(Point p)
        {
            Point diff = new Point(X - p.X, Y - p.Y);
            return diff.Magnitude();
        }

        public float Magnitude()
        {
            return MathF.Sqrt(X * X + Y * Y);
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }

        #region operators
        public static Point operator +(Point p1, Point p2) {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point operator *(Point p, int s)
        {
            return new Point(p.X * s, p.Y * s);
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return p1.X != p2.X && p1.Y != p2.Y;
        }
        #endregion

    }
    public class Solution
    {
        public List<Point> gridPositions;
        public Dictionary<string, Point>        //Map the points. We will need this to do data matching later as the given points are not perfectly collinear.
       


        public Solution()
        {
            string path = "grid_input.txt";
            gridPositions = new List<Point>();

            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                string line = "";

                while ((line = reader.ReadLine())!= null)
                {

                    //Console.WriteLine(line);
                    string[] words = line.Split(',');
                    float x = float.Parse(words[0]);
                    Point p = new Point(float.Parse(words[0]), float.Parse(words[1]));
                    Console.WriteLine(p);
                    gridPositions.Add(p);
                }

            }
            else
            {
                Console.WriteLine("File could not be found!");
            }

           
        }


        //Key assumptions:
        //Assume that smallest square side is the shortest distance between two given points on the same axis
        //We know all cells are square so this will give the length of both sides

        //ALL graph points are present in the given data set.
        //This means we need to find the smallest delta X and use it to compute delta Y (Assuming the grid is square and each cell is square)
        //This gives us the dimensions of an individual cell
        //We find the mid point and use that as an "Anchor point" to re compute the data using our delta x and delta Y
        //Once we have matched all the data we use the coefficients to find the row and column of each data point

        public void Solve()
        {
            //Get the average of all the points. This will give us the center of the graph
            float xAvg = gridPositions.Average(p => p.X);
            float yAvg = gridPositions.Average(p => p.Y);
            Point midPointApprox = new Point(xAvg, yAvg);

            Console.WriteLine("Mid point approximate: " + midPointApprox);
            float maxDist = -1;
            int origIndex = -1;
            Point origPoint = new Point(0, 0);

            //The point in the dataset closest to the average will be the center point

            //Matching the center point to a point on the graph only works if the number of data points is odd.
            //To make it work for even numbered sets, find the origin point instead of the mid point.
            //Since we already have a mid point, we need to find the largest vector with x and y components both negative. 
            for (int i = 0; i < gridPositions.Count; i++)
            {
                Point diff =   gridPositions[i] - midPointApprox;
                float dist = diff.Magnitude();

                if (dist > maxDist)
                {
                    if (diff.X < 0 && diff.Y < 0)
                    {
                        maxDist = dist;
                        origIndex = i;
                    }
                }
            }
            
            origPoint = gridPositions[origIndex];
            Console.WriteLine("Origin Point: " + origPoint);

            //Now we find the smallest diff between the orig point and another point. This is our grid cell size. 
            //We just need to rotate it 90 degrees to find the second grid cell size
            //Once we have both of these, we can reconstruct the graph

            float minDist = float.MaxValue;
            Point deltaA = new Point(0, 0);
            int checkIndex = -1;
            for(int i = 0; i < gridPositions.Count; i++)
            {
                if(i != origIndex)
                {
                    Point diff = gridPositions[i] - origPoint;
                    float dist = diff.Magnitude();
                    if (dist < minDist)
                    {
                        minDist = dist;
                        deltaA = diff;
                        checkIndex = i;
                    }
                }
            }

            //Now that we have delta A, we can find delta B. Since these are our two fundamental coordinates, we can reconstruct the entire graph with them
            Point deltaB = new Point (deltaA.Y, -deltaA.X);
            Console.WriteLine("DeltaA: " + deltaA + "  DeltaB: " + deltaB);
            //run check. if dataset contains origin + deltaB we are good. Otherwise we will have to invert deltaB
            //We can escape the loop early if we find deltaB.
            Point testPoint = origPoint + deltaB;
            Console.WriteLine("Test point: " + testPoint);




            //Assuming the graph will always be a perfect square, we can find its dimensions
            int dim = (int)MathF.Sqrt(gridPositions.Count);

            //Now we reconstruct the graph. Since dim is the square root of gridPositions.Count, this is technically a linear operation ;)
            //Assumption: Gaps between cells is larger than 1.
            //
            //Since "Rows" and "Columns" is not collinear, a heuristic is needed to match the non-linear data with the linear data I have generated here. 
            //The Band-Aid heuristic I am using here is to round the point X and Y values to the nearest integer 
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    Point gridPoint = origPoint + deltaA*i + deltaB*j;
                    Console.WriteLine(gridPoint);
                }

            }

        }
    
    }
}
    