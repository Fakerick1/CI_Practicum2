using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Sudoku
    {
        public SudokuNode[,] nodeArray;
        private const int SudokuSize = 9;
        private const int SudokuBlockSize = 3;
        private Random rnd;
        HashSet<int> correctRows;
        HashSet<int> correctColumns;

        public Sudoku(string input)
        {
            rnd = new Random();
            correctRows = new HashSet<int>();
            correctColumns = new HashSet<int>();
            this.nodeArray = new SudokuNode[9, 9];
            ConvertInputToNodeArray(input);
            PopulateNodeArray();
            EvaluateSudoku(); // check how many rows and columns have 9 different numbers in them
            PrintSudoku(); // display the sudoku           
            for (int i = 0; i < 200; i++)
            {
                SolveSudoku(); // apply algorithm for solving sudoku, now set with a finite value of reps so computer won't get destroyed
            }
            EvaluateSudoku();
            PrintSudoku();
        }
        private static int CalculateRow(int i, int h)
        {
            return 3 * (h / 3) + i;
        }

        private static int CalculateColumn(int j, int h)
        {
            return 3 * (h % 3) + j;
        }

        private void ConvertInputToNodeArray(string input)
        {
            // Trim input since input is always given with a white space, will also work with input where this is not the case
            int[] inputArray = input.Trim().Split(' ').Select(int.Parse).ToArray();

            //for loop for rows
            for (int i = 0; i < SudokuSize; i++)
            {
                //for loop for columns
                for (int j = 0; j < SudokuSize; j++)
                {
                    if (inputArray[(i * SudokuSize) + j] != 0)
                    {
                        nodeArray[i, j] = new SudokuNode(inputArray[(i * SudokuSize) + j], true);
                    }
                }
            }
        }

        private void PopulateNodeArray()
        {
            int randomNumber;
            List<int> numbersAlreadyInBlock = new List<int>();

            // for loop for each 3x3 block
            for (int h = 0; h < 9; h++)
            {
                //loop 2 times through each 3x3 block. first time placing all static numbers in the block, second time placing all random generated numbers
                for (int g = 0; g < 2; g++)
                {
                    //for loop for rows
                    for (int i = 0; i < SudokuBlockSize; i++)
                    {
                        //for loop for columns
                        for (int j = 0; j < SudokuBlockSize; j++)
                        {
                            if (nodeArray[(3 * (h / 3) + i), (3 * (h % 3) + j)] != null)
                            {
                                numbersAlreadyInBlock.Add(nodeArray[(3 * (h / 3) + i), (3 * (h % 3) + j)].Value());
                            }
                            else
                            {
                                if (g == 1)
                                {
                                    randomNumber = rnd.Next(1, 10);

                                    while (numbersAlreadyInBlock.Contains(randomNumber))
                                    {
                                        randomNumber = new Random().Next(1, 10);
                                    }
                                    nodeArray[(3 * (h / 3) + i), (3 * (h % 3) + j)] = new SudokuNode(randomNumber, false);
                                    numbersAlreadyInBlock.Add(randomNumber);
                                }
                            }
                        }
                    }
                }
                numbersAlreadyInBlock.Clear();
            }

        }

        private void PrintSudoku()
        {
            // For each row
            for (int i = 0; i < SudokuSize; i++)
            {
                // For each column
                for (int j = 0; j < SudokuSize; j++)
                {
                    if (nodeArray[i, j] != null)
                    {
                        Console.Write(nodeArray[i, j].Value());
                    }
                    else
                    {
                        Console.Write(0);
                    }
                    Console.Write(" ");
                    if (j % 3 == 2)
                    {
                        Console.Write("  ");
                    }
                }
                Console.WriteLine();
                if (i % 3 == 2)
                {
                    Console.WriteLine();
                }
            }
        }

        private void SolveSudoku()
        {
            // While sudoku is not solved
            int h = rnd.Next(0, 9);
            ((int i, int j) firstNode, (int k, int l) secondNode, int h, int diff) bestSwap = default;

            //for loop for rows
            for (int i = 0; i < SudokuBlockSize; i++)
            {
                //for loop for columns
                for (int j = 0; j < SudokuBlockSize; j++)
                {
                    if (!nodeArray[(3 * (h / 3) + i), (3 * (h % 3) + j)].IsFixed())
                    {
                        for (int k = 0; k < SudokuBlockSize; k++)
                        {
                            for (int l = 0; l < SudokuBlockSize; l++)
                            {
                                // check if node is not fixed and check that it's not equal to itself
                                if (!nodeArray[(3 * (h / 3) + i), (3 * (h % 3) + j)].IsFixed() && !nodeArray[(3 * (h / 3) + k), (3 * (h % 3) + l)].IsFixed() && !(i == k && j == l))
                                {
                                    int currDiff = GetSwapEvaluation((i, j), (k, l), h);
                                    if (bestSwap == default || currDiff < bestSwap.diff)
                                    {
                                        bestSwap = ((i, j), (k, l), h, currDiff);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (bestSwap.diff == 0)
            {
                // Check if solved
                if (correctRows.Count == 9 && correctColumns.Count == 9)
                {
                    Console.WriteLine("Sudoku solved!");
                    Console.ReadKey();
                }
                else
                {
                    // Random walk S times (experiment with S)
                    Console.WriteLine("We have reached a plateau!");
                }
            }

            GetLocalEvaluationValue(new List<int>() { bestSwap.firstNode.i, bestSwap.secondNode.k }, "row");
            //TODO: refactor and update evaluations in dict
            (nodeArray[CalculateRow(bestSwap.firstNode.i, bestSwap.h), CalculateColumn(bestSwap.firstNode.j, bestSwap.h)],
                nodeArray[CalculateRow(bestSwap.secondNode.k, bestSwap.h), CalculateColumn(bestSwap.secondNode.l, bestSwap.h)]) =
                (nodeArray[CalculateRow(bestSwap.secondNode.k, bestSwap.h), CalculateColumn(bestSwap.secondNode.l, bestSwap.h)],
                nodeArray[CalculateRow(bestSwap.firstNode.i, bestSwap.h), CalculateColumn(bestSwap.firstNode.j, bestSwap.h)]); // Swap
        }

        private void EvaluateSudoku()
        {
            HashSet<int> valuesFound;

            // check rows
            for (int i = 0; i < SudokuSize; i++)
            {
                // check columns
                valuesFound = new HashSet<int>();
                for (int j = 0; j < SudokuSize; j++)
                {
                    valuesFound.Add(nodeArray[i, j].Value());
                }
                if (valuesFound.Count == 9)
                {
                    correctRows.Add(i);
                }
            }

            // check columns
            for (int j = 0; j < SudokuSize; j++)
            {
                // check rows
                valuesFound = new HashSet<int>();
                for (int i = 0; i < SudokuSize; i++)
                {
                    valuesFound.Add(nodeArray[i, j].Value());
                }
                if (valuesFound.Count == 9)
                {
                    correctColumns.Add(j);
                }
            }

            Console.WriteLine("Rows correct    : " + correctRows);
            Console.WriteLine("Columns correct : " + correctColumns);
        }

        private int GetLocalEvaluationValue(List<int> list, string rowOrColumn, bool doUpdate = false)
        {
            int evaluationValue = 0;
            HashSet<int> valuesFound;

            foreach (int i in list)
            {
                valuesFound = new HashSet<int>();
                for (int j = 0; j < SudokuSize; j++)
                {
                    if (rowOrColumn == "rows")
                    {
                        valuesFound.Add(nodeArray[i, j].Value());
                    }
                    else if (rowOrColumn == "cols")
                    {
                        valuesFound.Add(nodeArray[j, i].Value());
                    }
                }
                evaluationValue += SudokuSize - valuesFound.Count();

                if (doUpdate && SudokuSize - valuesFound.Count() == 0)
                {
                    if (rowOrColumn == "rows" && correctRows.Count < 9)
                    {
                        correctRows.Add(i);
                    }
                    else if (rowOrColumn == "cols" && correctColumns.Count < 9)
                    {
                        correctColumns.Add(i);
                    }
                }
            }
            return evaluationValue;
        }

        private int GetSwapEvaluation((int i, int j) firstNode, (int k, int l) secondNode, int h)
        {
            int i = (3 * (h / 3) + firstNode.i), j = (3 * (h % 3) + firstNode.j);
            int k = (3 * (h / 3) + secondNode.k), l = (3 * (h % 3) + secondNode.l);
            List<int> rows = new List<int>() { (i == k) ? i : i, k };
            List<int> cols = new List<int>() { (j == l) ? j : j, l };

            // get sum of how much unique numbers are in the rows and columns
            int currentEvaluationValue = 0;
            currentEvaluationValue += GetLocalEvaluationValue(rows, "rows");
            currentEvaluationValue += GetLocalEvaluationValue(cols, "cols");

            (nodeArray[i, j], nodeArray[k, l]) = (nodeArray[k, l], nodeArray[i, j]); // Swap

            // get sum of how much unique numbers are in the rows and columns after swapping 2 tiles
            int newEvaluationValue = 0;
            newEvaluationValue += GetLocalEvaluationValue(rows, "rows");
            newEvaluationValue += GetLocalEvaluationValue(cols, "cols");

            (nodeArray[i, j], nodeArray[k, l]) = (nodeArray[k, l], nodeArray[i, j]); // Swap

            return newEvaluationValue - currentEvaluationValue;
        }
    }
}