using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Othello.Logic.Common;
using Othello.Logic.Interfaces;

namespace Othello.Logic.Classes.Players
{
    public class MiniMaxAlphaBetaPlayer:InteligentPlayer
    {
        public MiniMaxAlphaBetaPlayer(PlayerKind playerKind)
        {
            PlayerName = "Normal - Mini-Max with Alpha Beta";
            PlayerKind = playerKind;
            OriginalMaxLevel = 5;
        }

        #region MoveComparer
        private IComparer<IMove> _moveComparer;

        protected override IComparer<IMove> MoveComparer
        {
            get { return _moveComparer ?? (_moveComparer = new AlphaBetaMoveComparerDescending()); }
            set { _moveComparer = value; }
        }

        private class AlphaBetaMoveComparer:IComparer<IMove>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
            /// </returns>
            /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
            public int Compare(IMove x, IMove y)
            {
                if (x == null || x.IsPassMove)
                    return -1;
                if (y == null || y.IsPassMove)
                    return 1;


                return x.ConvertedPoints.Count() - y.ConvertedPoints.Count();
            }
        }

        private class AlphaBetaMoveComparerDescending : IComparer<IMove>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
            /// </returns>
            /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
            public int Compare(IMove x, IMove y)
            {
                if (x == null || x.IsPassMove)
                    return -1;
                if (y == null || y.IsPassMove)
                    return 1;


                return y.ConvertedPoints.Count() - x.ConvertedPoints.Count();
            }
        }

        protected class ComparerByUtilityBoardDescending : IComparer<IMove>
        {
            public IBoard Board { get; set; }

            public MiniMaxAlphaBetaPlayer Player { get; set; }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
            /// </returns>
            /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
            public int Compare(IMove x, IMove y)
            {
                if (x == null || x.IsPassMove)
                    return -1;
                if (y == null || y.IsPassMove)
                    return 1;
                Board.MakeMove(y);
                var yValue = Player.Utility(Board);
                Board.UnDoMove(y);

                Board.MakeMove(x);
                var xValue = Player.Utility(Board);
                Board.UnDoMove(x);

                return yValue - xValue;
            }
        }


        #endregion

        #region ComaprerByUtility

        private ComparerByUtilityBoardDescending _ComaprerByUtility;


        protected ComparerByUtilityBoardDescending ComaprerByUtility
        {
            get { return _ComaprerByUtility??(_ComaprerByUtility = new ComparerByUtilityBoardDescending{Player=this}); }
            set { _ComaprerByUtility = value; }
        }

        #endregion
        
        #region MiniMax-AlphaBeta-Prune

        public override IMove GetNextMove(IBoard board)
        {
            //Im the max player
            var maxResult = int.MinValue;
            IMove moveToDo = null;
            var rnd = new Random();

            var possiblesMoves = board.GetPlausibleMoves(this);

            //to list to avoid multiples iteration
            var possiblesMoveList = possiblesMoves as List<IMove> ?? possiblesMoves.ToList();

            if (possiblesMoves == null || !possiblesMoveList.Any())
                return new Move(true);

            //sort to get better results
            //for improve, the first level will be sorted by board utility
            ComaprerByUtility.Board = board;
            possiblesMoveList.Sort(ComaprerByUtility);
            //possiblesMoveList.Sort(new ComparerByUtilityBoardDescending { Player = this, Board = board });

            

            //Increment the deep search if game is advanced
            var count = board.BlackPoints.Count + board.WhitePoints.Count - 4;
            //MaxLevel = count >= MiddleGameLevel ? OriginalMaxLevel + 3 : count >= EndGameLevel ? EndGameLevel : OriginalMaxLevel;

            MaxLevel = count >= EndGameLevel
                           ? EndGameLevel
                           : count >= MiddleGameLevel ? OriginalMaxLevel + 2 : OriginalMaxLevel;
            
            if (possiblesMoveList.Count > 1)

            {
                //Start the threads
                var board1 = board.Clone() as IBoard;
                var board2 = board.Clone() as IBoard;
                var thread1GiveResult = false;
                var thread2GiveResult = false;

                var moveList1 = new List<IMove>();
                var moveList2 = new List<IMove>();
                var middleList = possiblesMoveList.Count / 2;
                for (int i = 0; i < middleList; i++)
                {
                    moveList1.Add(possiblesMoveList[i]);
                }
                for (int i = middleList; i < possiblesMoveList.Count; i++)
                {
                    moveList2.Add(possiblesMoveList[i]);
                }

                var thread1 = new BackgroundWorker();
                
                var thread2 = new BackgroundWorker();
                

                MyTuple<int,IMove> thread1Result = null;
                MyTuple<int,IMove> thread2Result = null;

                thread1.DoWork += (_, __) =>
                                      {
                                          thread1Result = MaxInThread(board1, moveList1);
                                          thread1GiveResult = true;
                                      };
                thread2.DoWork += (_, __) =>
                                      {
                                          thread2Result = MaxInThread(board2, moveList2);
                                          thread2GiveResult = true;
                                      };
                thread1.RunWorkerAsync();
                thread2.RunWorkerAsync();
                while(!thread1GiveResult || !thread2GiveResult) //while any of both thread working
                {
                    Thread.Sleep(200);
                }

                return thread2Result.Item1 > thread1Result.Item1 ? thread2Result.Item2 : thread1Result.Item2;
            }
            else
            {
                var result = MaxInThread(board, possiblesMoveList);
                return result.Item2;
            }
        }

        private MyTuple<int,IMove> MaxInThread(IBoard board, List<IMove> moves)
        {
            var maxResult = int.MinValue;

            MyTuple<int, IMove> toRet = null;

            var currentPlayer = this;
            var alpha = int.MinValue;
            var beta = int.MaxValue;
            var otherPlayer = new MiniMaxAlphaBetaPlayer(PlayerKind == PlayerKind.White
                                                             ? PlayerKind.Black
                                                             : PlayerKind.White);

            foreach (var move in moves)
            {
                board.MakeMove(move);

                var min = MinValue(ref board, otherPlayer, currentPlayer, 1, alpha, beta);
                if (maxResult < min)
                {
                    maxResult = min;
                    toRet = new MyTuple<int, IMove>(min, move);
                }

                board.UnDoMove(move);

                alpha = Math.Max(alpha, maxResult);
            }

            return toRet;
        }

        private int MaxValue(ref IBoard board, IPlayer currentPlayer, IPlayer otherPlayer, int level, int alpha,
                             int beta)
        {
            var successors = board.GetPlausibleMoves(currentPlayer);

            //to list to avoid possible multi enumeration
            var successorsList = successors as List<IMove> ?? successors.ToList();

            if (successors == null || !successorsList.Any()) //see if is a leaf node
            {
                var otherPlayerSuccessors = board.GetPlausibleMoves(otherPlayer);
                if (otherPlayerSuccessors == null || !otherPlayerSuccessors.Any()) //then is a leaf node
                {
                    //terminal state
                    var utility = BasicUtility(board);      //if is a game over states, use the real utility
                    return utility > 0 ? utility + 10000 : utility < 0 ? utility - 10000 : 0;
                        //give this the max or min values, bcos is a Win or Loos state
                }
//                else    //pass -> NEW
//                {
//                    return MinValue(ref board, otherPlayer, currentPlayer, level, alpha, beta);
//                    //the same level -> because has no sense count a level that doesn't cost
//                }
            }

            if (level == MaxLevel)
            {
                //max levet => terminal state
                return Utility(board);
            }

            //consider the pass move
            if (successorsList.Count <= 0)
                successorsList = new List<IMove> { new Move(true) };

            //successorsList.Sort(level <= 1 ? ComaprerByUtility : MoveComparer);
            successorsList.Sort(MoveComparer);

            

            //then the logic
            var v = int.MinValue;
            foreach (var s in successorsList)
            {
                //modify board
                board.MakeMove(s);

                //make the recursive call
                v = Math.Max(v, MinValue(ref board, otherPlayer, currentPlayer, level + 1, alpha, beta));

                //revert move in board
                board.UnDoMove(s);

                if (v > beta) //alhpa-beta prune
                    return v;
                alpha = Math.Max(alpha, v);
            }

            return v;
        }

        private int MinValue(ref IBoard board, IPlayer currentPlayer, IPlayer otherPlayer, int level, int alpha,
                             int beta)
        {
            var successors = board.GetPlausibleMoves(currentPlayer);

            //to list to avoid possible multi enumeration
            var successorsList = successors as List<IMove> ?? successors.ToList();

            if (successors == null || !successorsList.Any()) //see if is a leaf node
            {
                var otherPlayerSuccessors = board.GetPlausibleMoves(otherPlayer);
                if (otherPlayerSuccessors == null || !otherPlayerSuccessors.Any()) //then is a leaf node
                {
                    //terminal state
                    var utility = BasicUtility(board);      //if is a game over states, use the real utility
                    return utility > 0 ? utility + 10000 : utility < 0 ? utility - 10000 : 0;
                        //give this the max or min values, bcos is a Win or Loos state
                }
//                else    //pass -> NEW
//                {
//                    return MaxValue(ref board, otherPlayer, currentPlayer, level , alpha, beta);
//                    //the same level -> because has no sense count a level that doesn't cost
//                }
            }

            if (level == MaxLevel)
            {
                //max levet => terminal state
                return Utility(board);
            }

            //consider the pass move
            if (successorsList.Count <= 0)
                successorsList = new List<IMove> {new Move(true)};

            //successorsList.Sort(level <= 1 ? ComaprerByUtility : MoveComparer);
            successorsList.Sort(MoveComparer);


            //then the logic
            var v = int.MaxValue;
            foreach (var s in successorsList)
            {
                //modify board
                board.MakeMove(s);

                //make the recursive call
                v = Math.Min(v, MaxValue(ref board, otherPlayer, currentPlayer, level + 1, alpha, beta));

                //revert move in board
                board.UnDoMove(s);

                if (v <= alpha) //alhpa-beta prune
                    return v;
                beta = Math.Min(beta, v);
            }

            return v;
        }

        #endregion

        protected override int Utility(IBoard board)
        {
            var maxIsWhiteUtility = board.WhitePoints.Count - board.BlackPoints.Count;

            //award and penalize corners
            for (int i = 0; i < 4; i++)
            {
                var cornerState = board.Positions[Corners[i, 0].Item1, Corners[i, 0].Item2];
                if (cornerState == CellState.White)
                {
                    maxIsWhiteUtility += CornerAward;   //award white if have a corner
                }
                else
                {
                    for (int j = 1; j < 4; j++)
                    {
                        if (board.Positions[Corners[i, j].Item1, Corners[i, j].Item2] == CellState.White)
                            maxIsWhiteUtility += NearToCornerButNotCornerPenalization;  //penalize if is near to corner but not in corner
                    }
                }
                if (cornerState == CellState.Black)
                {
                    maxIsWhiteUtility -= CornerAward;   //award black if have a corner
                }
                else
                {
                    for (int j = 1; j < 4; j++)
                    {
                        if (board.Positions[Corners[i, j].Item1, Corners[i, j].Item2] == CellState.Black)
                            maxIsWhiteUtility -= NearToCornerButNotCornerPenalization;  //penalize if is near to corner but not in corner
                    }
                }
            }


            //price to borders
            foreach (var borderState in Borders.Select(b => board.Positions[b.Item1, b.Item2]))
            {
                switch (borderState)
                {
                    case CellState.White:
                        maxIsWhiteUtility += BorderAward;
                        break;
                    case CellState.Black:
                        maxIsWhiteUtility -= BorderAward;
                        break;
                }
            }

            //proccess aviables moves
            int whitesMoves = board.GetPlausibleMoves(new MiniMaxAlphaBetaPlayer(PlayerKind.White)).Count();
            int blackMoves = board.GetPlausibleMoves(new MiniMaxAlphaBetaPlayer(PlayerKind.Black)).Count();
            
            maxIsWhiteUtility += (whitesMoves - blackMoves)*MovilityFactor;

//            if (MaxIsWhite)
//                maxIsWhiteUtility -= (blackMoves * MovilityFactor);
//            else
//                maxIsWhiteUtility += (whitesMoves * MovilityFactor);

            int whitesStables, blackStables, whitesFrontiers, blackFrontiers, whiteStablesNotCorner, blackStablesNotCorner;

            GetUtilities(board, out whitesStables, out blackStables, out whitesFrontiers, out blackFrontiers, out whiteStablesNotCorner, out blackStablesNotCorner);

            maxIsWhiteUtility += (whitesStables*StableAward);
            maxIsWhiteUtility -= (blackStables*StableAward);

            maxIsWhiteUtility += (whiteStablesNotCorner*StableButNotFromCornerAward);
            maxIsWhiteUtility -= (blackStablesNotCorner*StableButNotFromCornerAward);

            maxIsWhiteUtility += (whitesFrontiers*FrontierPenalization);
            maxIsWhiteUtility -= (blackFrontiers * FrontierPenalization);

            return MaxIsWhite
                       ? maxIsWhiteUtility
                       : -maxIsWhiteUtility;
        }


        private int BasicUtility(IBoard board)
        {
            return MaxIsWhite
                       ? board.WhitePoints.Count - board.BlackPoints.Count
                       : board.BlackPoints.Count - board.WhitePoints.Count;
        }

        private static void GetUtilities(IBoard board, out int whitesStables, out int blackStables, out int whitesFrontiers, out int blackFrontiers, out int whitesNotCornerStables, out int blackNotCornerStables)
        {
            whitesStables = 0;
            blackStables = 0;
            whitesFrontiers = 0;
            blackFrontiers = 0;
            whitesNotCornerStables = 0;
            blackNotCornerStables = 0;

            var stables = new bool[8,8];
            var leftTopStables = LeftTopStabilityCorner(0, 0, stables, board);
            var leftBottomStables = LeftBottomStabilityCorner(7, 0, stables, board);
            var rightBottomStables = RightBottomStabilityCorner(7, 7, stables, board);
            var rightTopStables = RightTopStabilityCorner(0, 7, stables, board);

            switch (board.Positions[0, 0])
            {
                case CellState.White:
                    whitesStables += leftTopStables;
                    break;
                case CellState.Black:
                    blackStables += leftTopStables;
                    break;
            }

            switch (board.Positions[7, 0])
            {
                case CellState.White:
                    whitesStables += leftBottomStables;
                    break;
                case CellState.Black:
                    blackStables += leftBottomStables;
                    break;
            }

            switch (board.Positions[7, 7])
            {
                case CellState.White:
                    whitesStables += rightBottomStables;
                    break;
                case CellState.Black:
                    blackStables += rightBottomStables;
                    break;
            }

            switch (board.Positions[0, 7])
            {
                case CellState.White:
                    whitesStables += rightTopStables;
                    break;
                case CellState.Black:
                    blackStables += rightTopStables;
                    break;
            }

            whitesFrontiers = board.WhitePoints.Count(whitePoint => !stables[whitePoint.Item1, whitePoint.Item2] && board.IsFrontier(whitePoint.Item1, whitePoint.Item2));
            blackFrontiers = board.BlackPoints.Count(blackPoint => !stables[blackPoint.Item1, blackPoint.Item2] && board.IsFrontier(blackPoint.Item1, blackPoint.Item2));

            //get stables in the middle of the board
            //do not count it if is in the border
            whitesNotCornerStables += board.WhitePoints.Count(whitePoint => whitePoint.Item1 != 0 && whitePoint.Item1 != 7 && whitePoint.Item2 != 0 && whitePoint.Item2 != 7 && !stables[whitePoint.Item1, whitePoint.Item2] && board.IsUnFlankeable(whitePoint.Item1, whitePoint.Item2));

            blackNotCornerStables += board.BlackPoints.Count(blackPoint => blackPoint.Item1 != 0 && blackPoint.Item1 != 7 && blackPoint.Item2 != 0 && blackPoint.Item2 != 7 && !stables[blackPoint.Item1, blackPoint.Item2] && board.IsUnFlankeable(blackPoint.Item1, blackPoint.Item2));

            //now analize the estables in the borders that are not corner stables
            var foundedWhites = 0;
            var foundedBlack = 0;

            //check left border
            if (board.Positions[0,0] != CellState.Empty && board.Positions[7,0] != CellState.Empty)
            {
                for (var i = 1; i < 7; i++) //do not check corners, it is not necesary, always will be the corner color
                {
                    if (board.Positions[i, 0] == CellState.Empty)   //if the edge is not complete exit
                    {
                        foundedWhites = 0;
                        foundedBlack = 0;
                        break;
                    }
                    if (stables[i, 0])  //if already counted continue
                        continue;
                    if (board.Positions[i, 0] == CellState.White)   //if is white or black increment each one
                        foundedWhites++;
                    else
                        foundedBlack++;
                }
                whitesNotCornerStables += foundedWhites;
                blackNotCornerStables += foundedBlack;
            }

            foundedWhites = 0;
            foundedBlack = 0;

            //check top border
            if (board.Positions[0, 0] != CellState.Empty && board.Positions[0, 7] != CellState.Empty)
            {
                for (var j = 1; j < 7; j++) //do not check corners, it is not necesary, always will be the corner color
                {
                    if (board.Positions[0, j] == CellState.Empty)
                    {
                        foundedWhites = 0;
                        foundedBlack = 0;
                        break;
                    }
                    if (stables[0, j])  //if already counted continue
                        continue;
                    if (board.Positions[0,j] == CellState.White)   //if is white or black increment each one
                        foundedWhites++;
                    else
                        foundedBlack++;
                }
                whitesNotCornerStables += foundedWhites;
                blackNotCornerStables += foundedBlack;
            }

            foundedWhites = 0;
            foundedBlack = 0;

            //check right border
            if (board.Positions[0, 7] != CellState.Empty && board.Positions[7, 7] != CellState.Empty)
            {
                for (var i = 1; i < 7; i++) //do not check corners, it is not necesary, always will be the corner color
                {
                    if (board.Positions[i, 7] == CellState.Empty)
                    {
                        foundedWhites = 0;
                        foundedBlack = 0;
                        break;
                    }
                    if (stables[i, 7])  //if already counted continue
                        continue;
                    if (board.Positions[i, 7] == CellState.White)   //if is white or black increment each one
                        foundedWhites++;
                    else
                        foundedBlack++;
                }
                whitesNotCornerStables += foundedWhites;
                blackNotCornerStables += foundedBlack;
            }

            foundedWhites = 0;
            foundedBlack = 0;

            //check bottom border
            if (board.Positions[7, 7] != CellState.Empty && board.Positions[7, 0] != CellState.Empty)
            {
                for (var j = 1; j < 7; j++) //do not check corners, it is not necesary, always will be the corner color
                {
                    if (board.Positions[7, j] == CellState.Empty)
                    {
                        foundedWhites = 0;
                        foundedBlack = 0;
                        break;
                    }
                    if (stables[7, j])  //if already counted continue
                        continue;
                    if (board.Positions[7, j] == CellState.White)   //if is white or black increment each one
                        foundedWhites++;
                    else
                        foundedBlack++;
                }
                whitesNotCornerStables += foundedWhites;
                blackNotCornerStables += foundedBlack;
            }
            
        }

        private static int LeftTopStabilityCorner(int corneri, int cornerj, bool[,] marked, IBoard board)
        {
            var cornerState = board.Positions[corneri, cornerj];
            if (cornerState == CellState.Empty)
                return 0;
            var cornerStability = 0;

            if (!marked[corneri, cornerj])
            {
                cornerStability++;
                marked[corneri, cornerj] = true;
            }

            int i = corneri, j = cornerj;

            //check right
            while (j + 1 < 8 && board.Positions[i, j + 1] == cornerState && (i == 0 || marked[i - 1, j + 1]))   //while is inside, is same color of corner, and top is top is stable
            {
                if (!marked[i, j + 1])
                {
                    cornerStability++;
                    marked[i, j + 1] = true;
                }
                j++;
            }

            i = corneri;
            j = cornerj;

            //check bottom
            while (i + 1 < 8 && board.Positions[i + 1, j] == cornerState && (j == 0 || marked[i + 1, j - 1])) //while is inside, is same color of corner, and top is left is stable
            {
                if (!marked[i + 1, j])
                {
                    cornerStability++;
                    marked[i + 1, j] = true;
                }
                i++;
            }

            if (corneri + 1 < 8 && cornerj + 1 < 8 && marked[corneri, cornerj + 1] && marked[corneri + 1, cornerj] && board.Positions[corneri + 1, cornerj + 1] == cornerState)
                cornerStability += LeftTopStabilityCorner(corneri + 1, cornerj + 1, marked, board);

            return cornerStability;
        }

        private static int LeftBottomStabilityCorner(int corneri, int cornerj, bool[,] marked, IBoard board)
        {
            var cornerState = board.Positions[corneri, cornerj];
            if (cornerState == CellState.Empty)
                return 0;
            var cornerStability = 0;

            if (!marked[corneri, cornerj])
            {
                cornerStability++;
                marked[corneri, cornerj] = true;
            }

            int i = corneri, j = cornerj;

            //check right
            while (j + 1 < 8 && board.Positions[i, j + 1] == cornerState && (i == 7 || marked[i + 1, j + 1]))   //while is inside, is same color of corner, and top is bottom is stable
            {
                if (!marked[i, j + 1])
                {
                    cornerStability++;
                    marked[i, j + 1] = true;
                }
                j++;
            }

            i = corneri;
            j = cornerj;

            //check top
            while (i - 1 >= 0 && board.Positions[i - 1, j] == cornerState && (j == 0 || marked[i - 1, j - 1])) //while is inside, is same color of corner, and top is left is stable
            {
                if (!marked[i - 1, j])
                {
                    cornerStability++;
                    marked[i - 1, j] = true;
                }
                i--;
            }

            if (corneri - 1 >= 0 && cornerj + 1 < 8 && marked[corneri, cornerj + 1] && marked[corneri - 1, cornerj] && board.Positions[corneri - 1, cornerj + 1] == cornerState)
                cornerStability += LeftBottomStabilityCorner(corneri - 1, cornerj + 1, marked, board);

            return cornerStability;
        }

        private static int RightBottomStabilityCorner(int corneri, int cornerj, bool[,] marked, IBoard board)
        {
            var cornerState = board.Positions[corneri, cornerj];
            if (cornerState == CellState.Empty)
                return 0;
            var cornerStability = 0;

            if (!marked[corneri, cornerj])
            {
                cornerStability++;
                marked[corneri, cornerj] = true;
            }

            int i = corneri, j = cornerj;

            //check left
            while (j - 1 >= 0 && board.Positions[i, j - 1] == cornerState && (i == 7 || marked[i + 1, j - 1]))   //while is inside, is same color of corner, and top is bottom is stable
            {
                if (!marked[i, j - 1])
                {
                    cornerStability++;
                    marked[i, j - 1] = true;
                }
                j--;
            }

            i = corneri;
            j = cornerj;

            //check top
            while (i - 1 >= 0 && board.Positions[i - 1, j] == cornerState && (j == 7 || marked[i - 1, j + 1])) //while is inside, is same color of corner, and top is right is stable
            {
                if (!marked[i - 1, j])
                {
                    cornerStability++;
                    marked[i - 1, j] = true;
                }
                i--;
            }

            if (corneri - 1 >= 0 && cornerj - 1 >= 0 && marked[corneri, cornerj - 1] && marked[corneri - 1, cornerj] && board.Positions[corneri - 1, cornerj - 1] == cornerState)
                cornerStability += RightBottomStabilityCorner(corneri - 1, cornerj - 1, marked, board);

            return cornerStability;
        }

        private static int RightTopStabilityCorner(int corneri, int cornerj, bool[,] marked, IBoard board)
        {
            var cornerState = board.Positions[corneri, cornerj];
            if (cornerState == CellState.Empty)
                return 0;
            var cornerStability = 0;

            if (!marked[corneri, cornerj])
            {
                cornerStability++;
                marked[corneri, cornerj] = true;
            }

            int i = corneri, j = cornerj;

            //check left
            while (j - 1 >= 0 && board.Positions[i, j - 1] == cornerState && (i == 0 || marked[i - 1, j - 1]))   //while is inside, is same color of corner, and top is top is stable
            {
                if (!marked[i, j - 1])
                {
                    cornerStability++;
                    marked[i, j - 1] = true;
                }
                j--;
            }

            i = corneri;
            j = cornerj;

            //check bottom
            while (i + 1 < 8 && board.Positions[i + 1, j] == cornerState && (j == 7 || marked[i + 1, j + 1])) //while is inside, is same color of corner, and top is right is stable
            {
                if (!marked[i + 1, j])
                {
                    cornerStability++;
                    marked[i + 1, j] = true;
                }
                i++;
            }

            if (corneri + 1 < 8 && cornerj - 1 >= 0 && marked[corneri, cornerj - 1] && marked[corneri + 1, cornerj] && board.Positions[corneri + 1, cornerj - 1] == cornerState)
                cornerStability += RightTopStabilityCorner(corneri + 1, cornerj - 1, marked, board);

            return cornerStability;
        }


    }
}
