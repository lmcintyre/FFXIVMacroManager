﻿using System;
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
using SaintCoinach.Ex.Relational.ValueConverters;

namespace MacroManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IconConverter _Converter;

        public MainWindow()
        {
            InitializeComponent();

            _Converter = new IconConverter();
        }

        private void CreateGrid()
        {

        }

    }
}
