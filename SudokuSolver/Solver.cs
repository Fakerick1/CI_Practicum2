using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Solver
    {
        List<String> inputs;
        int amountOfRuns;
        int[] sValues;
        int[] plateauValues;
        bool printOutput = false;

        public Solver(List<String> inputs) : this(inputs, 1, new int[] { 2 }, new int[] { 10 }) {
            printOutput = true;
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
                Random rnd = new Random();
                int iterationCounter = 0;
                for (int j = 0; j < sValues.Length; j++)
                {
                    for (int k = 0; k < plateauValues.Length; k++)
                    {
                        for (int l = 0; l < inputs.Count; l++)
                        {
                            // Create sudoku with the current parameter configuration
                            Sudoku sudoku = new Sudoku(inputs.ElementAt(l), sValues[j], plateauValues[k], rnd, this.printOutput);
                            if (printOutput) sudoku.PrintSudoku();
                            sudoku.Solve();

                            iterationCounter++;

                            // Create and show string with information on the solved sudoku
                            string line = $"{l},{sValues[j]},{plateauValues[k]},{sudoku.GetAmountOfSteps()}";
                            csv.AppendLine(line);
                            Console.WriteLine(String.Format("Sudoku solved! Took: {0} steps. Iteration {1}/{2}", sudoku.GetAmountOfSteps(), iterationCounter, maxIterations));
                            if (printOutput) sudoku.PrintSudoku();

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
