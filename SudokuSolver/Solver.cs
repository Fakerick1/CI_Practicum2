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

        public Solver(List<String> inputs, int amountOfRuns) 
        {
            this.inputs = inputs;
            this.amountOfRuns = amountOfRuns;
        }

        public void Execute()
        {
            StringBuilder csv = new StringBuilder();

            for (int i = 0; i < amountOfRuns; i++)
            {
                Console.WriteLine("Run: {0}", i + 1);

                // Solve every given sudoku and save to csv
                for (int l = 0; l < inputs.Count; l++)
                {
                    // Create sudoku with the current parameter configuration
                    DateTime startSudoku = DateTime.Now;
                    Sudoku sudoku = new Sudoku(inputs.ElementAt(l));
                    if (SudokuSolver.PrintStartAndFinish) Console.WriteLine("Starting sudoku:");
                    if (SudokuSolver.PrintStartAndFinish) sudoku.PrintSudoku();
                    sudoku.Solve();

                    int sudokuTime = (int)DateTime.Now.Subtract(startSudoku).TotalMilliseconds;

                    // Write data to csv file
                    string line = $"{l},{sudoku.GetAmountOfSteps()},{sudokuTime}";
                    csv.AppendLine(line);

                    if (SudokuSolver.PrintStartAndFinish)
                    {
                        Console.WriteLine("\nFinished! Took: {0} ms and {1} steps.", sudokuTime, sudoku.GetAmountOfSteps());
                        sudoku.PrintSudoku();
                    }
                }
            }
            // Relative path to root folder
            File.WriteAllText("..\\..\\..\\result.csv", csv.ToString());
        }
    }
}
