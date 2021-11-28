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

        public SudokuNode(int value, bool isFixed)
        {
            this.value = value;
            this.isFixed = isFixed;
        }

        public bool IsFixed()
        {
            return isFixed;
        }

        public int Value()
        {
            return value;
        }
    }
}
