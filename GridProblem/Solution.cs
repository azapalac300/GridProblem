using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//using 

namespace GridProblem
{
    public class Solution
    {
        public List<float[]> gridPositions;


        public Solution()
        {
            string path = "grid_input.txt";
            gridPositions = new List<float[]>();

            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                string line = "";

                while ((line = reader.ReadLine())!= null)
                {

                    //Console.WriteLine(line);
                    string[] words = line.Split(',');
                    float [] position = new float[]{
                    float.Parse(words[0]),
                    float.Parse(words[1]),
                    };
                    Console.WriteLine(position[0] + "," + position[1]);
                    gridPositions.Add(position);
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
        //Assume alpha is small.
        //ALL graph points are present in the given data set.
        //This means we need to find the smallest delta X and delta Y such that deltaX == deltaY. Once we find that and the origin
        //we know where to start by finding the minimum magnitude from 0,0. This is the first point of the graph.

        public void Solve()
        {
            float alpha = 0.0f;


            //Find an arbitrary point in the graph. For this we will be using the 0 index.
            //From here, find the shortest magnitude vector difference from this point to any

            int startingIndex = 0;


            int closestPointIndex = 0;
            float minMag = float.MaxValue; //Stores the minimum magnitude
            float[] origin = gridPositions[startingIndex];
            //Try to find the minimum magnitude difference vector and its 90 degree counterpart
            //This will give us our cell dimensions
            for (int i = 0; i < gridPositions.Count; i++)
            {

                if (i != startingIndex)
                {
                    float[] pos = gridPositions[i];
                    float diffX = pos[0] - origin[0];
                    float diffY = pos[1] - origin[1];

                    float mag = MathF.Sqrt(diffX * diffX + diffY * diffY);

                    if (mag < minMag)
                    {
                        minMag = mag;
                        closestPointIndex = i;
                    }


                }
            }

            float[] closestPoint = gridPositions[closestPointIndex];
            //Extract the cell size by subtracting the closest point from the origin. This will give us one of the "walls" of the cell.
            float[] deltaA = new float[]
            {
                origin[0] - closestPoint[0],
                origin[1] - closestPoint[1],
            };
            //create the perpendicular complement of DeltaA
            float[] deltaB = new float[] {
                -deltaA[1],
                deltaA[0]
            };


            //loop through all points again. This time we will find how far away they are from the origin in units of DeltaA and DeltaB
            //(OgX, OgY) + A*(DeltaA) + B*(DeltaB) = gridPos
            //DeltaA and DeltaB are whole numbers
            //A*(DeltaA) + B*(DeltaB) = gridPos - origin
            //Need to find A and B
            
            for (int i = 0; i < gridPositions.Count; i++)
            {



            }
        }
    
    }
}
