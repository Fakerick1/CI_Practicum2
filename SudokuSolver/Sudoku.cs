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

        private SudokuNode[,] nodeArray;
        private Random rnd;
        private HashSet<int> correctRows;
        private HashSet<int> correctColumns;
        private HashSet<int> plateauBlocks;

        private int sValue;
        private int maxPlateauRepetitions;
        private int subsequentZeroes;
        private int amountOfSteps;
        private int currentBlockIndex;

        public Sudoku(string input, int sValue, int maxPlateauRepetitions, Random rnd)
        {
            this.rnd = rnd;
            correctRows = new HashSet<int>();
            correctColumns = new HashSet<int>();
            this.nodeArray = new SudokuNode[9, 9];
            this.sValue = sValue;
            this.maxPlateauRepetitions = maxPlateauRepetitions;
            this.subsequentZeroes = 0;
            this.plateauBlocks = new HashSet<int>();
            this.amountOfSteps = 0;

            // Generate and display starting state
            ConvertInputToNodeArray(input);
            PopulateNodeArray();

            // Evaluate heuristic function
            UpdateAllHeuristics();
        }

        public void Solve()
        {
            // As long as the sudoku is not solved, generate successors and swap the best successor
            while (GetHeuristicValue() < 18)
            {
                ProcessBestSuccessor(GenerateBestSuccessor());
                if (SudokuSolver.PrintHeuristics) Console.WriteLine("Heuristic value: {0}, rows: {1}, cols: {2}", this.GetHeuristicValue(), this.correctRows.Count, this.correctColumns.Count);
                if (SudokuSolver.PrettyPrint) PrintSudoku();
                amountOfSteps++;
            }
        }

        public int GetAmountOfSteps()
        {
            return amountOfSteps;
        }

        private int GetHeuristicValue()
        {
            return correctRows.Count + correctColumns.Count;
        }

        private SudokuNode GetNodeAtPosition(int i, int j)
        {
            return this.nodeArray[CalculateRow(i), CalculateColumn(j)];
        }

        private int CalculateRow(int i)
        {
            return 3 * (this.currentBlockIndex / 3) + i;
        }

        private int CalculateColumn (int j)
        {
            return 3 * (this.currentBlockIndex % 3) + j;
        }

        private void ConvertInputToNodeArray(string input)
        {
            // Trim input since input is always given with a white space, will also work with input where this is not the case
            int[] inputArray = input.Trim().Split(' ').Select(int.Parse).ToArray();

            // Loop through rows
            for (int i = 0; i < SudokuSize; i++)
            {
                // Loop through columns
                for (int j = 0; j < SudokuSize; j++)
                {
                    if (inputArray[(i * SudokuSize) + j] != 0)
                    {
                        this.nodeArray[i, j] = new SudokuNode(inputArray[(i * SudokuSize) + j], true);
                    }
                }
            }
        }

        private void PopulateNodeArray()
        {
            int randomNumber;
            List<int> numbersAlreadyInBlock = new List<int>();

            // Loop through 3x3 blocks
            for (int h = 0; h < 9; h++)
            {
                this.currentBlockIndex = h;

                // Loop through each 3x3 block twice, first tracking all of the static numbers, then placing all the randomly generated numbers, which cannot be any of the static numbers
                for (int g = 0; g < 2; g++)
                {
                    // Loop through rows
                    for (int i = 0; i < SudokuBlockSize; i++)
                    {
                        // Loop through columns
                        for (int j = 0; j < SudokuBlockSize; j++)
                        {
                            if (GetNodeAtPosition(i, j) != null)
                            {
                                numbersAlreadyInBlock.Add(GetNodeAtPosition(i, j).Value());
                            }
                            else if (g == 1)
                            {
                                do
                                {
                                    randomNumber = rnd.Next(1, 10);
                                } while (numbersAlreadyInBlock.Contains(randomNumber));

                                // Add SudokuNode to NodeArray and add the number used to the numbers already in block
                                this.nodeArray[CalculateRow(i), CalculateColumn(j)] = new SudokuNode(randomNumber, false);
                                numbersAlreadyInBlock.Add(randomNumber);
                            }
                        }
                    }
                }
                numbersAlreadyInBlock.Clear();
            }
        }

        public void PrintSudoku()
        {
            // Loop through rows
            for (int i = 0; i < SudokuSize; i++)
            {
                // Loop through columns
                for (int j = 0; j < SudokuSize; j++)
                {
                    Console.ForegroundColor = (this.nodeArray[i, j].IsFixed()) ? ConsoleColor.Blue : ConsoleColor.Black;

                    if (correctRows.Contains(i) && correctColumns.Contains(j))
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                    } else if (correctRows.Contains(i) || correctColumns.Contains(j))
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                    } else
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }

                    if (this.nodeArray[i, j] != null)
                    {
                        Console.Write(this.nodeArray[i, j].Value());
                    }
                    else
                    {
                        Console.Write(0);
                    }
                    Console.Write(" ");
                    if (j % 3 == 2 && j != SudokuSize - 1)
                    {
                        Console.Write(" ");
                    }

                    Console.ResetColor();
                }
                Console.WriteLine();
                if (i % 3 == 2 && i != SudokuSize - 1)
                {
                    Console.WriteLine();
                }
                if (i == SudokuSize - 1 & SudokuSolver.PrettyPrint)
                {
                    Console.WriteLine("--------------------");
                }
            }
        }

        private ((int i, int j) firstNode, (int k, int l) secondNode, int diff) GenerateBestSuccessor()
        {
            do
            {
                this.currentBlockIndex = rnd.Next(0, 9);
            } while (plateauBlocks.Contains(this.currentBlockIndex) && plateauBlocks.Count < 9);

            ((int i, int j) firstNode, (int k, int l) secondNode, int diff) bestSwap = default;

            // Loop through rows
            for (int i = 0; i < SudokuBlockSize; i++)
            {
                // Loop through columns
                for (int j = 0; j < SudokuBlockSize; j++)
                {
                    // For every node, if it is not fixed, check all possible swaps within the block
                    if (!GetNodeAtPosition(i, j).IsFixed())
                    {
                        for (int k = 0; k < SudokuBlockSize; k++)
                        {
                            for (int l = 0; l < SudokuBlockSize; l++)
                            {
                                // If the other node is not fixed and the other node is not equal to the node currently being evaluated
                                if (!GetNodeAtPosition(k, l).IsFixed() && !(i == k && j == l))
                                {
                                    // Find the best possible swap within the block
                                    int currDiff = GetSwapEvaluation((i, j), (k, l));
                                    if (bestSwap == default || currDiff >= bestSwap.diff)
                                    {
                                        bestSwap = ((i, j), (k, l), currDiff);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return bestSwap;
        }

        private void ProcessBestSuccessor(((int i, int j) firstNode, (int k, int l) secondNode, int diff) bestSwap)
        {
            // Best available swap evaluates to a lower heuristic value than the current state
            if (bestSwap.diff < 0)
            {
                plateauBlocks.Add(this.currentBlockIndex);
                // If there is no better successor in each of the 9 blocks, random walk
                if (plateauBlocks.Count == 9)
                {
                    if (SudokuSolver.PrintHeuristics) Console.WriteLine("Random walk, reason: plateaublocks = 9");
                    RandomWalk();
                    plateauBlocks.Clear();
                }
            }
            else
            {
                // If evaluation of successor state is equal to that of current state
                if (bestSwap.diff == 0) this.subsequentZeroes++;

                if (this.subsequentZeroes >= this.maxPlateauRepetitions)
                {
                    // If a plateau has been reached and the algorithm is stuck on it for a predefined amount of steps, random walk
                    if (SudokuSolver.PrintHeuristics) Console.WriteLine("Random walk, reason: subsequent zeroes >= max");
                    RandomWalk();
                    this.subsequentZeroes = 0;
                }
                else
                {
                    // Best possible successor is better than current state
                    if (bestSwap.diff > 0)
                    {
                        this.subsequentZeroes = 0;
                    }
                    plateauBlocks.Clear();

                    // Swap if best successor is better or equal to current state
                    SwapNodes(bestSwap.firstNode.i, bestSwap.firstNode.j, bestSwap.secondNode.k, bestSwap.secondNode.l);
                    
                    // Update the evaluation function after the swap
                    UpdateHeuristics(new List<int>() { CalculateRow(bestSwap.firstNode.i), CalculateRow(bestSwap.secondNode.k) }, true);
                    UpdateHeuristics(new List<int>() { CalculateColumn(bestSwap.firstNode.j), CalculateColumn(bestSwap.secondNode.l) }, false);
                }

            }
        }

        private void RandomWalk()
        {
            // Variables for random coordinates to be selected
            int i, j, k, l;

            // Do a predefined amound of random swaps
            for (int s = 0; s < sValue; s++)
            {
                do
                {
                    this.currentBlockIndex = rnd.Next(0, 9);
                    i = rnd.Next(0, 3);
                    j = rnd.Next(0, 3);
                    k = rnd.Next(0, 3);
                    l = rnd.Next(0, 3);
                } while (GetNodeAtPosition(i, j).IsFixed() || GetNodeAtPosition(k, l).IsFixed() || (i == k && j == l));

                SwapNodes(i, j, k, l);

                // Update the evaluation function for the swapped rows/columns
                UpdateHeuristics(new List<int>() { CalculateRow(i), CalculateRow(k)}, true);
                UpdateHeuristics(new List<int>() { CalculateColumn(j), CalculateColumn(l)}, false);
            }
        }

        private void SwapNodes(int i, int j, int k, int l)
        {
            // Use tuples to swap the array members, no temp value is needed
            (this.nodeArray[CalculateRow(i), CalculateColumn(j)], this.nodeArray[CalculateRow(k), CalculateColumn(l)]) =
                    (this.nodeArray[CalculateRow(k), CalculateColumn(l)], this.nodeArray[CalculateRow(i), CalculateColumn(j)]);
        }

        private List<int> GetLocalEvaluationValue(List<int> list, bool isRow)
        {
            List<int> evaluationValues = new List<int>();
            HashSet<int> valuesFound;

            // For a list of rows or columns, determine the evaluationValue, which is the amount of columns missing.
            foreach (int i in list)
            {
                valuesFound = new HashSet<int>();
                for (int j = 0; j < SudokuSize; j++)
                {
                    valuesFound.Add(isRow ? this.nodeArray[i, j].Value() : this.nodeArray[j, i].Value());
                }
                evaluationValues.Add(SudokuSize - valuesFound.Count());
            }
            return evaluationValues;
        }

        private void UpdateAllHeuristics()
        {
            // Update the evaluation value for all rows and columns
            List<int> indexes = Enumerable.Range(0, SudokuSize).ToList();
            UpdateHeuristics(indexes, true);
            UpdateHeuristics(indexes, false);
        }

        private void UpdateHeuristics(List<int> list, bool isRow)
        {
            List<int> evaluationValues = GetLocalEvaluationValue(list, isRow);

            // Combine two lists that are indexed in the same way so they can be accessed in the same foreach
            var evaluationsAndPositions = list.Zip(evaluationValues, (pos, eval) => new { Position = pos, Evaluation = eval });

            // Add or remove the correct rows/columns from the HashSet, adding a row/column that is already in does not change the HashSet, neither does removing a row/column that is not in the HashSet
            foreach (var value in evaluationsAndPositions) {
                if (value.Evaluation == 0)
                {
                    if (isRow)
                    {
                        this.correctRows.Add(value.Position);
                    }
                    else if (!isRow)
                    {
                        this.correctColumns.Add(value.Position);
                    }
                }
                else
                {
                    if (isRow)
                    {
                        this.correctRows.Remove(value.Position);
                    }
                    else if (!isRow)
                    {
                        this.correctColumns.Remove(value.Position);
                    }
                }
            }
        }

        private int GetSwapEvaluation((int i, int j) firstNode, (int k, int l) secondNode)
        {
            // Determine the rows and columns involved in the swap
            int i = CalculateRow(firstNode.i), j = CalculateColumn(firstNode.j);
            int k = CalculateRow(secondNode.k), l = CalculateColumn(secondNode.l);
            List<int> rows = new List<int>() { (i == k) ? i : i, k };
            List<int> cols = new List<int>() { (j == l) ? j : j, l };

            // Get the evaluationvalues of the rows and columns where the swap will take place
            int currentEvaluationValue = 0;
            currentEvaluationValue += GetLocalEvaluationValue(rows, true).Sum();
            currentEvaluationValue += GetLocalEvaluationValue(cols, false).Sum();

            SwapNodes(firstNode.i, firstNode.j, secondNode.k, secondNode.l);

            // Get the evaluationvalues of the rows and columns after they have been swapped
            int newEvaluationValue = 0;
            newEvaluationValue += GetLocalEvaluationValue(rows, true).Sum();
            newEvaluationValue += GetLocalEvaluationValue(cols, false).Sum();

            // Swap the nodes back
            SwapNodes(firstNode.i, firstNode.j, secondNode.k, secondNode.l);

            // Returns the difference between the summed evaluation values, the higher this value the better
            return currentEvaluationValue - newEvaluationValue;
        }
    }
}