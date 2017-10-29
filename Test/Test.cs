using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISEditor.Test
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Data.UISInstance test;
            Data.UISParser.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cat.mui"));
            test = Data.UISParser.ParseInstance();
        }
    }
}
