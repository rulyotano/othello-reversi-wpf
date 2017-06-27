using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Othello.Logic.Common;
using Othello.Logic.Interfaces;

namespace Othello.Logic.Classes.Players
{
    public abstract class InteligentPlayer:IPlayer
    {

        protected const int CornerAward = 5;
        protected const int StableAward = 50;
        protected const int StableButNotFromCornerAward = 20;
        protected const int NearToCornerButNotCornerPenalization = -15;
        protected const int FrontierPenalization = -10;
        protected const int BorderAward = 5;
        protected const int MovilityFactor = 7;
        protected const int MiddleGameLevel = 46;
        protected const int EndGameLevel = 49;

        protected static MyTuple<int, int>[,] Corners =
            {
                {new MyTuple<int,int>(0,0),new MyTuple<int,int>(0,1),new MyTuple<int,int>(1,0),new MyTuple<int,int>(1,1)},
                {new MyTuple<int,int>(0,7),new MyTuple<int,int>(0,6),new MyTuple<int,int>(1,7),new MyTuple<int,int>(1,6)},
                {new MyTuple<int,int>(7,0),new MyTuple<int,int>(6,0),new MyTuple<int,int>(7,1),new MyTuple<int,int>(6,1)},
                {new MyTuple<int,int>(7,7),new MyTuple<int,int>(6,7),new MyTuple<int,int>(7,6),new MyTuple<int,int>(6,6)}
            };

        protected static MyTuple<int, int>[] Borders = { new MyTuple<int, int>(2, 0), new MyTuple<int, int>(3, 0), new MyTuple<int, int>(4, 0), new MyTuple<int, int>(5, 0) ,
                                                       new MyTuple<int,int>(0,2),new MyTuple<int,int>(0,3),new MyTuple<int,int>(0,4),new MyTuple<int,int>(0,5),
                                                       new MyTuple<int,int>(2,7),new MyTuple<int,int>(3,7),new MyTuple<int,int>(4,7),new MyTuple<int,int>(5,7),
                                                       new MyTuple<int,int>(7,2),new MyTuple<int,int>(7,3),new MyTuple<int,int>(7,4),new MyTuple<int,int>(7,5)};

        public PlayerKind PlayerKind { get; set; }

        public string PlayerName { get; set; }

        public abstract IMove GetNextMove(IBoard board);

        protected bool MaxIsWhite {
            get { return PlayerKind == PlayerKind.White; } 
        }

        protected abstract IComparer<IMove> MoveComparer { get; set; }

        protected int MaxLevel { get; set; }

        protected int OriginalMaxLevel { get; set; }

        protected int EndGameMaxLevel { get; set; }

        protected abstract int Utility(IBoard board);

        
    }
}
