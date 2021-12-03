using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Sudoku
    {

        private const int SudokuSize = 9;
        private const int SudokuBlockSize = 3;

        public SudokuNode[,] nodeArray;
        private Random rnd;
        private HashSet<int> correctRows;
        private HashSet<int> correctColumns;
        private HashSet<int> plateauBlocks;
        private int sValue;
        private int maxPlateauRepetitions;
        private int subsequentZeroes;

        public Sudoku(string input, int sValue, int maxPlateauRepetitions)
        {
            rnd = new Random();
            correctRows = new HashSet<int>();
            correctColumns = new HashSet<int>();
            this.nodeArray = new SudokuNode[9, 9];
            this.sValue = sValue;
            this.maxPlateauRepetitions = maxPlateauRepetitions;
            this.subsequentZeroes = 0;
            this.plateauBlocks = new HashSet<int>();

            // Generate and display starting state
            ConvertInputToNodeArray(input);
            PopulateNodeArray();
            PrintSudoku();

            // Evaluate heuristic function
            EvaluateSudoku();

            int amountOfSteps = 0;
            while (GetHeuristicValue() < 18)
            {
                ProcessBestSuccessor(GenerateBestSuccessor());
                Console.WriteLine("Heuristic value: {0}, rows: {1}, cols: {2}", GetHeuristicValue(), correctRows.Count, correctColumns.Count);
                amountOfSteps++;
            }
            Console.WriteLine(String.Format("Sudoku solved! Took: {0} steps.", amountOfSteps));
           
            PrintSudoku();
        }

        private int GetHeuristicValue()
        {
            return correctRows.Count + correctColumns.Count;
        }
        private SudokuNode GetNodeAtPosition(int i, int j, int h)
        {
            return nodeArray[CalculateRow(i, h), CalculateColumn(j, h)];
        }

        private int CalculateRow(int i, int h)
        {
            return 3 * (h / 3) + i;
        }

        private int CalculateColumn (int j, int h)
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
                            if (GetNodeAtPosition(i, j, h) != null)
                            {
                                numbersAlreadyInBlock.Add(GetNodeAtPosition(i, j, h).Value());
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
                                    nodeArray[CalculateRow(i, h), CalculateColumn(j, h)] = new SudokuNode(randomNumber, false);
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

        private ((int i, int j) firstNode, (int k, int l) secondNode, int h, int diff) GenerateBestSuccessor()
        {
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
                    if (!GetNodeAtPosition(i, j, h).IsFixed())
                    {
                        for (int k = 0; k < SudokuBlockSize; k++)
                        {
                            for (int l = 0; l < SudokuBlockSize; l++)
                            {
                                // check if node is not fixed and check that it's not equal to itself
                                if (!GetNodeAtPosition(k, l, h).IsFixed() && !(i == k && j == l))
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
            return bestSwap;
        }

        private void ProcessBestSuccessor(((int i, int j) firstNode, (int k, int l) secondNode, int h, int diff) bestSwap)
        {
            // BestSwap is worse
            if (bestSwap.diff < 0)
            {
                plateauBlocks.Add(bestSwap.h);
                // Check if solved
                if (plateauBlocks.Count == 9)
                {
                    Console.WriteLine("Random walk, reason: plateaublocks = 9");
                    RandomWalk();
                    plateauBlocks.Clear();
                }
            }
            else
            {   // If evaluation of successor state is equal to that of current state
                if (bestSwap.diff == 0) this.subsequentZeroes++;

                if (this.subsequentZeroes >= this.maxPlateauRepetitions)
                {
                    Console.WriteLine("Random walk, reason: subsequent zeroes > max");
                    RandomWalk();
                    this.subsequentZeroes = 0;
                }
                else
                {
                    SwapNodes(bestSwap.firstNode.i, bestSwap.firstNode.j, bestSwap.secondNode.k, bestSwap.secondNode.l, bestSwap.h);
                    plateauBlocks.Clear();
                    UpdateHeuristics(new List<int>() { CalculateRow(bestSwap.firstNode.i, bestSwap.h), CalculateColumn(bestSwap.secondNode.k, bestSwap.h) }, true);
                    UpdateHeuristics(new List<int>() { CalculateRow(bestSwap.firstNode.j, bestSwap.h), CalculateColumn(bestSwap.secondNode.l, bestSwap.h) }, false);
                }

            }
        }

        private void RandomWalk()
        {
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
                } while (GetNodeAtPosition(i, j, h).IsFixed() || GetNodeAtPosition(k, l, h).IsFixed() || (i == k && j == l));

                SwapNodes(i, j, k, l, h);

                UpdateHeuristics(new List<int>() { CalculateRow(i, h), CalculateRow(k, h)}, true);
                UpdateHeuristics(new List<int>() { CalculateColumn(j, h), CalculateColumn(l, h)}, false);
            }
        }

        private void SwapNodes(int i, int j, int k, int l, int h)
        {
            (nodeArray[CalculateRow(i, h), CalculateColumn(j, h)], nodeArray[CalculateRow(k, h), CalculateColumn(l, h)]) =
                    (nodeArray[CalculateRow(k, h), CalculateColumn(l, h)], nodeArray[CalculateRow(i, h), CalculateColumn(j, h)]);
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
        }

        private List<int> GetLocalEvaluationValue(List<int> list, bool isRow)
        {
            List<int> evaluationValue = new List<int>();
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
                evaluationValue.Add(SudokuSize - valuesFound.Count());
            }
            return evaluationValue;
        }

        private void UpdateHeuristics(List<int> list, bool isRow)
        {
            List<int> evaluationValues = GetLocalEvaluationValue(list, isRow);

            // Combine two lists that are indexed in the same way so they can be accessed at the same time
            var evaluationsAndPositions = list.Zip(evaluationValues, (pos, eval) => new { Position = pos, Evaluation = eval });
            foreach (var value in evaluationsAndPositions) {
                if (value.Evaluation == 0)
                {
                    if (isRow && correctRows.Count < 9)
                    {
                        correctRows.Add(value.Position);
                    }
                    else if (!isRow && correctColumns.Count < 9)
                    {
                        correctColumns.Add(value.Position);
                    }
                }
                else
                {
                    if (isRow)
                    {
                        correctRows.Remove(value.Position);
                    }
                    else if (!isRow)
                    {
                        correctColumns.Remove(value.Position);
                    }
                }
            }
        }

        private int GetSwapEvaluation((int i, int j) firstNode, (int k, int l) secondNode, int h)
        {
            int i = CalculateRow(firstNode.i, h), j = CalculateColumn(firstNode.j, h);
            int k = CalculateRow(secondNode.k, h), l = CalculateColumn(secondNode.l, h);
            List<int> rows = new List<int>() { (i == k) ? i : i, k };
            List<int> cols = new List<int>() { (j == l) ? j : j, l };

            // get sum of how much unique numbers are in the rows and columns
            int currentEvaluationValue = 0;
            currentEvaluationValue += GetLocalEvaluationValue(rows, true).Sum();
            currentEvaluationValue += GetLocalEvaluationValue(cols, false).Sum();

            SwapNodes(firstNode.i, firstNode.j, secondNode.k, secondNode.l, h);

            // get sum of how much unique numbers are in the rows and columns after swapping 2 tiles
            int newEvaluationValue = 0;
            newEvaluationValue += GetLocalEvaluationValue(rows, true).Sum();
            newEvaluationValue += GetLocalEvaluationValue(cols, false).Sum();

            SwapNodes(firstNode.i, firstNode.j, secondNode.k, secondNode.l, h);

            return currentEvaluationValue - newEvaluationValue;
        }
    }
}