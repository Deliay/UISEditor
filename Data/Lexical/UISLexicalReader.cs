using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISEditor.Data
{
    public class Reader
    {
        string codes;
        int currentPos = 0, currentLine = 0, currentLinePos = 0;
        public Reader(string code)
        {
            codes = code;
        }

        public int CurrentPosition { get => currentLinePos; }
        public int CurrentLineNumber { get => currentLine; }
        public int CurrentLinePosition { get => currentLinePos; }

        public char ReadChar()
        {
            char result = codes[currentPos++];
            if (result == (char)Tag.LINE_END)
            {
                currentLine++;
                currentLinePos = -1;
            }
            currentLinePos++;
            return codes[currentPos++];
        }

        public char PeekChar()
        {
            return codes[currentPos];
        }

        public bool isTerminalSymbol(char ch)
        {
            return Enum.IsDefined(typeof(Tag), ch);
        }

        public bool EOF()
        {
            return currentPos + 1 <= codes.Length;
        }

        public string ReadLine()
        {
            string cache = string.Empty;
            char cur = ReadChar();
            while (cur != (char)Tag.LINE_END)
            {
                cache += (cur);
                cur = ReadChar();
            }
            return cache;
        }

        public void Back() => currentPos--;

    }

}
