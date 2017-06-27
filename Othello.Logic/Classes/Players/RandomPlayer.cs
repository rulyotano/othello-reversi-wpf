using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Othello.Logic.Common;
using Othello.Logic.Interfaces;

namespace Othello.Logic.Classes.Players
{
    public class RandomPlayer:IPlayer
    {
        public RandomPlayer(PlayerKind playerKind)
        {
            PlayerKind = playerKind;
        }

        public PlayerKind PlayerKind { get; set; }

        public IMove GetNextMove(IBoard board)
        {
            var moves = board.GetPlausibleMoves(this);
            var moveList = moves as List<IMove> ?? moves.ToList();
            Thread.Sleep(500);
            if (moves==null || !moveList.Any())
                return new Move(true);

            var rndIndex = (new Random()).Next(moveList.Count());
            return moveList[rndIndex];
        }



        #region PlayerName
        private string _playerName = "Easy - Random";

        public string PlayerName
        {
            get { return _playerName; }
            set { _playerName = value; }
        }

        #endregion

    }
}
