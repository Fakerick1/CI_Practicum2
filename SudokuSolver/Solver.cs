using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Solver
    {
        Sudoku sudoku;
        int result;

        public Solver(string input, int sValue, int maxPlateauRepetitions, Random rnd)
        {
            this.sudoku = new Sudoku(input, sValue, maxPlateauRepetitions, rnd);
            this.result = this.sudoku.GetAmountOfSteps();
        }

        public int GetResult()
        {
            return this.result;
        }
    }
}
