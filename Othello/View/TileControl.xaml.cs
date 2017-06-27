using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Othello.ViewModel;

namespace Othello.View
{
    /// <summary>
    /// Interaction logic for TileControl.xaml
    /// </summary>
    public partial class TileControl : UserControl
    {
        public TileControl()
        {
            InitializeComponent();
            DataContextChanged+=OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var cellVm = DataContext as CellViewModel;
            if (cellVm == null)
                return;
            cellVm.PropertyChanged += CellVmOnPropertyChanged;
            if (cellVm.CellState == Logic.Common.CellState.Empty)
                VisualStateManager.GoToState(this, "Empty", true);
            else if (cellVm.CellState == Logic.Common.CellState.White)
                VisualStateManager.GoToState(this, "White", true);
            else
                VisualStateManager.GoToState(this, "Black", true);

            if (cellVm.IsPossibleNextMove)
                VisualStateManager.GoToState(this, "IsPossibleMove", true);
            else
                VisualStateManager.GoToState(this, "NoIsPossibleMove", true);
        }

        

        private void CellVmOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var cellVm = sender as CellViewModel; 
            if (cellVm == null)
                return;
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "CellState":
                    {
                        if (cellVm.CellState == Logic.Common.CellState.Empty)
                            VisualStateManager.GoToState(this, "Empty", true);
                        else if (cellVm.CellState == Logic.Common.CellState.White)
                            VisualStateManager.GoToState(this, "White", true);
                        else
                            VisualStateManager.GoToState(this, "Black", true);
                        break;
                    }
                case "IsPossibleNextMove" :
                    {
                        if (cellVm.IsPossibleNextMove)
                            VisualStateManager.GoToState(this, "IsPossibleMove", true);
                        else
                            VisualStateManager.GoToState(this, "NoIsPossibleMove", true);
                        break;
                    }
            }
        }
    }
}
