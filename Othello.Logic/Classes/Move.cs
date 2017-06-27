using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Othello.Logic.Common;
using Othello.Logic.Interfaces;

namespace Othello.Logic.Classes
{
    public class Move:IMove
    {
        #region Constructors

        public Move(bool isPassMove)
        {
            IsPassMove = isPassMove;
            if (!IsPassMove)
            {
                ConvertedPoints = new List<MyTuple<int, int>>();
            }
        }

        public Move(IPlayer player, bool isPassMove) : this(isPassMove)
        {
            Player = player;
        }

        public Move(IPlayer player) : this(player, false)
        {
        }

        public Move() : this(null)
        {
        }

        #endregion

        #region Implementation of IMove

        public bool IsPassMove { get; set; }
        public IPlayer Player { get; set; }
        public IList<MyTuple<int, int>> ConvertedPoints { get; set; }
        public MyTuple<int, int> MovePosition { get; set; }

        #endregion
    }
}
