using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Lexer.Core;
using FiniteStateRecognizer;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using CodeBoxControl.Decorations;
using Parser.Core;
using RecursiveDescentParser;
using FormalParser;
using CodeGeneration;

namespace TranslatorGUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Error> Errors { get; private set; }
        
        private SyntaxTree _syntaxTree;
        private IntermediateCode _code;
        private Translator _translator;

        private Dictionary<TokenType, Brush> _colorMap;
        private DateTime _nextRescanTime;
        private bool _modified;

        public MainWindow()
        {
            Errors = new ObservableCollection<Error>();

            InitializeComponent();

            _colorMap = new Dictionary<TokenType, Brush>();

            _colorMap[TokenType.Keyword] = new SolidColorBrush(Colors.Blue);
            _colorMap[TokenType.StringConstant] = new SolidColorBrush(Colors.SaddleBrown);
            _colorMap[TokenType.CharConstant] = new SolidColorBrush(Colors.Brown);
            _colorMap[TokenType.IntegerConstant] = new SolidColorBrush(Colors.DarkOrange);
            _colorMap[TokenType.FloatConstant] = new SolidColorBrush(Colors.Orange);
            _colorMap[TokenType.Operator] = new SolidColorBrush(Colors.DarkCyan);
            _colorMap[TokenType.Function] = new SolidColorBrush(Colors.SteelBlue);
            _colorMap[TokenType.Commentary] = new SolidColorBrush(Colors.Green);
            _colorMap[TokenType.Unknown] = new SolidColorBrush(Colors.Red);

            Task.Factory.StartNew(Updating);

            Init();
        }

        private void Invoke(Action action)
        {
            Dispatcher.Invoke(action);
        }

        private void Init()
        {
            // default translator
            _translator = MyTranslators.FormalTranslator;

            // load init file
            LoadFile("init.txt");
        }

        private void Updating()
        {
            while (true)
            {
                if (_modified && DateTime.Now > _nextRescanTime && _translator != null)
                {
                    Update();

                    _modified = false;
                }

                Thread.Sleep(300);
            }
        }

        private void PresentColoredTokens(IEnumerable<Token> tokens, IEnumerable<Error> errors)
        {
            SourceTextBox.Decorations.Clear();
            SourceTextBox.InvalidateVisual();

            foreach (var token in tokens)
            {
                if (_colorMap.ContainsKey(token.Type))
                {
                    ExplicitDecoration ed = new ExplicitDecoration();

                    ed.Start = token.Position.Value.Offset;
                    ed.Length = token.Value.Length;
                    ed.Brush = _colorMap[token.Type];
                    ed.DecorationType = EDecorationType.TextColor;

                    SourceTextBox.Decorations.Add(ed);
                }
            }

            foreach (var error in errors)
            {
                if (error.Token != null)
                {
                    if (error.Kind == ErrorKind.Syntax)
                    {
                        ExplicitDecoration ed = new ExplicitDecoration();
                        ExplicitDecoration ed2 = new ExplicitDecoration();

                        ed.Start = ed2.Start = error.Token.Position.Value.Offset;
                        ed.Length = ed2.Length = error.Token.Value.Length;

                        ed.DecorationType = EDecorationType.Hilight;
                        ed.Brush = new SolidColorBrush(Color.FromArgb(200, 255, 0, 0));

                        ed2.DecorationType = EDecorationType.TextColor;
                        ed2.Brush = new SolidColorBrush(Colors.White);

                        SourceTextBox.Decorations.Add(ed);
                        SourceTextBox.Decorations.Add(ed2);
                    }
                    else if (error.Kind == ErrorKind.Lexical)
                    {
                        ExplicitDecoration ed = new ExplicitDecoration();

                        ed.Start = error.Token.Position.Value.Offset;
                        ed.Length = error.Token.Value.Length;

                        ed.DecorationType = EDecorationType.Underline;
                        ed.Brush = Brushes.Red;

                        SourceTextBox.Decorations.Add(ed);
                    }
                    else if (error.Kind == ErrorKind.Semantic)
                    {
                        ExplicitDecoration ed = new ExplicitDecoration();

                        ed.Start = error.Token.Position.Value.Offset;
                        ed.Length = error.Token.Value.Length;

                        ed.DecorationType = EDecorationType.Hilight;
                        ed.Brush = new SolidColorBrush(Color.FromArgb(200, 0, 255, 0));

                        SourceTextBox.Decorations.Add(ed);
                    }
                }
            }

            SourceTextBox.InvalidateVisual();
        }

        private void PresentDescriptorsTable(StringList table, ListBox destinationList)
        {
            destinationList.Items.Clear();

            for (int i = 0; i < table.Count; i++)
            {
                destinationList.Items.Add(string.Format("{0}. {1}", i + 1, table[i]));
            }
        }

        private void PresentDescriptorsPresentation(DescriptorsPresentation dp)
        {
            PresentDescriptorsTable(dp.KeywordsList, KeywordsList);
            PresentDescriptorsTable(dp.OperatorsList, OperatorsList);
            PresentDescriptorsTable(dp.FunctionsList, FunctionsList);
            PresentDescriptorsTable(dp.IdentifiersList, IdentifiersList);
            PresentDescriptorsTable(dp.ConstantsList, ConstantsList);

            var sb = new StringBuilder();

            foreach (Token token in dp.Tokens)
            {
                if (token.HasType(TokenType.Delimiter))
                {
                    sb.Append(token.Value);
                }
                else if (!token.HasOneOfTypes(TokenType.Commentary, TokenType.Unknown, TokenType.EndOfText))
                {
                    StringList table = dp.GetTableFor(token);
                    string desc = string.Format("({0} {1})", table.Description, table.Find(token.Value.Escape()) + 1);

                    sb.Append(desc);
                }
            }

            DescriptorsRichTextBox.Document = new FlowDocument(new Paragraph(new Run(sb.ToString())));
        }

        private void Update()
        {
            try
            {
                string text = "";

                Invoke(() => { text = SourceTextBox.Text; });

                List<Error> errors = new List<Error>();

                #region Lexical analysis
                IEnumerable<Error> lexicalErrors = null;
                IEnumerable<Token> tokens = _translator.Scaner.GetTokens(text, out lexicalErrors);
                errors.AddRange(lexicalErrors); 
                #endregion

                #region Syntax analysis
                if (_translator.Parser != null && errors.Count == 0)
                {
                    IEnumerable<Error> syntaxErrors = null;
                    _syntaxTree = _translator.Parser.Parse(tokens, out syntaxErrors);
                    errors.AddRange(syntaxErrors);
                } 
                #endregion

                #region Semantic analysis
                if (_translator.SemanticChecker != null && errors.Count == 0)
                {
                    IEnumerable<Error> semanticErrors = _translator.SemanticChecker.GetSemanticErrors(_syntaxTree);
                    errors.AddRange(semanticErrors);
                }
                #endregion

                #region Codegeneration
                if (_translator.Generator != null && errors.Count == 0)
                {
                    _code = _translator.Generator.GenerateIntermediateCode(_syntaxTree);
                }
                #endregion

                var descriptorsPresentation = new DescriptorsPresentation(tokens);

                Invoke(() =>
                {
                    Errors.Clear();
                    errors.ForEach((e) => Errors.Add(e));

                    PresentColoredTokens(tokens, Errors);
                    PresentDescriptorsPresentation(descriptorsPresentation);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void SourceText_Changed(object sender, TextChangedEventArgs e)
        {
            if (SourceTextBox.Decorations.Count != 0)
            {
                SourceTextBox.Decorations.Clear();
                SourceTextBox.InvalidateVisual();
            }

            _nextRescanTime = DateTime.Now + TimeSpan.FromSeconds(1.0);
            _modified = true;
        }

        private void LoadFile(string fileName)
        {
            try
            {
                using (StreamReader initSourceCode = File.OpenText(fileName))
                {
                    SourceTextBox.Text = initSourceCode.ReadToEnd();
                }
            }
            catch (Exception)
            {
                MessageBox.Show(string.Format("Cannot load file {0}", fileName));
            }
        }

        private void SaveToFile(string fileName)
        {
            try
            {
                File.WriteAllText(fileName, SourceTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(string.Format("Cannot write file {0}", fileName));
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();

            if (sfd.ShowDialog() == true)
            {
                SaveToFile(sfd.FileName);
            }
        }

        private void Load_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                LoadFile(ofd.FileName);
            }
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void ShowAST_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_syntaxTree != null || _code != null)
            {
                var ASTViewer = new ASTViewer();

                if (_syntaxTree != null)
                    ASTViewer.LoadTree(_syntaxTree);

                if (_code != null)
                    ASTViewer.LoadCode(_code);

                ASTViewer.ShowDialog();
            }
        }

        private void LoadTranslator_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is Translator)
            {
                _translator = e.Parameter as Translator;

                SourceText_Changed(this, null);
            }
        }
    }
}