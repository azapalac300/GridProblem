using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GridProblem
{
    public class Solution
    {
        public List<String> gridPositions;
        public Solution()
        {
            string path = "grid_input.txt";
            gridPositions = new List<String>();
            if (File.Exists(path))
            {
                Console.WriteLine("File Exists! Path: " + path);
                StreamReader reader = new StreamReader(path);
                string line = "";

                while ((line = reader.ReadLine())!= null)
                {
                    gridPositions.Add(line);
                    Console.WriteLine(line);
                }

            }
            else
            {
                Console.WriteLine("File could not be found!");
            }
        }
            
     
        //Key assumptions:
        //Assume that 

        public void Solve () {
        
        }
    
    }
}
