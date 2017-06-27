using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Othello.Logic.Common;

namespace Othello.Logic.Classes.Players
{
    public class MiniMaxAlphaBetaEvenHarder:MiniMaxAlphaBetaPlayer
    {
        public MiniMaxAlphaBetaEvenHarder(PlayerKind playerKind) : base(playerKind)
        {
            PlayerName = "Very Hard";
            OriginalMaxLevel = 7;
        }
    }
}
