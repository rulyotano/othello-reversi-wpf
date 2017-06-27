using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Othello.Logic.Common;

namespace Othello.Logic.Classes.Players
{
    public class MiniMaxAlphaBetaPlayerHard:MiniMaxAlphaBetaPlayer
    {
        public MiniMaxAlphaBetaPlayerHard(PlayerKind playerKind) : base(playerKind)
        {
            OriginalMaxLevel = 6;
            PlayerName = "Hard - Mini-Max with Alpha Beta";
        }
    }
}
