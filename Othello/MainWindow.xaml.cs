using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Othello
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded+=(_,__)=>SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            var main = DataContext as MainViewModel;
            if (main == null)
                return;
            main.PropertyChanged+=(_,e)=>
                                      {
                                          if (e.PropertyName != "WhitePlaysNow") return;
                                          if (main.WhitePlaysNow == null)
                                              VisualStateManager.GoToState(turnControl, "NonePlaying", false);
                                          else if (main.WhitePlaysNow.Value)
                                              VisualStateManager.GoToState(turnControl, "PlayingWhite", false);
                                          else
                                              VisualStateManager.GoToState(turnControl, "PlayingBlack", false);
                                      };
        }
    }
}
