using System.Text;

namespace SudokuSolver
{
    public class main
    {
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;

            string[] inputs = new string[5];
            inputs[0] = " 0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0";
            inputs[1] = " 2 0 0 0 8 0 3 0 0 0 6 0 0 7 0 0 8 4 0 3 0 5 0 0 2 0 9 0 0 0 1 0 5 4 0 8 0 0 0 0 0 0 0 0 0 4 0 2 7 0 6 0 0 0 3 0 1 0 0 7 0 4 0 7 2 0 0 4 0 0 6 0 0 0 4 0 1 0 0 0 3";
            inputs[2] = " 0 3 0 0 5 0 0 4 0 0 0 8 0 1 0 5 0 0 4 6 0 0 0 0 0 1 2 0 7 0 5 0 2 0 8 0 0 0 0 6 0 3 0 0 0 0 4 0 1 0 9 0 3 0 2 5 0 0 0 0 0 9 8 0 0 1 0 2 0 6 0 0 0 8 0 0 6 0 0 2 0";
            inputs[3] = " 0 2 0 8 1 0 7 4 0 7 0 0 0 0 3 1 0 0 0 9 0 0 0 2 8 0 5 0 0 9 0 4 0 0 8 7 4 0 0 2 0 8 0 0 3 1 6 0 0 3 0 2 0 0 3 0 2 7 0 0 0 6 0 0 0 5 6 0 0 0 0 8 0 7 6 0 5 1 0 9 0";
            inputs[4] = " 0 0 0 0 0 0 9 0 7 0 0 0 4 2 0 1 8 0 0 0 0 7 0 5 0 2 6 1 0 0 9 0 4 0 0 0 0 5 0 0 0 0 0 4 0 0 0 0 5 0 7 0 0 9 9 2 0 1 0 8 0 0 0 0 3 4 0 5 9 0 0 0 5 0 7 0 0 0 0 0 0";

            int amountOfRuns = 10;
            int[] sValues = new int[] {1, 2, 3, 4};
            int[] plateauValues = new int[] {10, 20, 30, 40};

            int maxIterations = (sValues.Length * plateauValues.Length * inputs.Length);
            StringBuilder csv = new StringBuilder();
        
            for (int i = 0; i < amountOfRuns; i++)
            {
                Console.WriteLine("Run: {0}", i);
                // Solve every given sudoku with a seeded random value for every combination of parameter values and save to csv
                Random rnd = new Random();
                int iterationCounter = 0;
                for (int j = 0; j < sValues.Length; j++)
                {
                    for (int k = 0; k < plateauValues.Length; k++)
                    {
                        for (int l = 0; l < inputs.Length; l++)
                        {
                            Solver solver = new Solver(inputs[l], sValues[j], plateauValues[k], rnd);

                            string line = $"{l},{sValues[j]},{plateauValues[k]},{solver.GetResult()}";
                            csv.AppendLine(line);

                            iterationCounter++;
                            Console.WriteLine(String.Format("Sudoku solved! Took: {0} steps. Iteration {1}/{2}", solver.GetResult(), iterationCounter, maxIterations));
                        }
                    }
                }
                Console.WriteLine("Elapsed: {0}", (DateTime.Now - start).Minutes);
            }

            // Relative path to root folder
            File.WriteAllText("..\\..\\..\\result.csv", csv.ToString());
            
        }
    }
}