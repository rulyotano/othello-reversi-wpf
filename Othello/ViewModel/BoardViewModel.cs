using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Othello.Logic.Classes;
using Othello.Logic.Interfaces;

namespace Othello.ViewModel
{
    public class BoardViewModel:ViewModelBase
    {
        public BoardViewModel()
        {
            InitializeBoard();
        }

        public void InitializeBoard()
        {
            Board.ConfigureNewGame();
            var cellList = new ObservableCollection<CellViewModel>();
            Cells = cellList;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var cell = new CellViewModel {CellState = Board.Positions[i, j], X = i, Y = j};
                    cellList.Add(cell);
                    BoardOfCellViewModel[i, j] = cell;
                }
            }
            if (LastMove != null)
                LastMove.IsLastMove = false;
            LastMove = null;
        }

        #region Properties

        #region Board

        private IBoard _Board;

        public IBoard Board
        {
            get { return _Board ?? (_Board = new Board()); }
            set
            {
                if (_Board != value)
                {
                    _Board = value;
                    RaisePropertyChanged("Board");
                }
            }
        }

        #endregion

        #region BoardOfCellViewModel

        private CellViewModel[,] _boardOfCellViewModel;

        public CellViewModel[,] BoardOfCellViewModel
        {
            get { return _boardOfCellViewModel ?? (_boardOfCellViewModel = new CellViewModel[8,8]); }
            set
            {
                if (_boardOfCellViewModel != value)
                {
                    _boardOfCellViewModel = value;
                    RaisePropertyChanged("BoardOfCellViewModel");
                }
            }
        }

        #endregion

        #region Cells

        private ObservableCollection<CellViewModel> _Cells;

        public ObservableCollection<CellViewModel> Cells
        {
            get { return _Cells; }
            set
            {
                if (_Cells != value)
                {
                    _Cells = value;
                    RaisePropertyChanged("Cells");
                }
            }
        }

        #endregion

        #region LastMove

        private CellViewModel _LastMove;
        private const string LastMoveName = "LastMove";

        public CellViewModel LastMove
        {
            get { return _LastMove; }
            set
            {
                if (_LastMove != value)
                {
                    _LastMove = value;
                    RaisePropertyChanged(LastMoveName);
                }
            }
        }

        #endregion


        #endregion

        #region Methods

        public void DoMove(IMove move)
        {
            //Board.MakeMove(move);
            if (move.IsPassMove)
                return;
            if (move.Player.PlayerKind == Logic.Common.PlayerKind.Black)
            {
                BoardOfCellViewModel[move.MovePosition.Item1, move.MovePosition.Item2].CellState =
                    Logic.Common.CellState.Black;
                foreach (var convertedPoint in move.ConvertedPoints)
                {
                    BoardOfCellViewModel[convertedPoint.Item1, convertedPoint.Item2].CellState =
                        Logic.Common.CellState.Black;
                }
            }
            else
            {
                BoardOfCellViewModel[move.MovePosition.Item1, move.MovePosition.Item2].CellState =
                    Logic.Common.CellState.White;
                foreach (var convertedPoint in move.ConvertedPoints)
                {
                    BoardOfCellViewModel[convertedPoint.Item1, convertedPoint.Item2].CellState =
                        Logic.Common.CellState.White;
                }
            }
            if (LastMove != null)
                LastMove.IsLastMove = false;
            LastMove = BoardOfCellViewModel[move.MovePosition.Item1, move.MovePosition.Item2];
            LastMove.IsLastMove = true;
        }

        public void ReverseMove(IMove move)
        {
            Board.UnDoMove(move);
            if (move.IsPassMove)
                return;
            BoardOfCellViewModel[move.MovePosition.Item1, move.MovePosition.Item2].CellState =
                    Logic.Common.CellState.Empty;
            if (move.Player.PlayerKind == Logic.Common.PlayerKind.Black)
            {
                foreach (var convertedPoint in move.ConvertedPoints)
                {
                    BoardOfCellViewModel[convertedPoint.Item1, convertedPoint.Item2].CellState =
                        Logic.Common.CellState.White;
                }
            }
            else
            {
                foreach (var convertedPoint in move.ConvertedPoints)
                {
                    BoardOfCellViewModel[convertedPoint.Item1, convertedPoint.Item2].CellState =
                        Logic.Common.CellState.Black;
                }
            }
        }

        public void RefreshCells()
        {
            foreach (var cellViewModel in Cells)
            {
                cellViewModel.Refresh();
            }
        }
        #endregion

    }
}
