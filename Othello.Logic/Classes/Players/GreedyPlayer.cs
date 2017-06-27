using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Othello.Logic.Common;
using Othello.Logic.Interfaces;

namespace Othello.Logic.Classes.Players
{
    public class GreedyPlayer:InteligentPlayer
    {
        public GreedyPlayer(PlayerKind kind)
        {
            PlayerKind = kind;
        }

        public override IMove GetNextMove(IBoard board)
        {
            var moves = board.GetPlausibleMoves(this);
            var moveList = moves as List<IMove> ?? moves.ToList();
            Thread.Sleep(500);
            if (moves == null || !moveList.Any())
                return new Move(true);

            var corners = new List<MyTuple<int, int>> ();
            for (int i = 0; i < 4; i++)
            {
                corners.Add(Corners[i, 0]);
            }

            var cornersMoves = (from k in moveList where corners.Contains(k.MovePosition) select k).ToList();
            if (cornersMoves.Count > 0)
            {
                var crnIndex = (new Random()).Next(moveList.Count());
                return cornersMoves[crnIndex];
            }

            var bordersMoves = (from k in moveList where Borders.Contains(k.MovePosition) select k).ToList();
            if (bordersMoves.Count > 0)
            {
                var crnIndex = (new Random()).Next(bordersMoves.Count());
                return bordersMoves[crnIndex];
            }

            var rndIndex = (new Random()).Next(moveList.Count());
            return moveList[rndIndex];
        }

        protected override IComparer<IMove> MoveComparer { get; set; }
        protected override int Utility(IBoard board)
        {
            throw new NotImplementedException();
        }

        #region PlayerName

        private string _playerName = "Normal - Greedy";

        public string PlayerName
        {
            get { return _playerName; }
            set { _playerName = value; }
        }

        #endregion

    }
}
