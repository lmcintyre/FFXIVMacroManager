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
        private IconConverter _Converter;
        private ARealmReversed _Realm;
        private List<MacroFile> _Macros;

        // SE... you have bytes right there. Please.
        private Dictionary<String, BitmapImage> _IconImages;

        private string _GamePath = @"C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\";
        private string _DatsPath = @"C:\Users\Liam\Documents\My Games\FINAL FANTASY XIV - A Realm Reborn\";

        public MainWindow()
        {
            //Environment.GetFolderPath(Environment.SpecialFolder.Mydocuments)
            InitializeComponent();

            _Realm = new ARealmReversed(_GamePath, SaintCoinach.Ex.Language.English);
            ResizeMode = ResizeMode.NoResize;

            LoadMacroImages();
            List<string> dats = FindMacroDats();
            _Macros = new List<MacroFile>();
            foreach (string path in dats)
                _Macros.Add(new MacroFile(path));

            loadBox.ItemsSource = _Macros;
            loadBox.DisplayMemberPath = "PrettyPath";
            loadBox.SelectedValuePath = ".";

            loadBox.SelectedIndex = 0;

            AddToGrid();
        }

        private void Load()
        {
            
        }

        private List<string> FindMacroDats(string basePath = null)
        {
            string searchPath;
            if (basePath == null)
                searchPath = _DatsPath;
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

        private void LoadMacroImages()
        {
            var micons = _Realm.GameData.GetSheet("MacroIcon");
            _IconImages = new Dictionary<string, BitmapImage>();

            foreach (var row in micons)
            {
                string key = $"{(int) row.GetRaw(0):X7}";
                ImageFile imgFile = IconHelper.GetIcon(row.Sheet.Collection.PackCollection, SaintCoinach.Ex.Language.English, (int) row.GetRaw(0));
                var tmp = ImageConverter.Convert(imgFile.GetData(), ImageFormat.A8R8G8B8_1, imgFile.Width, imgFile.Height);

                using (var ms = new MemoryStream())
                {
                    tmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;

                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = ms;
                    bi.EndInit();

                    if (!_IconImages.ContainsKey(key))
                        _IconImages.Add(key, bi);
                }
            }

        }

        private void AddToGrid()
        {
            MacroFile currentFile = (MacroFile) loadBox.SelectedValue;
            for (int i = 0; i < currentFile.Macros.Count; i++)
            {
                Macro currentMacro = currentFile.Macros[i];

                BitmapImage bi = null;
                if (!_IconImages.TryGetValue(currentMacro.Icon.Data.Trim(), out bi))
                    return;

                Image im = new Image {Source = bi};

                Grid.SetColumn(im, i % 10);
                Grid.SetRow(im, i / 10);

                Thickness m = im.Margin;
                m.Top = 2.0f;
                m.Bottom = 2.0f;
                m.Right = 2.0f;
                m.Left = 2.0f;
                im.Margin = m;

                System.Diagnostics.Debug.WriteLine($"Adding image at column {i % 10} row {i / 10}");
                macroGrid.Children.Add(im);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            macroGrid.Children.Clear();

            AddToGrid();
        }
    }
}
