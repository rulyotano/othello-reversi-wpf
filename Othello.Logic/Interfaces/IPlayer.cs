using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Othello.Logic.Common;

namespace Othello.Logic.Interfaces
{
    public interface IPlayer
    {
        PlayerKind PlayerKind { get; set; }
        IMove GetNextMove(IBoard board);
        string PlayerName { get; set; }
    }
}
