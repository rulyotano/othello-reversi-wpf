using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Othello.Logic.Classes.Players;
using Othello.Logic.Common;
using Othello.Logic.Interfaces;

namespace Othello.Logic.Classes
{
    public class GameManager:IGameManager
    {
        #region Implementation of IGameManager

        public IBoard Board { get; set; }
        public IPlayer WhitePlayer { get; set; }
        public IPlayer BlackPlayer { get; set; }
        public IPlayer CurrentPlayerAtTurn { get; set; }
        public bool GameStarted { get; set; }
        public void StartGame()
        {
            ResetGame();
            GameStarted = true;
            CurrentPlayerAtTurn = BlackPlayer;
        }

        public void ResetGame()
        {
            Board.ConfigureNewGame();
            GameStarted = false;
            IsPlaying = false;
            IsEndGame = false;
        }

        public bool IsPlaying { get; set; }
        public bool IsEndGame { get; set; }
        private bool HasPlayerPassed { get; set; }

        public void MoveNext()
        {
            if (IsEndGame || IsPlaying || !GameStarted)
                return;
            
            IsPlaying = true;
            var nextMove = CurrentPlayerAtTurn.GetNextMove(Board);

            if (nextMove == null)   //if can't move 
                return; 

            if (nextMove.IsPassMove && HasPlayerPassed) //finish case
            {
                IsEndGame = true;
                
                var whites = Board.WhitePoints.Count;
                var blacks = Board.BlackPoints.Count;
                var gameFinishedEventArgs = new GameFinishedEventArgs {WhiteCount = whites, BlackCount = blacks};
                if (whites - blacks != 0)
                    gameFinishedEventArgs.WhiteWins = whites - blacks > 0;
                RaiseGameFinished(gameFinishedEventArgs);

                GameStarted = false;
                IsPlaying = false;
                return;
            }
            HasPlayerPassed = nextMove.IsPassMove;
            
            MakeMove(nextMove);
            IsPlaying = false;
        }

        private void MakeMove(IMove nextMove)
        {
            Board.MakeMove(nextMove);

            CurrentPlayerAtTurn = CurrentPlayerAtTurn == WhitePlayer ? BlackPlayer : WhitePlayer;

            RaiseMoveDone(new MoveEventArgs(nextMove));
        }

        #region MoveDone

        public event EventHandler<MoveEventArgs> MoveDone;
        

        private void RaiseMoveDone(MoveEventArgs e)
        {
            EventHandler<MoveEventArgs> handler = MoveDone;
            if (handler != null) handler(this, e);
        }

        #endregion

        #region GameFinished

        public event EventHandler<GameFinishedEventArgs> GameFinished;

        private void RaiseGameFinished(GameFinishedEventArgs e)
        {
            EventHandler<GameFinishedEventArgs> handler = GameFinished;
            if (handler != null) handler(this, e);
        }

        #endregion

        #endregion
    }
}
