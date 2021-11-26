using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Sudoku
    {
        public SudokuNode[] nodeArray;

        public Sudoku(string input)
        {
            this.nodeArray = new SudokuNode[81];
            ConvertInputToNodeArray(input);
            PopulateNodeArray();
        }

        private void ConvertInputToNodeArray(string input)
        {
            // Trim input since input is always given with a white space, will also work with input where this is not the case
            int[] inputArray = input.Trim().Split(' ').Select(int.Parse).ToArray();
            
            for (int i = 0; i < inputArray.Length; i++)
            {
                if (inputArray[i] != 0)
                {
                    nodeArray[i] = new SudokuNode(inputArray[i], true);
                }
            }
        }

        private void PopulateNodeArray()
        {
            // For each 3x3 in sudoku, populate unused space excepting fixed numbers

            // For each block
            for (int i = 0; i < nodeArray.Length / 9; i += 9)
            {
                // For each col in block
                for (int j = i; j < 3; j++)
                {
                    // For each row in block
                    for (int k = j; k < 3; k++)
                    {
                        Console.WriteLine(k);
                    }
                }
            }

        }

    }
}
