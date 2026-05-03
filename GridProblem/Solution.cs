using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;


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

        public Point RoundVals()
        {
            return new Point(MathF.Round(X), MathF.Round(Y));
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

        public static float Dot(Point p1, Point p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y;
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
        private bool errorFlag = false;
        //Map the points. This seems reduntant, but we will need this to do data matching later as the given points are not perfectly collinear.
        private Dictionary<string, Point> positionMap;
        private string path;

        private Point diffVect; //Store the initial difference vector to help us calculate alpha later

        public Solution(string filePath)
        {
            path = filePath;
            
            gridPositions = new List<Point>();
            positionMap = new Dictionary<string, Point>();
            diffVect = new Point(0, 0);
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                string line = "";

                while ((line = reader.ReadLine())!= null)
                {
                    string[] words = line.Split(',');
                    float x = float.Parse(words[0]);
                    Point p = new Point(float.Parse(words[0]), float.Parse(words[1]));
                    gridPositions.Add(p);

                    //Add position to positionmap
                    Point keyPoint = p.RoundVals();
                    positionMap.Add(keyPoint.ToString(), p);
                }

            }
            else
            {
                Console.WriteLine("File could not be found!");
                errorFlag = true;
            }

           
        }


        //Key assumptions:
        //Assume that smallest square side is the shortest distance between two given points on the same axis
        //We know all cells are square so this will give the length of both sides

        //ALL graph points are present in the given data set.
        //This means we need to find the smallest delta X and use it to compute delta Y (Assuming the grid is square and each cell is square)
        //This gives us the dimensions of an individual cell
        //We find the origin of the grap and use that as an "Anchor point" to re compute the data using our delta x and delta Y
        //Once we have matched all the data we use the coefficients to find the row and column of each data point

        public void Solve()
        {
            if (errorFlag) return;

            //Get the average of all the points. This will give us the center of the graph
            float xAvg = gridPositions.Average(p => p.X);
            float yAvg = gridPositions.Average(p => p.Y);
            Point midPointApprox = new Point(xAvg, yAvg);

            float maxDist = -1;
            int origIndex = -1;
            Point origPoint = new Point(0, 0);


            //We can use the mid point as an "Anchor Point" for the graph as well, but this only works effectively if the number of points in graph is odd.
            //To make it work for even numbered sets as well, find the origin point instead of the mid point.
            //Since we already have a mid point, we need to find the largest vector with x and y components both negative.
            //Finding this will get us the graph's origin
            for (int i = 0; i < gridPositions.Count; i++)
            {
                Point diff =   gridPositions[i] - midPointApprox;
                float dist = diff.Magnitude();

                if (dist > maxDist)
                {
                    if ((diff.X < 0 && diff.Y < 0) || (diff.X == 0 && diff.Y < 0)) //Added case for 45 degrees
                    {
                        maxDist = dist;
                        origIndex = i;
                        diffVect = diff;
                    }
                }
            }
            
            origPoint = gridPositions[origIndex];
            Console.WriteLine("Orig Point: " + origPoint);

            //Now we find the smallest diff between the orig point and another point. This is our grid cell size. 
            //We just need to rotate it 90 degrees to find the second grid cell size
            //Once we have both of these, we can reconstruct the graph

            float minDist = float.MaxValue;
            Point deltaA = new Point(0, 0);
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
                    }
                }
            }

            //Now that we have delta A, we can find delta B. Since these are our two fundamental coordinates, we can reconstruct the entire graph with them
            Point deltaB = new Point (deltaA.Y, -deltaA.X);

            //Check if origin plus delta B exists in the map, if not, flip deltaB
            string testKey = (deltaB + origPoint).RoundVals().ToString();
            if (!positionMap.ContainsKey(testKey))
            {
                 deltaB = deltaB * -1;
            }
        
           
            //With origPoint, deltaA, and deltaB, we have what we need to print our solution
            PrintSolution(origPoint, deltaA, deltaB);

            //Calculate alpha based on deltaA and deltaB
            CalculateAlpha(deltaA, deltaB);

        }

        private void PrintSolution(Point origPoint, Point deltaA, Point deltaB)
        {
            //Now we reconstruct the graph and use that reconstruction to print the solution.

            //Since the data set is not collinear, a heuristic is needed to match the non-linear data with the linear data I have generated here. 
            //The Band-Aid heuristic I am using here is to round the point X and Y values to the nearest integer causing a collision with the map I defined in the constructor.
            //Assumption: Graph increment size is larger than 1. Mapping function does not work otherwise.

            //Here I have formatted the data to match the desired pattern, hence the repetition

            //Assuming the graph will always be a perfect square, we can find its dimensions as an integer
            int dim = (int)MathF.Sqrt(gridPositions.Count);
            //I've printed the collinear solutions I generated as well.

            Console.WriteLine("Solution: ");
            string collinear = "";
            for (int i = 0; i < dim; i++)
            {
                string s = " Row " + i + ": ";
                collinear += s;
                for (int j = 0; j < dim; j++)
                {
                    Point gridPoint = origPoint + deltaA * i + deltaB * j;
                    collinear += gridPoint;

                    //create a unique collision by rounding to the nearest integer.
                    //This efficiently maps the linear  points I have generated with the non-linear points in the data set
                    Point keyPoint = gridPoint.RoundVals();
                    s += positionMap[keyPoint.ToString()];
                    if (j < dim - 1)
                    {
                        s += "-";
                        collinear  += "-";
                    }

                }
                Console.WriteLine(s);
                collinear += "\n";
            }


            for (int i = 0; i < dim; i++)
            {
                string s = " Col " + i + ": ";
                collinear += s;
                for (int j = dim - 1; j >= 0; j--)
                {
                    //Do the same for the columns. Here I switch DeltaB with DeltaA to generate column data instead of row data
                    Point gridPoint = origPoint + deltaB * i + deltaA * j;
                    collinear += gridPoint;

                    Point keyPoint = gridPoint.RoundVals();
                    s += positionMap[keyPoint.ToString()];
                    if (j > 0)
                    {
                        s += "-";
                        collinear += "-";
                    }

                }
                Console.WriteLine(s);
                collinear += "\n";
            }
            Console.WriteLine();
            Console.WriteLine("Generated collinear data:");
            Console.WriteLine(collinear);

        }

        private void CalculateAlpha(Point deltaA, Point deltaB)
        {

            //Now that we have deltaA and deltaB, we can calculate alpha
            //between deltaA and deltaB, find the lowest dot product with the Forward vector (0, 1)
            Point fwd = new Point(1, 0);
            float dotA = Point.Dot(deltaA, fwd);
            float dotB = Point.Dot(deltaB, fwd);
            float alpha = 0;

            Console.WriteLine("DeltaA: " + deltaA + " DeltaB: " + deltaB);

            //Console.WriteLine("Dot A: " + dotA + " Dot B: " + dotB);

            float maxDot = 0;

            Point closestToFwd = new Point(0, 0);

            if (MathF.Abs(dotA) > MathF.Abs(dotB))
            {
                maxDot = dotA;
                closestToFwd = deltaA;
            }
            else
            {
                maxDot = dotB;
                closestToFwd = deltaB;

            }

            //Magnitude of the forward vector is 1
            //so we just need the magnitde of closestToFwd
            float alphaA = MathF.Acos(dotA / deltaA.Magnitude()) * (180.0f / MathF.PI);
            float alphaB = MathF.Acos(dotB / deltaB.Magnitude()) * (180.0f / MathF.PI);

            //Check for correct quadrant. We want to find the smallest angle relative to (1, 0)
            //NOTE: This will work for all angles under 180, but not above 180 because of symmetry
            if((deltaA.X < 0 && deltaA.Y < 0) || (deltaA.X > 0 && deltaA.Y < 0))
            {
                alphaA += 180;
            }

            if ((deltaB.X < 0 && deltaB.Y < 0) || (deltaB.X > 0 && deltaB.Y < 0))
            {
                alphaB += 180;
            }

            alpha  = MathF.Min(alphaA, alphaB);
            Console.WriteLine("Alpha: " + alpha);
 
        }
   
    }
}
    