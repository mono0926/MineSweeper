using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Mono.App.MineSweeper
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            Control.MouseRightButtonDown += (sender, e) =>
            {
                e.Handled = true;
            };
            //Control.MouseLeftButtonUp += (sender, e) =>
            //    {
            //        MessageBox.Show("aaa");
            //    };
        }
    }
}