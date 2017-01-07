using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TranslatorGUI
{
    public static class Commands
    {
        public static RoutedUICommand Load = new RoutedUICommand("_Load", "Load command", typeof(Commands));
        public static RoutedUICommand Save = new RoutedUICommand("_Save", "Save command", typeof(Commands));
        public static RoutedUICommand Exit = new RoutedUICommand("_Exit", "Exit command", typeof(Commands));

        public static RoutedUICommand LoadTranslator = new RoutedUICommand("Load translator", "Load translator command", typeof(Commands));

        public static RoutedUICommand ShowAST = new RoutedUICommand("Show abstract syntax tree", "Show abstract syntax tree command", typeof(Commands));

    }
}
