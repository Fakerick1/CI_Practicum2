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

        public Sudoku(string input)
        {
            rnd = new Random();
            this.nodeArray = new SudokuNode[9, 9];
            ConvertInputToNodeArray(input);
            PopulateNodeArray();
            PrintSudoku();
            SolveSudoku();
            PrintSudoku();
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
            //TODO sometimes number is generated in a block where there already is a constant with same value, needs to be fixed

            // for loop for each 3x3 block
            for (int h = 0; h < 9; h++)
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
                }
                Console.WriteLine();
            }
        }

        private void SolveSudoku()
        {
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
                                if (!nodeArray[(3 * (h / 3) + i), (3 * (h % 3) + j)].IsFixed() && !(i == k && j == l))
                                {
                                    int currDiff = SwapNodes((i, j), (k, l), h);
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
            //TODO: refactor
            (nodeArray[CalculateRow(bestSwap.firstNode.i, bestSwap.h), CalculateColumn(bestSwap.firstNode.j, bestSwap.h)],
                nodeArray[CalculateRow(bestSwap.secondNode.k, bestSwap.h), CalculateColumn(bestSwap.secondNode.l, bestSwap.h)]) =
                (nodeArray[CalculateRow(bestSwap.secondNode.k, bestSwap.h), CalculateColumn(bestSwap.secondNode.l, bestSwap.h)],
                nodeArray[CalculateRow(bestSwap.firstNode.i, bestSwap.h), CalculateColumn(bestSwap.firstNode.j, bestSwap.h)]); // Swap
            Console.WriteLine(bestSwap.diff);
        }

        private int CalculateRow(int i, int h)
        {
            return 3 * (h / 3) + i;
        }

        private int CalculateColumn(int j, int h)
        {
            return 3 * (h % 3) + j;
        }

        private int Evaluate(List<int> list)
        {
            int evaluationValue = 0;
            HashSet<int> valuesFound;

            foreach (int i in list)
            {
                valuesFound = new HashSet<int>();
                for (int j = 0; j < 9; j++)
                {
                    valuesFound.Add(nodeArray[i, j].Value());
                }
                evaluationValue += 9 - valuesFound.Count();
            }
            return evaluationValue;
        }

        private int SwapNodes((int i, int j) firstNode, (int k, int l) secondNode, int h)
        {
            int i = (3 * (h / 3) + firstNode.i), j = (3 * (h % 3) + firstNode.j);
            int k = (3 * (h / 3) + secondNode.k), l = (3 * (h % 3) + secondNode.l);
            List<int> rows = new List<int>() { (i == k) ? i : i, k};
            List<int> cols = new List<int>() { (j == l) ? j : j, l };

            int currentEvaluationValue = 0;
            currentEvaluationValue += Evaluate(rows);
            currentEvaluationValue += Evaluate(cols);

            (nodeArray[i, j], nodeArray[k, l]) = (nodeArray[k, l], nodeArray[i, j]); // Swap

            int newEvaluationValue = 0;
            newEvaluationValue += Evaluate(rows);
            newEvaluationValue += Evaluate(cols);

            (nodeArray[i, j], nodeArray[k, l]) = (nodeArray[k, l], nodeArray[i, j]); // Swap

            return newEvaluationValue - currentEvaluationValue;
        }
    }
}