using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Othello.Logic.Common;

namespace Othello.Logic.Interfaces
{
    public interface IBoard:ICloneable
    {
        CellState[,] Positions { get; set; }
        void ConfigureNewGame();
        void MakeMove(IMove move);
        void UnDoMove(IMove move);
        IEnumerable<IMove> GetPlausibleMoves(IPlayer player);
        IList<MyTuple<int, int>> WhitePoints { get; set; }
        IList<MyTuple<int, int>> BlackPoints { get; set; }
        bool IsFrontier(int i, int j);
        bool IsUnFlankeable(int i, int j);
    }
}
