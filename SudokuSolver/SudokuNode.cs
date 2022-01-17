using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class SudokuNode
    {
        private int value;
        private bool isFixed;
        List<int> allowedValues;

        public SudokuNode(int value, bool isFixed)
        {
            this.value = value;
            this.isFixed = isFixed;
            if (!isFixed)
            {
                this.allowedValues = new List<int>(Enumerable.Range(1, Sudoku.SudokuSize));
            }
        }

        public bool IsFixed()
        {
            return isFixed;
        }

        public int Value()
        {
            return value;
        }

        public bool RemoveValue(int value, bool resetValue = false)
        {
            if (resetValue && this.value == value)
            {
                this.value = 0;
            }
            return this.allowedValues.Remove(value);
        }

        public void AddValue(int value)
        {
            this.allowedValues.Add(value);
            this.allowedValues.Sort();
        }

        public bool SetFirstValue()
        {
            this.value = this.allowedValues.FirstOrDefault();
            return this.value != default;
        }

        public bool DomainIsEmpty()
        {
            return this.allowedValues.Count == 0;
        }
    }
}
