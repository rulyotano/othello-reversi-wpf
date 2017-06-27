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
    public class HummanPlayer:IPlayer
    {
        public HummanPlayer(PlayerKind playerKind)
        {
            PlayerKind = playerKind;
        }

        public PlayerKind PlayerKind { get; set; }

        /// <summary>
        /// return null if is not a posible move
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public IMove GetNextMove(IBoard board)
        {
            var moves = board.GetPlausibleMoves(this);

            var moveList = moves as List<IMove> ?? moves.ToList();

            //Pass of no aviable moves
            if (moves == null || !moveList.Any())
                return new Move(true);

            //If human has no played
            while(HummanPositionToMove == null || moveList.All(i => i.MovePosition != HummanPositionToMove))
            {
                //empty loop waiting for humman move
                if (HummanPositionToMove != null && moveList.All(i => i.MovePosition != HummanPositionToMove))
                    HummanPositionToMove = null;
                Thread.Sleep(100);
            }

            var toRet = moveList.First(i => i.MovePosition == HummanPositionToMove);
            HummanPositionToMove = null;    //reset the Human move
            return toRet;
        }


        #region HummanPositionToMove

        public MyTuple<int, int> HummanPositionToMove { get; set; }

        #endregion

        #region PlayerName
        private string _playerName = "Human";

        public string PlayerName
        {
            get { return _playerName; }
            set { _playerName = value; }
        }

        #endregion
    }
}
