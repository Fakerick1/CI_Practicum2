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
        private int sValue;
        private HashSet<int> plateauBlocks;

        public Sudoku(string input, int sValue)
        {
            rnd = new Random();
            correctRows = new HashSet<int>();
            correctColumns = new HashSet<int>();
            this.nodeArray = new SudokuNode[9, 9];
            this.sValue = sValue;
            this.plateauBlocks = new HashSet<int>();
            ConvertInputToNodeArray(input);
            PopulateNodeArray();
            EvaluateSudoku(); // check how many rows and columns have 9 different numbers in them
            PrintSudoku(); // display the sudoku
            int counter = 0;
            while(correctRows.Count != 9 && correctColumns.Count != 9)
            {
                SolveSudoku(); // apply algorithm for solving sudoku, now set with a finite value of reps so computer won't get destroyed
                counter++;
            }
            Console.WriteLine(counter);
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
            int h;
            do
            {
                h = rnd.Next(0, 9);
            } while (plateauBlocks.Contains(h) && plateauBlocks.Count < 9);

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
                                if (!nodeArray[(3 * (h / 3) + k), (3 * (h % 3) + l)].IsFixed() && !(i == k && j == l))
                                {
                                    int currDiff = GetSwapEvaluation((i, j), (k, l), h);
                                    if (bestSwap == default || currDiff >= bestSwap.diff)
                                    {
                                        bestSwap = ((i, j), (k, l), h, currDiff);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("diff: " + bestSwap.diff);
            Console.WriteLine("block: " + bestSwap.h);

            if (bestSwap.diff <= 0)
            {
                plateauBlocks.Add(h);
                // Check if solved
                if (correctRows.Count == 9 && correctColumns.Count == 9)
                {
                    Console.WriteLine("Sudoku solved!");
                    Console.ReadKey();
                }
                else if (plateauBlocks.Count == 9)
                {
                    RandomWalk();
                    Console.WriteLine("We have reached a plateau!");
                    plateauBlocks.Clear();
                }
            } else
            {
                GetLocalEvaluationValue(new List<int>() { bestSwap.firstNode.i, bestSwap.secondNode.k }, true, true);
                GetLocalEvaluationValue(new List<int>() { bestSwap.firstNode.j, bestSwap.secondNode.l }, false, true);
                (nodeArray[CalculateRow(bestSwap.firstNode.i, bestSwap.h), CalculateColumn(bestSwap.firstNode.j, bestSwap.h)],
                    nodeArray[CalculateRow(bestSwap.secondNode.k, bestSwap.h), CalculateColumn(bestSwap.secondNode.l, bestSwap.h)]) =
                    (nodeArray[CalculateRow(bestSwap.secondNode.k, bestSwap.h), CalculateColumn(bestSwap.secondNode.l, bestSwap.h)],
                    nodeArray[CalculateRow(bestSwap.firstNode.i, bestSwap.h), CalculateColumn(bestSwap.firstNode.j, bestSwap.h)]); // Swap
                plateauBlocks.Clear();
            }
        }

        private void RandomWalk()
        {
            Console.WriteLine("Random walk");
            int h; // block
            int i, j, k, l; //coordinates in block;

            // s times do
            for (int s = 0; s < sValue; s++)
            {
                do
                {
                    h = rnd.Next(0, 9);
                    i = rnd.Next(0, 3);
                    j = rnd.Next(0, 3);
                    k = rnd.Next(0, 3);
                    l = rnd.Next(0, 3);
                } while (nodeArray[(3 * (h / 3) + i), (3 * (h % 3) + j)].IsFixed() || nodeArray[(3 * (h / 3) + k), (3 * (h % 3) + l)].IsFixed() || (i == k && j == l));

                (nodeArray[(3 * (h / 3) + i), (3 * (h % 3) + j)], nodeArray[(3 * (h / 3) + k), (3 * (h % 3) + l)]) = 
                    (nodeArray[(3 * (h / 3) + k), (3 * (h % 3) + l)], nodeArray[(3 * (h / 3) + i), (3 * (h % 3) + j)]);
            }
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

            Console.WriteLine("Rows correct    : " + correctRows.Count);
            Console.WriteLine("Columns correct : " + correctColumns.Count);
        }

        private int GetLocalEvaluationValue(List<int> list, bool isRow, bool doUpdate = false)
        {
            int evaluationValue = 0;
            HashSet<int> valuesFound;

            foreach (int i in list)
            {
                valuesFound = new HashSet<int>();
                for (int j = 0; j < SudokuSize; j++)
                {
                    if (isRow)
                    {
                        valuesFound.Add(nodeArray[i, j].Value());
                    }
                    else
                    {
                        valuesFound.Add(nodeArray[j, i].Value());
                    }
                }
                evaluationValue += SudokuSize - valuesFound.Count();

                if (doUpdate)
                {
                    if (SudokuSize - valuesFound.Count() == 0)
                    {
                        if (isRow && correctRows.Count < 9)
                        {
                            correctRows.Add(i);
                        }
                        else if (!isRow && correctColumns.Count < 9)
                        {
                            correctColumns.Add(i);
                        }
                    } else
                    {
                        if (isRow)
                        {
                            correctRows.Remove(i);
                        }
                        else if (!isRow)
                        {
                            correctColumns.Remove(i);
                        }
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
            currentEvaluationValue += GetLocalEvaluationValue(rows, true);
            currentEvaluationValue += GetLocalEvaluationValue(cols, false);

            (nodeArray[i, j], nodeArray[k, l]) = (nodeArray[k, l], nodeArray[i, j]); // Swap

            // get sum of how much unique numbers are in the rows and columns after swapping 2 tiles
            int newEvaluationValue = 0;
            newEvaluationValue += GetLocalEvaluationValue(rows, true);
            newEvaluationValue += GetLocalEvaluationValue(cols, false);

            (nodeArray[i, j], nodeArray[k, l]) = (nodeArray[k, l], nodeArray[i, j]); // Swap

            return currentEvaluationValue - newEvaluationValue;
        }
    }
}