using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MacroManager.Annotations;
using SaintCoinach;
using SaintCoinach.Ex;
using SaintCoinach.Imaging;
using ImageConverter = SaintCoinach.Imaging.ImageConverter;

namespace MacroManager
{
    public enum EntryType
    {
        T,
        I,
        K, //unknown
        L
    };
    
    public struct MacroEntry
    {
        public EntryType Type;
        public string Data { get; set; }

        public MacroEntry(EntryType type, string macroData)
        {
            Type = type;
            Data = macroData;
        }
    }

    public class Macro
    {
        public BitmapImage IconImage { get; set; }

        public MacroEntry Title { get; set; }
        public MacroEntry Icon { get; set; }
        public MacroEntry Key { get; set; }
        public MacroEntry[] Lines { get; set; }

        public Macro(MacroEntry title, MacroEntry icon, MacroEntry key, MacroEntry[] lines)
        {
            Title = title;
            Icon = icon;
            Key = key;
            Lines = lines;
        }
    }

    public class MacroFile// : INotifyPropertyChanged
    {
        private static Dictionary<String, BitmapImage> _iconImages = null;

        public string Path { get; set; }
        public string PrettyPath { get; set; }
        public List<Macro> Macros { get; set; }

//        public event PropertyChangedEventHandler PropertyChanged;
//
//        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
//        {
//            add => throw new NotImplementedException();
//            remove => throw new NotImplementedException();
//        }

        private void LoadIcons()
        {
            var realm = new ARealmReversed(Properties.Settings.Default.GamePath, Language.English);
            var micons = realm.GameData.GetSheet("MacroIcon");
            _iconImages = new Dictionary<string, BitmapImage>();

            foreach (var row in micons)
            {
                string key = $"{(int)row.GetRaw(0):X7}";
                if (key == "0000000")
                    continue;
                ImageFile imgFile = IconHelper.GetIcon(row.Sheet.Collection.PackCollection, SaintCoinach.Ex.Language.English, (int)row.GetRaw(0));
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

                    if (!_iconImages.ContainsKey(key))
                        _iconImages.Add(key, bi);
                }
            }
        }

        public MacroFile(string path)
        {
            if (_iconImages == null)
                LoadIcons();

            Path = path;
            PrettyPath = path.EndsWith("MACROSYS.dat") ? "MACROSYS.dat" : "MACRO.DAT";
            //PrettyPath = path.Replace(Properties.Settings.Default.UserPath, "");

            Build();
        }

        private void Build()
        {
            byte[] data = System.IO.File.ReadAllBytes(Path);
            
            // We can skip to 0x08
            int offset = 8;

            ushort dataSize = BitConverter.ToUInt16(data, offset);
            offset += 2;

            while (data[offset] != 0xFF)
                offset++;
            offset++; // the FF

            XorBytes(data, offset, dataSize);

            Macros = new List<Macro>();
            while (offset < dataSize)
                Macros.Add(ReadMacro(data, ref offset));
        }

        private Macro ReadMacro(byte[] data, ref int offset)
        {
            MacroEntry t = new MacroEntry();
            t.Type = EntryType.T;
            MacroEntry i = new MacroEntry();
            i.Type = EntryType.I;
            MacroEntry k = new MacroEntry();
            k.Type = EntryType.K;
            MacroEntry[] lines = new MacroEntry[15];

            // we can actually ignore types, we know what comes after what
            offset++;

            byte titleSize = data[offset];
            offset += 2; // skip a null as well

            t.Data = Encoding.UTF8.GetString(data, offset, titleSize).TrimEnd('\0');
            offset += titleSize;

            offset++;

            byte iconSize = data[offset];
            offset += 2;

            i.Data = Encoding.UTF8.GetString(data, offset, iconSize).TrimEnd('\0');
            offset += iconSize;

            offset++;

            byte keySize = data[offset];
            offset += 2;

            k.Data = Encoding.UTF8.GetString(data, offset, keySize).TrimEnd('\0');
            offset += keySize;

            for (int j = 0; j < 15; j++)
            {
                lines[j] = new MacroEntry();
                offset++;
                byte thisLineSize = data[offset];
                offset += 2;
                lines[j].Data = Encoding.UTF8.GetString(data, offset, thisLineSize).TrimEnd('\0');
                offset += thisLineSize;
            }

            Macro toReturn = new Macro(t, i, k, lines);
            BitmapImage bi;
            _iconImages.TryGetValue(i.Data, out bi);
            toReturn.IconImage = bi;
            return toReturn;
        }

        private void XorBytes(byte[] data, int offset = 0, int length = -1)
        {
            if (length == 0)
                length = data.Length;

            for (int i = offset; i < length; i++)
            {
                data[i] ^= 0x73;
            }
                
        }

        private void BytesLength(MacroEntry e)
        {

        }

        private void BytesLength(Macro macro)
        {

        }

        private void BytesLength(List<Macro> macros)
        {

        }


        
    }
}
