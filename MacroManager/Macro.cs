﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroManager
{ 
    class Macro
    {

    }

    class MacroEntry
    {

    }
}

/*
MACRO DAT FILE By Ioncannon

0x00: Unknown
0x02: Unknown
0x04: Size of file minux 0x20?
0x08: Size of macro book from 0x10.

The beginning of the file is still being decoded. After a 0x10 byte header, the macro entries begin. All macro entries
are XOR encoded with 0x73.
	
Each macro entry follows this format:

-Title
-Icon
-Key
-Line List

Each section begins with a marker (T, I, K, L), followed by two bytes (little endian) telling the size, and then the data. 
Data seems to be always null terminated. For example: 

A title: T 06 00 Greet\0
A icon: I 08 00 001024B\0
A key: K 04 00 005\0
A line: L 0A 00 Well met!\0

Each macro entry contains 1 title, 1 icon, 1 key, and 15 lines. All three of these entries must exist even if not used,
with data size being 0x01 for Titles and Lines, containing an empty string ("\0"). 

-Max line size is 0xB5.
-Max lines is 15.
-99 macros must exist, regardless if used (values are empty as above).
-Icons are hex values to the icon table. There is a exh file called "macroicon.exh" which contains all usable icons. The key MUST equal the row number of the icon. This is how SE fixed the 
issue of allowing icons outside this set to be used.

After 99 macros, the data is padded with 0s to 0x46000. This is most likely to allow the max amount of data to be filled
for all macros, lines, and titles.  
*/
