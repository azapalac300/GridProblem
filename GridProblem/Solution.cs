using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
//using 

namespace GridProblem
{
    public class Point
    {
        public float X, Y;
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float Distance(Point p)
        {
            Point diff = new Point(X - p.X, Y-p.Y);
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

        public static Point operator +(Point p1, Point p2) { 
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

    }
    public class Solution
    {
        public List<Point> gridPositions;


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
                    Console.WriteLine(p.X + "," + p.Y);
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
        //This gives us the dimensions of an idndividual cell
        //We find the mid point and use that as an "Anchor point" to re compute the data using our delta x and delta Y
        //Once we have matched all the data we use the coefficients to find the row and column of each data point

        public void Solve()
        {
            //Get the average of all the points. This will give us the center of the graph
            float xAvg = gridPositions.Average(p => p.X);
            float yAvg = gridPositions.Average(p => p.Y);
            Point midPointApprox = new Point(xAvg, yAvg);

            Console.WriteLine("Mid point approximate: " + midPointApprox);
            float minDist = float.MaxValue;
            int centerIndex = -1;
            Point midPoint = new Point(0, 0);
            //The point in the dataset closest to the average will be the center point
            for (int i = 0; i < gridPositions.Count; i++)
            {
                Point diff =  gridPositions[i] - midPointApprox;
                float dist = diff.Magnitude();

                if (dist < minDist)
                {
                    minDist = dist;
                    centerIndex = i;
                }
            }
            midPoint = gridPositions[centerIndex];
            Console.WriteLine("Mid Point: " + midPoint);

            //Now we find the smallest diff that isn't the mid point. Since every cell of the grid is a square, this will be our first vector
            //We just need to rotate it 90 degrees to find the secod
            //we can re-use minDist here, no need to declare a new variable

            
            minDist = float.MaxValue;
            Point deltaA = new Point(0, 0);
            for(int i = 0; i < gridPositions.Count; i++)
            {
                if(i != centerIndex)
                {
                    Point diff = midPoint - gridPositions[i];
                    float dist = diff.Magnitude();
                    if (dist < minDist)
                    {
                        minDist= dist;
                        deltaA = diff;
                    }
                }
            }
            //Now that we have delta A, we can find delta b
            Point deltaB = new Point (-deltaA.Y, deltaA.X);


            //Assuming the graph will always be a perfect square, we can find its dimensions
            int dim = (int)MathF.Sqrt(gridPositions.Count);

            //since we are use integer division 

        }
    
    }
}
    