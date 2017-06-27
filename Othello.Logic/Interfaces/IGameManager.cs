using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Othello.Logic.Common;

namespace Othello.Logic.Interfaces
{
    public interface IGameManager
    {
        IBoard Board { get; set; }
        IPlayer WhitePlayer { get; set; }
        IPlayer BlackPlayer { get; set; }
        IPlayer CurrentPlayerAtTurn { get; set; }
        bool GameStarted { get; set; }
        void StartGame();
        void ResetGame();
        bool IsPlaying { get; set; }
        bool IsEndGame { get; set; }
        void MoveNext();
        event EventHandler<MoveEventArgs> MoveDone;
        event EventHandler<GameFinishedEventArgs> GameFinished;

    }
}
