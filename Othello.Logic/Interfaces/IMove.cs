using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Othello.Logic.Common;

namespace Othello.Logic.Interfaces
{
    public interface IMove
    {
        bool IsPassMove { get; set; }
        IPlayer Player { get; set; }
        IList<MyTuple<int, int>> ConvertedPoints { get; set; }
        MyTuple<int, int> MovePosition { get; set; }
    }
}
