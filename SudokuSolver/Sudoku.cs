using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Sudoku
    {
        public const int SudokuSize = 9;
        public const int SudokuBlockSize = 3;

        private SudokuNode[,] nodeArray;
        private Stack<DomainChangeList> changes;

        private int amountOfSteps;
        private int correctNodes;

        public Sudoku(string input)
        {
            this.nodeArray = new SudokuNode[SudokuSize, SudokuSize];
            this.amountOfSteps = 0;
            this.changes = new Stack<DomainChangeList>();

            // Generate and display starting state
            ConvertInputToNodeArray(input);

            // Ensure starting nodes are node-consistent
            EnsureNodeConsistency();
        }

        public void Solve()
        {
            // Use chronological backtracking by generating the nodes dynamically in depth-first order
            // Loop through rows
            for (int i = 0; i < SudokuSize; i++)
            {
                // Loop through columns
                for (int j = 0; j < SudokuSize; j++)
                {
                    if (!nodeArray[i, j].IsFixed())
                    {
                        // Check if domain of current node is empty
                        if (!this.nodeArray[i, j].SetFirstValue())
                        {
                            correctNodes--;
                            (int row, int column) source = UndoLastStep();
                            i = source.row;
                            j = source.column;
                        } else if (!EnsureLocalNodeConsistency(i, j))
                        {
                            (int row, int column) source = UndoLastStep();
                            i = source.row;
                            j = source.column;
                        } else
                        {
                            correctNodes++;
                        }
                        if (SudokuSolver.PrettyPrint) PrintSudoku();

                        amountOfSteps++;
                    }
                }
            }
            if (correctNodes != SudokuSize * SudokuSize)
            {
                Console.WriteLine("Sudoku was not finished correctly");
            }
        }

        public int GetAmountOfSteps()
        {
            return amountOfSteps;
        }

        private bool EnsureLocalNodeConsistency(int i, int j)
        {
            DomainChangeList domainChanges = new DomainChangeList((i,j));
            
            // Loop through rows and columns
            for (int k = 0; k < SudokuSize; k++)
            {
                if (this.nodeArray[i, k].Value() == 0)
                {
                    RemoveFromDomain(i, k, this.nodeArray[i, j].Value(), domainChanges);
                    if (SudokuSolver.ForwardChecking && this.nodeArray[i, k].DomainIsEmpty())
                    {
                        changes.Push(domainChanges);
                        return false;
                    }
                }
                if (this.nodeArray[k, j].Value() == 0)
                {
                    RemoveFromDomain(k, j, this.nodeArray[i, j].Value(), domainChanges);
                    if (SudokuSolver.ForwardChecking && this.nodeArray[k, j].DomainIsEmpty())
                    {
                        changes.Push(domainChanges);
                        return false;
                    }
                }
            }

            // Loop through 3*3 block
            int blockRowIndex = (i / SudokuBlockSize) * SudokuBlockSize;
            int blockColumnIndex = (j / SudokuBlockSize) * SudokuBlockSize;

            for (int l = blockRowIndex; l < blockRowIndex + SudokuBlockSize; l++)
            {
                for (int m = blockColumnIndex; m < blockColumnIndex + SudokuBlockSize; m++)
                {
                    if (this.nodeArray[l, m].Value() == 0)
                    {
                        RemoveFromDomain(l, m, this.nodeArray[i, j].Value(), domainChanges);
                        if (SudokuSolver.ForwardChecking && this.nodeArray[l, m].DomainIsEmpty())
                        {
                            changes.Push(domainChanges);
                            return false;
                        }
                    }
                }
            }

            changes.Push(domainChanges);
            return true;
        }

        private void RemoveFromDomain(int i, int j, int value, DomainChangeList domainChanges)
        {
            if (this.nodeArray[i, j].RemoveValue(value))
            {
                domainChanges.AddDomainChange(new DomainChange(i, j, value));
            }
        }

        private (int i, int j) UndoLastStep()
        {
            DomainChangeList domainChanges = changes.Pop();

            // Undo changes to domains that occurred in this step
            foreach (DomainChange domainChange in domainChanges.DomainChanges())
            {
                this.nodeArray[domainChange.Row(), domainChange.Column()].AddValue(domainChange.Value());
            }

            // Remove the invalid value from the domain of the node and add it to the domainchanges list of the previous change
            int i = domainChanges.Source().i;
            int j = domainChanges.Source().j;
            int value = this.nodeArray[i, j].Value();

            if (changes.Count != 0)
            {
                DomainChangeList lastChanges = changes.Pop();
                if (this.nodeArray[i, j].RemoveValue(value, true))
                {
                    lastChanges.AddDomainChange(new DomainChange(i, j, value));
                }
                changes.Push(lastChanges);
            } else
            {
                // When this is the first node, the value cannot and does not need to be added to the domainchanges list of the previous change
                this.nodeArray[i, j].RemoveValue(value, true);
            }

            // Return previous node taking into account if node is the last in the row
            return (j == 0) ? (i - 1, 8) : (i, j - 1);
        }
        
        private void EnsureNodeConsistency()
        {
            // Loop through rows
            for (int i = 0; i < SudokuSize; i++)
            {
                // Loop through columns
                for (int j = 0; j < SudokuSize; j++)
                {
                    // If the node is Fixed, propagate the value to those sudokuNodes in nodeArray that are not fixed and are in the same row, column, or block
                    if (this.nodeArray[i, j].IsFixed())
                    {
                        EnsureLocalNodeConsistency(i, j);
                    }
                }
            }
            // Clear stack for fresh start of chronological backtracking
            changes.Clear();
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
                    // Fill nodeArray with SudokuNodes, isFixed = true when a number other than 0 is the input
                    bool isFixed = inputArray[(i * SudokuSize) + j] != 0;
                    this.nodeArray[i, j] = new SudokuNode(inputArray[(i * SudokuSize) + j], isFixed);
                    if (isFixed) correctNodes++;
                }
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

                    Console.BackgroundColor = (this.nodeArray[i, j].Value() == 0) ? ConsoleColor.Red : ConsoleColor.Green;

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
                        Console.BackgroundColor = ConsoleColor.Black;
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
    }
}