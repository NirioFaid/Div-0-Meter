using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using MahApps.Metro.Controls;
using System.Text.RegularExpressions;

namespace Div_0_Meter
{
    public partial class notes : MetroWindow
    {
        public notes(string title, string text) //for 'help' window
        {
            Grid grid1 = new Grid();
            grid1.Background = new SolidColorBrush(Colors.Black);
            AddChild(grid1);
            Width = 350;
            Height = 360;
            notesRTB = new RichTextBox();
            notesRTB.Foreground = new SolidColorBrush(Colors.Orange);
            notesRTB.Background = new SolidColorBrush(Colors.Black);
            notesRTB.FontSize = 16;
            Paragraph p = notesRTB.Document.Blocks.FirstBlock as Paragraph;
            p.Margin = new Thickness(0);
            grid1.Children.Add(notesRTB);
            notesRTB.AppendText(text);
            notesRTB.Visibility = Visibility.Visible;
            notesRTB.IsReadOnly = true;
            notesRTB.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            notesRTB.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            ResizeMode = ResizeMode.NoResize;
            Title = title;
        }
        public notes(MainWindow mw, string password, bool rusText) //for 'notes' window
        {
            InitializeComponent();
            if (rusText) this.Title = "Заметки";
            mainwindow = mw;
            if (password == null || password == "") return;
            this.password = password;
            notesRTB.Document.Blocks.Clear();
            string text = mainwindow.DVMs[mainwindow.selectedID].CNotes;
            notesRTB.AppendText(text);
            loaded = true;
        }

        MainWindow mainwindow = null;
        string password = null; //can be changed btw
        bool loaded = false;

        private static Regex _invalidXMLChars = new Regex(
    @"(?<![\uD800-\uDBFF])[\uDC00-\uDFFF]|[\uD800-\uDBFF](?![\uDC00-\uDFFF])|[\x00-\x08\x0B\x0C\x0E-\x1F\x7F-\x9F\uFEFF\uFFFE\uFFFF]",
    RegexOptions.Compiled);

        private void notesRTB_TextChanged(object sender, TextChangedEventArgs e) //forced to autosave
        {
            if (password == null||password == "") return;
            if (loaded) {
                string t = new TextRange(notesRTB.Document.ContentStart, notesRTB.Document.ContentEnd).Text;
                t = _invalidXMLChars.Replace(t, "");
                mainwindow.DVMs[mainwindow.selectedID].CNotes = t;
                mainwindow.save();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) //just for quick exit
        {
            var key = (e.Key == Key.System ? e.SystemKey : e.Key);
            if (key == Key.Z && (Keyboard.Modifiers & (ModifierKeys.Alt)) == (ModifierKeys.Alt))
            {
                this.Close();
                e.Handled = true;
            }
        }
    }
}
