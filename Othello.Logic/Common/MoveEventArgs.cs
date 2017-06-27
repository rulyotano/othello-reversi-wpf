using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Othello.Logic.Interfaces;

namespace Othello.Logic.Common
{
    public class MoveEventArgs:EventArgs
    {
        public MoveEventArgs(IMove move)
        {
            Move = move;
        }
        public IMove Move { get; private set; }
    }
}
