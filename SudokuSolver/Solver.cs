using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Solver
    {
        private List<String> inputs;
        private int amountOfRuns;
        private int[] sValues;
        private int[] plateauValues;

        public Solver(List<String> inputs) : this(inputs, 1, new int[] { 2 }, new int[] { 10 }) {
        }
        
        public Solver(List<String> inputs, int amountOfRuns, int[] sValues, int[] plateauValues) 
        {
            this.inputs = inputs;
            this.amountOfRuns = amountOfRuns;
            this.sValues = sValues;
            this.plateauValues = plateauValues;
        }

        public void Execute()
        {
            DateTime start = DateTime.Now;

            int maxIterations = (sValues.Length * plateauValues.Length * inputs.Count);
            StringBuilder csv = new StringBuilder();

            for (int i = 0; i < amountOfRuns; i++)
            {
                Console.WriteLine("Run: {0}", i);

                // Solve every given sudoku with a seeded random value for every combination of parameter values and save to csv
                int iterationCounter = 0;
                for (int j = 0; j < sValues.Length; j++)
                {
                    for (int k = 0; k < plateauValues.Length; k++)
                    {
                        for (int l = 0; l < inputs.Count; l++)
                        {
                            // Create sudoku with the current parameter configuration
                            Sudoku sudoku = new Sudoku(inputs.ElementAt(l), sValues[j], plateauValues[k], new Random(i));
                            if (SudokuSolver.PrintStartAndFinish) Console.WriteLine("Starting sudoku:");
                            if (SudokuSolver.PrintStartAndFinish) sudoku.PrintSudoku();
                            sudoku.Solve();

                            iterationCounter++;

                            // Create and show string with information on the solved sudoku
                            string line = $"{l},{sValues[j]},{plateauValues[k]},{sudoku.GetAmountOfSteps()}";
                            csv.AppendLine(line);
                            if (SudokuSolver.PrintStartAndFinish) Console.WriteLine(String.Format("Sudoku solved! Took: {0} steps. Iteration {1}/{2}. Solved sudoku:", sudoku.GetAmountOfSteps(), iterationCounter, maxIterations));
                            if (SudokuSolver.PrintStartAndFinish) sudoku.PrintSudoku();

                        }
                    }
                }
                Console.WriteLine("Elapsed: {0} seconds", (DateTime.Now.Subtract(start)).TotalSeconds);
            }
            // Relative path to root folder
            File.WriteAllText("..\\..\\..\\result.csv", csv.ToString());
        }
    }
}
