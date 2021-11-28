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
        private Dictionary<int, int> rowDict;
        private Dictionary<int, int> colDict;

        public Sudoku(string input)
        {
            rnd = new Random();
            rowDict = new Dictionary<int, int>();
            colDict = new Dictionary<int, int>();
            this.nodeArray = new SudokuNode[9, 9];
            ConvertInputToNodeArray(input);
            PopulateNodeArray();
            PrintSudoku();
            SolveSudoku();
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

            // Populate the row & col dictionaries
            EvaluateSudoku();
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
                                if (!nodeArray[(3 * (h / 3) + i), (3 * (h % 3) + j)].IsFixed() && !(i == k && j == l))
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
                if (GetEvaluationValue() == 0) {
                    // Sudoku is solved
                } else
                {
                    // Random walk S times (experiment with S)
                }
            }
            //TODO: refactor and update evaluations in dict
            (nodeArray[CalculateRow(bestSwap.firstNode.i, bestSwap.h), CalculateColumn(bestSwap.firstNode.j, bestSwap.h)],
                nodeArray[CalculateRow(bestSwap.secondNode.k, bestSwap.h), CalculateColumn(bestSwap.secondNode.l, bestSwap.h)]) =
                (nodeArray[CalculateRow(bestSwap.secondNode.k, bestSwap.h), CalculateColumn(bestSwap.secondNode.l, bestSwap.h)],
                nodeArray[CalculateRow(bestSwap.firstNode.i, bestSwap.h), CalculateColumn(bestSwap.firstNode.j, bestSwap.h)]); // Swap
            Console.WriteLine(bestSwap.diff);
        }

        private void EvaluateSudoku()
        {
            HashSet<int> valuesFound;
            // Check rows
            for (int i = 0; i < SudokuSize; i++)
            {
                valuesFound = new HashSet<int>();
                for (int j = 0; j < SudokuSize; j++)
                {
                    valuesFound.Add(nodeArray[i, j].Value());
                }
                rowDict[i] = SudokuSize - valuesFound.Count();
            }

            // Check cols
            for (int i = 0; i < SudokuSize; i++)
            {
                valuesFound = new HashSet<int>();
                for (int j = 0; j < SudokuSize; j++)
                {
                    valuesFound.Add(nodeArray[i, j].Value());
                }
                colDict[i] = SudokuSize - valuesFound.Count();
            }
        }

        private int GetEvaluationValue()
        {
            return rowDict.Sum(x => x.Value) + colDict.Sum(x => x.Value);
        }

        private int GetLocalEvaluationValue(List<int> list)
        {
            return 0;
        }

        private int UpdateEvaluation(List<int> list)
        {
            int evaluationValue = 0;
            HashSet<int> valuesFound;

            foreach (int i in list)
            {
                valuesFound = new HashSet<int>();
                for (int j = 0; j < SudokuSize; j++)
                {
                    valuesFound.Add(nodeArray[i, j].Value());
                }
                evaluationValue += SudokuSize - valuesFound.Count();
            }
            return evaluationValue;
        }

        private int GetSwapEvaluation((int i, int j) firstNode, (int k, int l) secondNode, int h)
        {
            int i = (3 * (h / 3) + firstNode.i), j = (3 * (h % 3) + firstNode.j);
            int k = (3 * (h / 3) + secondNode.k), l = (3 * (h % 3) + secondNode.l);
            List<int> rows = new List<int>() { (i == k) ? i : i, k};
            List<int> cols = new List<int>() { (j == l) ? j : j, l };

            // Get value from dict
            int currentEvaluationValue = 0;
            currentEvaluationValue += GetLocalEvaluationValue(rows);
            currentEvaluationValue += GetLocalEvaluationValue(cols);

            (nodeArray[i, j], nodeArray[k, l]) = (nodeArray[k, l], nodeArray[i, j]); // Swap

            int newEvaluationValue = 0;
            newEvaluationValue += UpdateEvaluation(rows);
            newEvaluationValue += UpdateEvaluation(cols);

            (nodeArray[i, j], nodeArray[k, l]) = (nodeArray[k, l], nodeArray[i, j]); // Swap

            return newEvaluationValue - currentEvaluationValue;
        }
    }
}