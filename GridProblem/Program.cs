// See https://aka.ms/new-console-template for more information
using GridProblem;

string path = "grid_input.txt";

Solution s = new Solution(path);
s.Solve();

Console.WriteLine();
Console.WriteLine("Press any key to continue...");
Console.ReadKey();