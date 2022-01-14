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
        
        public Solver(List<String> inputs) 
        {
            this.inputs = inputs;
        }

        public void Execute()
        {
            DateTime start = DateTime.Now;
            int sudokuTime = 0;

            for (int l = 0; l < inputs.Count; l++)
            {
                // Create sudoku with the current parameter configuration
                DateTime startSudoku = DateTime.Now;
                Sudoku sudoku = new Sudoku(inputs.ElementAt(l));
                if (SudokuSolver.PrintStartAndFinish) Console.WriteLine("Starting sudoku:");
                if (SudokuSolver.PrintStartAndFinish) sudoku.PrintSudoku();
                sudoku.Solve();

                sudokuTime = (int)DateTime.Now.Subtract(startSudoku).TotalMilliseconds;

                Console.WriteLine();
                Console.WriteLine();
                if (SudokuSolver.PrintStartAndFinish) sudoku.PrintSudoku();

                Console.ReadKey();
            }
        }
    }
}
