using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Othello.Logic.Common
{
    public class GameFinishedEventArgs:EventArgs
    {
        public int WhiteCount { get; set; }
        public int BlackCount { get; set; }
        public bool? WhiteWins { get; set; }
    }
}
