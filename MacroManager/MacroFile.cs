using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MacroManager
{
    public enum EntryType
    {
        T,
        I,
        K, //unknown
        L
    };
    
    struct MacroEntry
    {
        public EntryType Type;
        public string Data;

        public MacroEntry(EntryType type, string macroData)
        {
            Type = type;
            Data = macroData;
        }
    }

    struct Macro
    {
        public MacroEntry Title;
        public MacroEntry Icon;
//        public Image IconImage; // for later :)
        public MacroEntry Key;
        public MacroEntry[] Lines;

        public Macro(MacroEntry title, MacroEntry icon, MacroEntry key, MacroEntry[] lines)
        {
            Title = title;
            Icon = icon;
            Key = key;
            Lines = lines;
        }
    }

    class MacroFile
    {
        public string Path { get; set; }
        public string PrettyPath { get; set; }
        public List<Macro> Macros { get; set; }

        public MacroFile(string path)
        {
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
            MacroEntry t;
            t.Type = EntryType.T;
            MacroEntry i;
            i.Type = EntryType.I;
            MacroEntry k;
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

            return new Macro(t, i, k, lines);
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
