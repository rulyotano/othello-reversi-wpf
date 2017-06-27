using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using Othello.Logic.Common;

namespace Othello.ViewModel
{
    public class CellViewModel:ViewModelBase
    {
        #region X

        private int _X;

        public int X
        {
            get { return _X; }
            set
            {
                if (_X != value)
                {
                    _X = value;
                    RaisePropertyChanged("X");
                }
            }
        }

        #endregion

        #region Y

        private int _Y;

        public int Y
        {
            get { return _Y; }
            set
            {
                if (_Y != value)
                {
                    _Y = value;
                    RaisePropertyChanged("Y");
                }
            }
        }

        #endregion

        #region CellState

        private CellState _cellState;

        public CellState CellState
        {
            get { return _cellState; }
            set
            {
                if (_cellState != value)
                {
                    _cellState = value;
                    RaisePropertyChanged("CellState");
                }
            }
        }

        #endregion

        #region IsLastMove

        private bool _IsLastMove;
        private const string IsLastMoveName = "IsLastMove";

        public bool IsLastMove
        {
            get { return _IsLastMove; }
            set
            {
                if (_IsLastMove != value)
                {
                    _IsLastMove = value;
                    RaisePropertyChanged(IsLastMoveName);
                }
            }
        }

        #endregion


        #region IsPossibleNextMove

        private bool _isPossibleNextMove;
        private const string _isPossibleNextMoveName = "IsPossibleNextMove";
        public bool IsPossibleNextMove
        {
            get
            {
                var mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();
                if (mainVm.Board == null || !mainVm.IsAHummanPlayer)
                    return false;
                return mainVm.PlausiblesMoves != null &&
                       mainVm.PlausiblesMoves.Any(i => i.MovePosition.Item1 == X && i.MovePosition.Item2 == Y);

            }
            
        }

        #endregion

        public void Refresh()
        {
            RaisePropertyChanged(_isPossibleNextMoveName);
        }
    }
}
