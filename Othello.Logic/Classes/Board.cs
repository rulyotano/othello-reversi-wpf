using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Othello.Logic.Common;
using Othello.Logic.Interfaces;

namespace Othello.Logic.Classes
{
    public class Board:IBoard
    {
        private const int BoardSize = 8;
        private static readonly short[] dx = { -1, 0, 0, 1, 1, -1, 1, -1 }; //directions in x
        private static readonly short[] dy = { 0, 1, -1, 0, 1, -1, -1, 1 }; //directions in y

        #region Methods

        public void ConfigureNewGame()
        {
            BlackPoints.Clear();
            WhitePoints.Clear();
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    Positions[i, j] = CellState.Empty;
                }
            }
            WhitePoints.Add(new MyTuple<int, int>(3, 3));
            WhitePoints.Add(new MyTuple<int, int>(4, 4));
            BlackPoints.Add(new MyTuple<int, int>(3, 4));
            BlackPoints.Add(new MyTuple<int, int>(4, 3));
            foreach (var whitePoint in WhitePoints)
                Positions[whitePoint.Item1, whitePoint.Item2] = CellState.White;
            foreach (var blackPoint in BlackPoints)
                Positions[blackPoint.Item1, blackPoint.Item2] = CellState.Black;
        }

        public void MakeMove(IMove move)
        {
            if (move.IsPassMove)
                return;
            if (move.Player.PlayerKind == PlayerKind.Black)
            {
                Positions[move.MovePosition.Item1, move.MovePosition.Item2] = CellState.Black;
                BlackPoints.Add(move.MovePosition);     

                foreach (var convertedPoint in move.ConvertedPoints)
                {
                    Positions[convertedPoint.Item1, convertedPoint.Item2] = CellState.Black;
                    BlackPoints.Add(convertedPoint);    //add to black
                    WhitePoints.Remove(convertedPoint); //remove from white
                }
            }
            else
            {
                Positions[move.MovePosition.Item1, move.MovePosition.Item2] = CellState.White;
                WhitePoints.Add(move.MovePosition);

                foreach (var convertedPoint in move.ConvertedPoints)
                {
                    Positions[convertedPoint.Item1, convertedPoint.Item2] = CellState.White;
                    WhitePoints.Add(convertedPoint);    //add to black
                    BlackPoints.Remove(convertedPoint);    //remove to black
                }
            }
        }

        public void UnDoMove(IMove move)
        {
            if (move.IsPassMove)
                return;
            Positions[move.MovePosition.Item1, move.MovePosition.Item2] = CellState.Empty;
            if (move.Player.PlayerKind == PlayerKind.Black)
            {
                BlackPoints.Remove(move.MovePosition);

                foreach (var convertedPoint in move.ConvertedPoints)
                {
                    Positions[convertedPoint.Item1, convertedPoint.Item2] = CellState.White;    //swap back the points
                    BlackPoints.Remove(convertedPoint);
                    WhitePoints.Add(convertedPoint);
                }
            }
            else
            {
                WhitePoints.Remove(move.MovePosition);

                foreach (var convertedPoint in move.ConvertedPoints)
                {
                    Positions[convertedPoint.Item1, convertedPoint.Item2] = CellState.Black;    //swap back the points
                    WhitePoints.Remove(convertedPoint);
                    BlackPoints.Add(convertedPoint);
                }
            }
        }

        public IEnumerable<IMove> GetPlausibleMoves(IPlayer player)
        {
            var toRet = new List<IMove>();
            if (player.PlayerKind == PlayerKind.Black)
            {
                foreach (var blackPoint in BlackPoints) //for each blac point in the board
                {
                    for (byte i = 0; i < 8; i++) //check in all directions
                    {
                        IMove posibleMove;
                        GetDirectionMove(blackPoint.Item1 + dx[i], blackPoint.Item2 + dy[i], true, i, ref player,
                                         out posibleMove);
                        if (posibleMove != null)
                            toRet.Add(posibleMove);
                    }
                }
            }
            else    //is white
            {
                foreach (var whitePoint in WhitePoints) //for each black point in the board
                {
                    for (byte i = 0; i < 8; i++) //check in all directions
                    {
                        IMove posibleMove;
                        GetDirectionMove(whitePoint.Item1 + dx[i], whitePoint.Item2 + dy[i], true,  i, ref player,
                                         out posibleMove);
                        if (posibleMove != null)
                            toRet.Add(posibleMove);
                    }
                }
            }
            return FactorizeMoves(toRet);   //avoid multiples marking
        }

        private IEnumerable<IMove> FactorizeMoves(IEnumerable<IMove> moves)
        {
            var movesDict = new Dictionary<MyTuple<int, int>, IMove>();
            foreach (var move in moves)
            {
                if (!movesDict.ContainsKey(move.MovePosition))
                    movesDict.Add(move.MovePosition, move);
                else
                {   //if contains the move
                    foreach (var convertedPoint in move.ConvertedPoints)
                    {
                        movesDict[move.MovePosition].ConvertedPoints.Add(convertedPoint);
                    }
                }
            }
            return movesDict.Values.ToList();
        }

        #region Privates

        private bool IsInsideBoard(int x, int y)
        {
            return x >= 0 && y >= 0 && x < BoardSize && y < BoardSize;
        }

        /// <summary>
        /// It is a method that recursive iterate throught a direction while there is a enemy item. Return move if there is aviable move.
        /// </summary>
        /// <param name="x">current point x</param>
        /// <param name="y">current point y</param>
        /// <param name="firstItem">if is first item, for check not empty space at first path item</param>
        /// <param name="direction">directon to move, it is an index of dx, and dy arrays</param>
        /// <param name="player">player</param>
        /// <param name="move">result move, it is null if not</param>
        private void GetDirectionMove(int x, int y,bool firstItem, byte direction, ref IPlayer player,out IMove move)
        {
            //if position is out side
            if (!IsInsideBoard(x, y))
            {
                move = null;
                return;
            }
            //if there is a pice of the same collor in the middle
            if (Positions[x,y] == CellState.Black && player.PlayerKind == PlayerKind.Black || Positions[x,y] == CellState.White && player.PlayerKind == PlayerKind.White)
            {
                move = null;
                return;
            }
            //if is first item and is empty
            if (firstItem && Positions[x,y]== CellState.Empty)
            {
                move = null;
                return;
            }

            //found the the move, if i find a empty space in the path (in the middle only there are enemy pices)
            if (Positions[x,y]==CellState.Empty)
            {
                move = new Move(player) {MovePosition = new MyTuple<int, int>(x, y)};
                return;
            }

            //not enter in the base cases, then call recursive
            GetDirectionMove(x + dx[direction], y + dy[direction], false, direction, ref player, out move);

            //Add the current point to the solution (if found a move)
            if (move!=null)
            {
                move.ConvertedPoints.Add(new MyTuple<int, int>(x, y));
            }
        }

        #endregion


        #endregion

        #region Properties

        #region Positions

        private CellState[,] _positions;
        public CellState[,] Positions
        {
            get { return _positions??(_positions = new CellState[BoardSize,BoardSize]); }
            set { _positions = value; }
        }

        #endregion

        #region WhitePoints

        private IList<MyTuple<int, int>> _WhitePoints;


        public IList<MyTuple<int, int>> WhitePoints
        {
            get { return _WhitePoints??(_WhitePoints = new List<MyTuple<int, int>>()); }
            set { _WhitePoints = value; }
        }

        #endregion

        #region BlackPoints

        private IList<MyTuple<int, int>> _BlackPoints;
        public IList<MyTuple<int, int>> BlackPoints
        {
            get { return _BlackPoints ?? (_BlackPoints = new List<MyTuple<int, int>>()); }
            set { _BlackPoints = value; }
        }

        #endregion

        #endregion

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            var toRet = new Board();

            foreach (var whitePoint in WhitePoints)
            {
                toRet.Positions[whitePoint.Item1, whitePoint.Item2] = CellState.White;
                toRet.WhitePoints.Add(whitePoint.Clone() as MyTuple<int, int>);
            }

            foreach (var blackPoint in BlackPoints)
            {
                toRet.Positions[blackPoint.Item1, blackPoint.Item2] = CellState.Black;
                toRet.BlackPoints.Add(blackPoint.Clone() as MyTuple<int, int>);
            }
            return toRet;
        }

        public bool IsFrontier(int i, int j)
        {
            if (Positions[i,j] == CellState.Empty)
                return false;

            for (var k = 0; k < 8; k++)
            {
                if (IsInsideBoard(i + dx[k], j + dy[k]) && Positions[i + dx[k], j + dy[k]] == CellState.Empty)
                    return true;
            }

            return false;
        }

        public bool IsUnFlankeable(int i ,int j)
        {
            for (var k = 0; k < 8; k++)     //each direction
            {
                var result = !IsInsideBoard(i + dx[k], j + dy[k]) || RecursiveIsUnFlankeableInDirection(i + dx[k], j + dy[k], k);
                if (!result)
                    return false;
            }
            return true;
        }

        private bool RecursiveIsUnFlankeableInDirection(int i, int j, int direction)
        {
            bool otherInDirections = true;
            //Move first to corner and then check
            if (IsInsideBoard(i + dx[direction], j + dy[direction]))
                otherInDirections = RecursiveIsUnFlankeableInDirection(i + dx[direction], j + dy[direction], direction);
            
            //the probability of be blank in corner is high
            return Positions[i, j] != CellState.Empty && otherInDirections;
        }
        public override string ToString()
        {
            var stringB = new StringBuilder();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    stringB.Append(Positions[i, j] == CellState.Empty
                                       ? "0 "
                                       : Positions[i, j] == CellState.White ? "1 " : "2 ");
                }
                stringB.Append("\n");
            }

            return stringB.ToString();
        }
    }
}
