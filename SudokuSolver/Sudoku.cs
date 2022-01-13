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

        private int amountOfSteps;
        private int currentBlockIndex;
        private int correctNodes;

        public Sudoku(string input)
        {
            this.nodeArray = new SudokuNode[SudokuSize, SudokuSize];
            this.amountOfSteps = 0;

            // Generate and display starting state
            ConvertInputToNodeArray(input);

            // Knoopconsistentie Constraints aanmaken op nodeArray
            EnsureNodeConsistency();
            Console.WriteLine("Test"); 
        }

        public void Solve()
        {
            // CBT
            // de zoekboom wordt dynamisch gegenereerd door knopen in een
            // depth - first volgorde te expanderen
            // Loop vanaf links naar rechts en boven naar onder
            // Als nieuw blad geen partiele oplossing biedt, haal uit allowedValues
            // Loop through rows
            for (int i = 0; i < SudokuSize; i++)
            {
                // Loop through columns
                for (int j = 0; j < SudokuSize; j++)
                {
                    nodeArray[i, j].SetFirstValue();
                    EnsureLocalNodeConsistency(i, j, true);
                }
            }
            amountOfSteps++;
            
        }

        public int GetAmountOfSteps()
        {
            return amountOfSteps;
        }

        private void EnsureLocalNodeConsistency(int i, int j, bool keepTrack = false)
        { 
            if (keepTrack)
            {
                List<(int i, int j, int value)> changes = new List<(int i, int j, int value)>();
            }
            // Loop door rijen en kolommen
            for (int k = 0; k < SudokuSize; k++)
            {
                if (!this.nodeArray[i, k].IsFixed())
                {
                    this.nodeArray[i, k].RemoveValue(this.nodeArray[i, j].Value());
                }
                if (!this.nodeArray[k, j].IsFixed())
                {
                    this.nodeArray[k, j].RemoveValue(this.nodeArray[i, j].Value());
                }
            }
            // Loop door blok
            int blockRowIndex = (i / SudokuBlockSize) * SudokuBlockSize;
            int blockColumnIndex = (j / SudokuBlockSize) * SudokuBlockSize;

            for (int l = blockRowIndex; l < blockRowIndex + SudokuBlockSize; l++)
            {
                for (int m = blockColumnIndex; m < blockColumnIndex + SudokuBlockSize; m++)
                {
                    if (!this.nodeArray[l, m].IsFixed())
                    {
                        this.nodeArray[l, m].RemoveValue(this.nodeArray[i, j].Value());
                    }
                }
            }
        }

        private void EnsureNodeConsistency()
        {

            // Loop through rows
            for (int i = 0; i < SudokuSize; i++)
            {
                // Loop through columns
                for (int j = 0; j < SudokuSize; j++)
                {
                    // Als node isFixed, propageer value naar sudokuNodes die !isFixed zijn in zelfde rij, kolom, blok, haal uit allowedValues
                    if (this.nodeArray[i, j].IsFixed())
                    {
                        EnsureLocalNodeConsistency(i, j);
                    }
                }
            }
        }

        //private SudokuNode GetNodeAtPosition(int i, int j)
        //{
        //    return this.nodeArray[CalculateRow(i), CalculateColumn(j)];
        //}

        //private int CalculateRow(int i)
        //{
        //    return 3 * (this.currentBlockIndex / 3) + i;
        //}

        //private int CalculateColumn (int j)
        //{
        //    return 3 * (this.currentBlockIndex % 3) + j;
        //}

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

                    //if (correctRows.Contains(i) && correctColumns.Contains(j))
                    //{
                    //    Console.BackgroundColor = ConsoleColor.Green;
                    //} else if (correctRows.Contains(i) || correctColumns.Contains(j))
                    //{
                    //    Console.BackgroundColor = ConsoleColor.Yellow;
                    //} else
                    //{
                    //    Console.BackgroundColor = ConsoleColor.Red;
                    //}
                    Console.BackgroundColor = ConsoleColor.Green;

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
    }
}