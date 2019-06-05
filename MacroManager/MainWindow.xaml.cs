using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using MacroManager.Properties;
using SaintCoinach;
using SaintCoinach.Imaging;
using Image = System.Windows.Controls.Image;
using File = SaintCoinach.IO.File;
using IconConverter = SaintCoinach.Ex.Relational.ValueConverters.IconConverter;
using ImageConverter = SaintCoinach.Imaging.ImageConverter;

namespace MacroManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
        public MacroFile CurrentMacroFile
        {
            get { return _currentMacroFile; }
            set
            {
                _currentMacroFile = value;
                OnPropertyChanged("CurrentMacroFile");
            }
        }
         */

        public MacroFile CurrentMacroFile { get; set; }
        public Macro SelectedMacro { get; set; }
        public List<MacroFile> Macros { get; set; }

        private ARealmReversed _realm;
        
        private string _gamePath = @"C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\";
        private string _datsPath = @"C:\Users\Liam\Documents\My Games\FINAL FANTASY XIV - A Realm Reborn\";

        public MainWindow()
        {
            try
            {
                //Environment.GetFolderPath(Environment.SpecialFolder.Mydocuments)
                InitializeComponent();
                DataContext = this;

                _realm = new ARealmReversed(_gamePath, SaintCoinach.Ex.Language.English);
                ResizeMode = ResizeMode.NoResize;

                List<string> dats = FindMacroDats();
                Macros = new List<MacroFile>();
                foreach (string path in dats)
                    Macros.Add(new MacroFile(path));

                CurrentMacroFile = Macros[0];

                InitGrid();
                //            TestImg();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                
            }


        }

        private void TestImg()
        {
            SelMacroIcon.Source = CurrentMacroFile.Macros[0].IconImage;
        }

        private void InitGrid()
        {
            for (int i = 0; i < 100; i++)
            {
                Thickness m = new Thickness(2, 2, 2, 2);

                Image im = new Image
                {
                    Margin = m,
                    Width = 32,
                    Height = 32
                };
                
                Binding imageBinding = new Binding
                {
                    Path = new PropertyPath($"CurrentMacroFile.Macros[{i}].IconImage"),
                    Mode = BindingMode.OneWay
                };
                BindingOperations.SetBinding(im, Image.SourceProperty, imageBinding);

                Grid.SetColumn(im, i % 10);
                Grid.SetRow(im, i / 10);

                im.MouseDown += ImOnMouseDown;

                System.Diagnostics.Debug.WriteLine($"Adding image at column {i % 10} row {i / 10}");

                MacroGrid.Children.Add(im);
            }
        }

        private List<string> FindMacroDats(string basePath = null)
        {
            string searchPath;
            if (basePath == null)
                searchPath = _datsPath;
            else
                searchPath = basePath;

            List<string> dats = new List<string>();

            List<string> pathes = new List<String>(Directory.GetFiles(searchPath));
            pathes.AddRange(Directory.GetDirectories(searchPath));

            foreach (string p in pathes)
            {
                if (System.IO.Directory.Exists(p))
                {
                    List<string> extras = FindMacroDats(p);
                    dats.AddRange(extras);
                }
                    
                else if (System.IO.File.Exists(p))
                {
                    if (p.EndsWith("MACROSYS.dat") || p.EndsWith("MACRO.DAT"))
                    {
                        dats.Add(p);
                    }
                }
                else
                {
                    // uh okay
                }
            }

            return dats;
        }

        private void ImOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image im = (Image) sender;
            int row = Grid.GetRow(im);
            int col = Grid.GetColumn(im);

            int index = col + row * 10;

            System.Diagnostics.Debug.WriteLine($"Mousedown on {index}");

            SelectedMacro = CurrentMacroFile.Macros[index];

            SelMacroTitle.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            SelMacroText.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            SelMacroIcon.GetBindingExpression(Image.SourceProperty)?.UpdateTarget();
        }

        private void LoadBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Current macro file path: {CurrentMacroFile.Path}");

            foreach (Image img in MacroGrid.Children)
                img.GetBindingExpression(Image.SourceProperty)?.UpdateTarget();
        }
    }
}
