using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.IO;
using MahApps.Metro.Controls;
using System.Net;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Div_0_Meter
{
    // Welcome Inside!

    public partial class MainWindow : MetroWindow
    {
        public List<DVM> DVMs = new List<DVM> { };
        public int selectedID { get; set; } = 0;
        bool autosave = true, changed = false, rusText=false, inPercent = false, mush = true, loaded = false, locked=false, rollLimited = true;
        DispatcherTimer clock, stopwatch, rand, randd;
        Stopwatch sw = new Stopwatch();
        string outputBuf = String.Empty, output = String.Empty, password = "El Psy Kongru";

        int styleNum = 0, counter = 0;

        public class DVM
        {
            public string ID { get; set; }
            public double ValueNow { get; set; }
            public double ValueGoal { get; set; }
            public DateTime StartLine { get; set; }
            public DateTime DeadLine { get; set; }
            public string CNotes { get; set; }
            public string Header { get; set; }

            public DVM()
            {
                ValueNow = 0;
                ValueGoal = 0;
                StartLine = DateTime.Now;
                DeadLine = DateTime.Now;
                CNotes = "";
                Header = "";
            }
            public DVM(string id)
            {
                ValueNow = 0;
                ValueGoal = 0;
                StartLine = DateTime.Now;
                DeadLine = DateTime.Now;
                CNotes = "";
                Header = "";
                ID = id;
            }
            public DVM(double valueNow, double valueGoal, DateTime startLine, DateTime deadLine, string cnotes, string header, string id)
            {
                ValueNow = valueNow;
                ValueGoal = valueGoal;
                StartLine = startLine;
                DeadLine = deadLine;
                CNotes = cnotes;
                Header = header;
                ID = id;
            }
        }

        public class Data
        {
            public List<DVM> DVMs { get; set; } = new List<DVM> { };
            public int SelectedID { get; set; }
            public bool RusText { get; set; }
            public bool Locked { get; set; }
            public int StyleNum { get; set; }

            public Data() { }
            public Data(List<DVM> dvms, int selectedID, bool rustext, int style, bool locked)
            {
                DVMs = dvms;
                SelectedID = selectedID;
                RusText = rustext;
                StyleNum = style;
                Locked = locked;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            if (!File.Exists("temp.dvm"))
            {
                DVMs.Add(new DVM());
                DVMs[0].ID = "";
            }
            else
            {
                string s = File.ReadAllText("temp.dvm");
                s = App.Crypt(s, "KuriGohan & Kamehameha");
                File.WriteAllText("temp.dvm", s);
                var loadData = new Data();
                XmlSerializer xs = new XmlSerializer(typeof(Data));
                using (var sr = new StreamReader("temp.dvm"))
                {
                    loadData = (Data)xs.Deserialize(sr);
                }
                DVMs = loadData.DVMs;
                selectedID = loadData.SelectedID;
                rusText = loadData.RusText;
                styleNum = loadData.StyleNum;
                locked = loadData.Locked;
            }
            if (locked) password = null;
            reload();

            clock = new DispatcherTimer(new TimeSpan(0,0,1), DispatcherPriority.Normal, delegate
            {
                display(DateTime.Now.ToString("HH:mm:ss"));
            }, this.Dispatcher);
            clock.Stop();
            stopwatch = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                display(String.Format("{0:00}:{1:00}:{2:00}", sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds));
            }, this.Dispatcher);
            stopwatch.Stop();
            rand = new DispatcherTimer(new TimeSpan(0,0,0,0,20), DispatcherPriority.Normal, delegate
            {
                display(String.Format("{0:F6}", (new Random().NextDouble() * new Random().Next(1, 11))));
                if (counter >= 7 && rollLimited) rand.Stop();
                counter++;
            }, this.Dispatcher); rand.Stop();
            randd = new DispatcherTimer(new TimeSpan(0,0,0,0,20), DispatcherPriority.Normal, delegate
            {
                display(String.Format("{0:F6}", (new Random().Next(0, 99999999))));
                if (counter >= 6) { display(output); randd.Stop(); }
                counter++;
            }, this.Dispatcher);
            refresh();
        }

        void reload()
        {
            loaded = false;
            nowNUD.Value = DVMs[selectedID].ValueNow;
            goalNUD.Value = DVMs[selectedID].ValueGoal;
            startlineDTP.SelectedDate = DVMs[selectedID].StartLine;
            deadlineDTP.SelectedDate = DVMs[selectedID].DeadLine;
            Title = DVMs[selectedID].Header;
            loaded = true;
        }

        Uri[] tubes = //both options available, just press F2 to switch
        {
            new Uri("resources/nixietube00.png", UriKind.Relative),
            new Uri("resources/0.png", UriKind.Relative),
            new Uri("resources/nixietube01.png", UriKind.Relative),
            new Uri("resources/1.png", UriKind.Relative),
            new Uri("resources/nixietube02.png", UriKind.Relative),
            new Uri("resources/2.png", UriKind.Relative),
            new Uri("resources/nixietube03.png", UriKind.Relative),
            new Uri("resources/3.png", UriKind.Relative),
            new Uri("resources/nixietube04.png", UriKind.Relative),
            new Uri("resources/4.png", UriKind.Relative),
            new Uri("resources/nixietube05.png", UriKind.Relative),
            new Uri("resources/5.png", UriKind.Relative),
            new Uri("resources/nixietube06.png", UriKind.Relative),
            new Uri("resources/6.png", UriKind.Relative),
            new Uri("resources/nixietube07.png", UriKind.Relative),
            new Uri("resources/7.png", UriKind.Relative),
            new Uri("resources/nixietube08.png", UriKind.Relative),
            new Uri("resources/8.png", UriKind.Relative),
            new Uri("resources/nixietube09.png", UriKind.Relative),
            new Uri("resources/9.png", UriKind.Relative),
            new Uri("resources/nixietube_nan.png", UriKind.Relative),
            new Uri("resources/off.png", UriKind.Relative),
            new Uri("resources/nixietube_dot.png", UriKind.Relative),
            new Uri("resources/point.png", UriKind.Relative),
        };

        private void display(string text) //just display values
        {
            outputBuf = text;
            while (text.Length < 8) text += " ";
            for (int i = 0; i < 8; i++)
            {
                switch (text[i])
                {
                    case '0':
                        chooseChamber(i, 0);
                        break;
                    case '1':
                        chooseChamber(i, 1);
                        break;
                    case '2':
                        chooseChamber(i, 2);
                        break;
                    case '3':
                        chooseChamber(i, 3);
                        break;
                    case '4':
                        chooseChamber(i, 4);
                        break;
                    case '5':
                        chooseChamber(i, 5);
                        break;
                    case '6':
                        chooseChamber(i, 6);
                        break;
                    case '7':
                        chooseChamber(i, 7);
                        break;
                    case '8':
                        chooseChamber(i, 8);
                        break;
                    case '9':
                        chooseChamber(i, 9);
                        break;
                    case ' ':
                        chooseChamber(i, 10);
                        break;
                    default:
                        chooseChamber(i, 11);
                        break;
                }
            }
        }

        public void refresh()
        {
            if (rusText)
            {
                if (nowL.Content.ToString() == "Now:")
                { nowL.Content = "Сейчас:"; nowM.Content = "Цель:"; }
                else if (nowL.Content.ToString() == "Startline:") { nowL.Content = "Начало:"; nowM.Content = "Дедлайн:"; }
            }
            else
            {
                if (nowL.Content.ToString() == "Сейчас:")
                { nowL.Content = "Now:"; nowM.Content = "Goal:"; }
                else if (nowL.Content.ToString() == "Начало:") { nowL.Content = "Startline:"; nowM.Content = "Deadline:"; }
            }

            double value = Convert.ToDouble(nowNUD.Value);
            double goalValue = Convert.ToDouble(goalNUD.Value);
            DateTime startLine = Convert.ToDateTime(startlineDTP.SelectedDate);
            DateTime deadLine = Convert.ToDateTime(deadlineDTP.SelectedDate);

            if (autosave&&loaded) save();

            if (goalValue == 0) //0 goals = 0 outputs.. kind of
            {
                display("");
                return;
            }
            double divergence;
            if (nowL.Content.ToString() == "Now:" || nowL.Content.ToString() == "Сейчас:") { divergence = value / goalValue; }
            else { TimeSpan dateRange1 = DateTime.Now - startLine; TimeSpan dateRange2 = deadLine - startLine; divergence = ((double)dateRange1.Days / (double)dateRange2.Days); }
            if (inPercent) divergence *= 100;
            output = String.Format("{0:F6}", divergence);
            display(String.Format("{0:F6}", divergence));
        }

        public void save()
        {
            DVMs[selectedID].ValueNow = Convert.ToDouble(nowNUD.Value);
            DVMs[selectedID].ValueGoal = Convert.ToDouble(goalNUD.Value);
            DVMs[selectedID].StartLine = Convert.ToDateTime(startlineDTP.SelectedDate);
            DVMs[selectedID].DeadLine = Convert.ToDateTime(deadlineDTP.SelectedDate);
            DVMs[selectedID].Header = Title;
            var saveData = new Data(DVMs, selectedID, rusText, styleNum, locked);
            XmlSerializer xs = new XmlSerializer(typeof(Data));
            TextWriter tw = new StreamWriter("temp.dvm");
            xs.Serialize(tw, saveData);
            tw.Close();
            string s = File.ReadAllText("temp.dvm");
            s = App.Crypt(s, "KuriGohan & Kamehameha");
            File.WriteAllText("temp.dvm", s);
        }

        private ImageSource inputPicture(int n)
        {
            switch (n)
            {
                case 0:
                    return new BitmapImage(tubes[0 + styleNum]);
                case 1:
                    return new BitmapImage(tubes[2 + styleNum]);
                case 2:
                    return new BitmapImage(tubes[4 + styleNum]);
                case 3:
                    return new BitmapImage(tubes[6 + styleNum]);
                case 4:
                    return new BitmapImage(tubes[8 + styleNum]);
                case 5:
                    return new BitmapImage(tubes[10 + styleNum]);
                case 6:
                    return new BitmapImage(tubes[12 + styleNum]);
                case 7:
                    return new BitmapImage(tubes[14 + styleNum]);
                case 8:
                    return new BitmapImage(tubes[16 + styleNum]);
                case 9:
                    return new BitmapImage(tubes[18 + styleNum]);
                case 10:
                    return new BitmapImage(tubes[20 + styleNum]);
                default:
                    return new BitmapImage(tubes[22 + styleNum]);
            }
        }
        private void chooseChamber(int n1, int n2)
        {
            switch (n1)
            {
                case 0:
                    nt1.Source = inputPicture(n2);
                    break;
                case 1:
                    nt2.Source = inputPicture(n2);
                    break;
                case 2:
                    nt3.Source = inputPicture(n2);
                    break;
                case 3:
                    nt4.Source = inputPicture(n2);
                    break;
                case 4:
                    nt5.Source = inputPicture(n2);
                    break;
                case 5:
                    nt6.Source = inputPicture(n2);
                    break;
                case 6:
                    nt7.Source = inputPicture(n2);
                    break;
                case 7:
                    nt8.Source = inputPicture(n2);
                    break;
            }
        }

        private void autoSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (autosave) { autosave = false; autosaveButtonImage.Source = new BitmapImage(new Uri("resources/button_off.png", UriKind.Relative)); }
            else { autosave = true; autosaveButtonImage.Source = new BitmapImage(new Uri("resources/button_on.png", UriKind.Relative)); }
        }

        private void nowNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (loaded)
            {
                if (clock != null && clock.IsEnabled) clock.Stop();
                if (stopwatch != null && stopwatch.IsEnabled) { stopwatch.Stop(); sw.Reset(); }
                refresh();
            }
        }

        private void goalNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (loaded)
            {
                if (clock != null && clock.IsEnabled) clock.Stop();
                if (stopwatch != null && stopwatch.IsEnabled) { stopwatch.Stop(); sw.Reset(); }
                refresh();
            }
        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e) //here goes hotkeys
        {
            var key = (e.Key == Key.System ? e.SystemKey : e.Key); //just needed that

            if ((new List<Key> { Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.NumPad0,
            Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9,
            Key.Up, Key.Down, Key.Left, Key.Right, Key.Tab}).Contains(e.Key)) return;
            else if (key == Key.Z && (Keyboard.Modifiers & (ModifierKeys.Alt)) == (ModifierKeys.Alt))
            {
                Environment.Exit(0); //fast way out of this app
                e.Handled = true;
            }
            else if (e.Key == Key.Enter) //press enter just to be sure
            {
                if (clock != null && clock.IsEnabled) clock.Stop();
                if (stopwatch != null && stopwatch.IsEnabled) { stopwatch.Stop(); sw.Reset(); }
                refresh();
            }
            else if (e.Key == Key.X) //Switch into deadline mode and back
            {
                if (clock != null && clock.IsEnabled) clock.Stop();
                if (stopwatch != null && stopwatch.IsEnabled) { stopwatch.Stop(); sw.Reset(); }
                if (mush) { counter = 0; randd.Start(); }
                if (nowNUD.Visibility == Visibility.Visible)
                {
                    nowNUD.Visibility = Visibility.Collapsed;
                    startlineDTP.Visibility = Visibility.Visible;
                    nowL.Content = "Startline:";
                    goalNUD.Visibility = Visibility.Collapsed;
                    deadlineDTP.Visibility = Visibility.Visible;
                    nowM.Content = "Deadline:";
                }
                else
                {
                    nowNUD.Visibility = Visibility.Visible;
                    startlineDTP.Visibility = Visibility.Collapsed;
                    nowL.Content = "Now:";
                    goalNUD.Visibility = Visibility.Visible;
                    deadlineDTP.Visibility = Visibility.Collapsed;
                    nowM.Content = "Goal:";
                }
                refresh();
            }
            else if (e.Key == Key.K)
            {
                if (locked) { locked = false; password = "El Psy Kongru"; }
                else { locked = true; password = null; }
                save();
                e.Handled = true;
            }
            else if ((key == Key.W && (Keyboard.Modifiers & (ModifierKeys.Alt)) == (ModifierKeys.Alt)) || (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.W))
            {
                notes notes = new notes(this, password, rusText); //opens notepad
                notes.Show();
                e.Handled = true;
            }
            else if (e.Key == Key.A) //Add value to currency one
            {
                try
                {
                    int result = Int32.Parse(this.ShowModalInputExternal("Add value", $""));
                    nowNUD.Value += result;
                }
                catch { }
                e.Handled = true;
            }
            else if ((key == Key.H && (Keyboard.Modifiers & (ModifierKeys.Alt)) == (ModifierKeys.Alt)) || (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.H))
            {
                string t;
                if (rusText) { t = "Введите новый заголовок окна"; }
                else { t = "Input new header"; }
                string result = this.ShowModalInputExternal(t, $"");
                if (result != null) Title = result;
                if (autosave) save();
                e.Handled = true;
            }
            else if (e.Key == Key.R) //random number
            {
                if (clock != null && clock.IsEnabled) clock.Stop();
                if (stopwatch != null && stopwatch.IsEnabled) { stopwatch.Stop(); sw.Reset(); }
                if (rand != null && rand.IsEnabled) rand.Stop();
                else
                {
                    counter = 0; rollLimited = false; outputBuf = "0,000000"; rand.Start();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Y) //starts stopwatch
            {
                if (clock != null && clock.IsEnabled) clock.Stop();

                if (stopwatch != null && stopwatch.IsEnabled) { stopwatch.Stop(); sw.Stop(); }
                else
                {
                    display(String.Format("{0:00}:{1:00}:{2:00}", sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds));
                    stopwatch.Start(); sw.Start();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.T) //starts clock
            {
                if (stopwatch != null && stopwatch.IsEnabled) { stopwatch.Stop(); sw.Reset(); }
                if (clock != null && clock.IsEnabled) clock.Stop(); //can also be stopped while running...
                else {
                    if (mush) { output = DateTime.Now.ToString("HH:mm:ss"); counter = 0; randd.Start(); }
                    else display(DateTime.Now.ToString("HH:mm:ss"));
                    clock.Start();
                } //...and started again
                e.Handled = true;
            }
            else if (e.Key == Key.D) //display current date
            {
                if (clock != null && clock.IsEnabled) clock.Stop();
                if (stopwatch != null && stopwatch.IsEnabled) { stopwatch.Stop(); sw.Reset(); }
                if (mush) { output = DateTime.Now.ToString("dd.MM.yy"); counter = 0; randd.Start(); }
                else display(DateTime.Now.ToString("dd.MM.yy"));
                e.Handled = true;
            }
            else if (key == Key.C) //copypaste everything tubes display!
            {
                Clipboard.Clear();
                Clipboard.SetText(outputBuf.Substring(0, 8)); //only 8, but can be changed (not recommended)
                e.Handled = true;
            }
            else if (e.Key == Key.S) //autosave toggle on/off
            {
                autoSaveButton_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.W)
            {
                try
                {
                    if (clock != null && clock.IsEnabled) clock.Stop();
                    if (stopwatch != null && stopwatch.IsEnabled) { stopwatch.Stop(); sw.Reset(); }
                    string[] data = Encoding.UTF8.GetString(Encoding.Default.GetBytes(new WebClient().DownloadString(@"https://docs.google.com/spreadsheets/d/1WWEX5R1BZkgLyvhEcZw2NEIlmle6cYn6MfMWwmmd01g/pubhtml?gid=434948351&single=true"))).Split('\u2248');
                    string translateProgress = "0." + data[data.Length - 1].Substring(0, 6).Trim(' ').Replace(",", "");
                    if (mush) { output = translateProgress; counter = 0; randd.Start(); }
                    else display(translateProgress);
                    e.Handled = true;
                } catch { }
            }
            else if ((key == Key.Q && (Keyboard.Modifiers & (ModifierKeys.Alt)) == (ModifierKeys.Alt)) || (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Q))
            {
                string t1, t2;
                if (rusText) { t1 = "Введите ID"; t2 = $"Чтобы сменить активную ячейку памяти"; }
                else { t1 = "Input ID"; t2 = $"To create/change active memory cell"; } 
                string result = this.ShowModalInputExternal(t1, t2);
                if (result != null)
                {
                    if (DVMs.Find(x => x.ID == result) == null) { DVMs.Add(new DVM(result)); }
                    selectedID = DVMs.FindIndex(x => x.ID == result);
                    if (result == "") selectedID = 0;
                    reload(); refresh();
                }
                e.Handled = true;
            }
            else if ((key == Key.R && (Keyboard.Modifiers & (ModifierKeys.Alt)) == (ModifierKeys.Alt)) || (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.R))
            {
                if (rusText) rusText = false;
                else rusText = true;
                refresh();
                e.Handled = true;
            }
            else if ((key == Key.A && (Keyboard.Modifiers & (ModifierKeys.Alt)) == (ModifierKeys.Alt)) || (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.A))
            { //you can check your age in double format...
                if (clock != null && clock.IsEnabled) clock.Stop();
                if (stopwatch != null && stopwatch.IsEnabled) { stopwatch.Stop(); sw.Reset(); }
                DateTime d1 = DateTime.Now;
                DateTime d2 = new DateTime(1996, 12, 27); //...by changing this to your birth age here
                TimeSpan time = d1 - d2;
                double year = time.Days / 365.25; //leap years included
                if (mush) { output = String.Format("{0:F6}", year); counter = 0; randd.Start(); }
                else display(String.Format("{0:F6}", year));
                e.Handled = true;
            }
            else if (e.Key == Key.F1) //got some help here
            {
                string helpText, helpHeader;
                if (rusText)
                {
                    helpHeader = "Горячие клавиши";
                    helpText =
                    "[X] Переключение в \"режим дедлайна\" и обратно\n" +
                    "[Y] Секундомер\n" +
                    "[T] Показать текущее время (часы)\n" +
                    "[D] Показать текущую дату\n" +
                    "[S] Автосохранение вкл/выкл\n" +
                    "[R] Случайное число от 0 до 9.99\n" +
                    "[H] Скрыть элементы справа\n" +
                    "[C] Скопировать выводимое значение\n" +
                    "[P] Значение в %\n" +
                    "[O] Поверх остальных окон\n" +
                    "[Alt+W] или [Ctrl+W] Открыть заметки\n" +
                    "[Alt+R] or [Ctrl+R] Switch to English\n" +
                    "[Alt+Z] Запасной выход\n" +
                    "[Alt+Space] Commander";
                } else
                {
                    helpHeader = "Hotkeys";
                    helpText = 
                    "[X] Switch to Deadline mode and back\n" +
                    "[T] Display local time (clock)\n" +
                    "[D] Display local date\n" +
                    "[S] Toggle autosave on/off\n" +
                    "[R] Display random number from 0 to 9.99\n" +
                    "[H] Hide right side elements\n" +
                    "[C] Copy displayed value\n" +
                    "[F] to Pay Respects\n" +
                    "[P] Value in %\n" +
                    "[O] Overlay mode\n" +
                    "[Alt+W] or [Ctrl+W] Open notes\n" +
                    "[Alt+R] или [Ctrl+R] Переключить на Русский\n" +
                    "[Alt+Z] Quick exit\n" +
                    "[Alt+Space] Commander";
                }
                notes notes = new notes(helpHeader, helpText);
                notes.Show();
                e.Handled = true;
            }
            else if (e.Key == Key.F2) //style changing.. nah, just tubes into older ones
            {
                if (styleNum == 0) styleNum++;
                else styleNum--;
                refresh();
                e.Handled = true;
            }
            else if (e.Key == Key.F) //Press F to Pay Respects
            {
                if (respects.IsVisible) { respects.Visibility = Visibility.Collapsed; nowNUD.Speedup = false; goalNUD.Speedup = false; }
                else { respects.Visibility = Visibility.Visible; nowNUD.Speedup = true; goalNUD.Speedup = true; }
                e.Handled = true;
            }
            else if (e.Key == Key.P) //Press P for percents
            {
                if (inPercent) inPercent = false;
                else inPercent = true;
                if (mush) { counter = 0; randd.Start(); }
                refresh();
                e.Handled = true;
            }
            else if (e.Key == Key.O)
            {
                if (Topmost) { Topmost = false; }
                else { Topmost = true; }
                e.Handled = true;
            }
            else if (e.Key == Key.M)
            {
                if (mush) { mush = false; }
                else { mush = true; }
                e.Handled = true;
            }
            else if (e.Key == Key.H) //hide dat controls on the right
            {
                if (controlColumn.IsVisible)
                {
                    controlColumn.Visibility = Visibility.Collapsed;
                }
                else
                {
                    controlColumn.Visibility = Visibility.Visible;
                }
            }
            else if ((key == Key.Space && (Keyboard.Modifiers & (ModifierKeys.Alt)) == (ModifierKeys.Alt)) || (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Space))
            {
                string t;
                if (rusText) { t = $"Введите свою команду"; }
                else { t = $"Input your command here"; }
                string result = this.ShowModalInputExternal("Commander", t);
                if (result != null)
                {
                    if (result.ToLower() == "idlist")
                    {
                        string text = String.Empty;
                        foreach (DVM d in DVMs) { text += $"{d.ID}\n"; }
                        notes notes = new notes("ID LIST", text);
                        notes.Show();
                    }
                    else if (result.ToLower() == "id")
                    {
                        string id = DVMs[selectedID].ID;
                        this.ShowMessageAsync("ID =", id);
                    }
                    else if (result.Length > 6 && result.Substring(0, 7).ToLower() == "copyto ")
                    {
                        if ((DVMs.Find(x => x.ID == result.Remove(0, 7)) == null) && (result.Remove(0, 7) != "")) { DVMs.Add(new DVM(result.Remove(0, 7))); }
                        DVM copy = DVMs[selectedID];
                        selectedID = DVMs.FindIndex(x => x.ID == result.Remove(0, 7));
                        if (result.Remove(0, 7) == "") selectedID = 0;
                        DVMs[selectedID].ValueNow = copy.ValueNow;
                        DVMs[selectedID].ValueGoal = copy.ValueGoal;
                        DVMs[selectedID].StartLine = copy.StartLine;
                        DVMs[selectedID].DeadLine = copy.DeadLine;
                        DVMs[selectedID].Header = copy.Header;
                        DVMs[selectedID].CNotes = copy.CNotes;
                        reload(); refresh();
                    }
                    else if (result.ToLower() == "delete" && selectedID > 0)
                    {
                        DVMs.RemoveAt(selectedID);
                        selectedID = 0;
                        reload(); refresh();
                    }
                    else if (result.ToLower() == "nullify"&&DVMs.Count>1) {
                        selectedID = 0;
                        while (DVMs.Count>1) { DVMs.RemoveAt(1); }
                        reload(); refresh();
                    }
                    else if (result.ToLower() == "help")
                    {
                        string helpText, helpHeader;
                        if (rusText)
                        {
                            helpHeader = "Справка[2]";
                            helpText =
                            "Горячие клавиши+:\n" +
                            "[F] Ускорение прокрутки\n" +
                            "[A] Добавить значение к полю \"Сейчас:\"\n" +
                            "[F1] Справка по горячим клавишам\n" +
                            "[Alt+H] Изменить заголовок окна\n" +
                            "[Alt+Q] Выбрать/создать ячейку памяти. Для этого ввести необходимый ID (для базовой ячейки оставить поле пустым)\n" +
                            "Каждая ячейка памяти хранит свой набор данных и имеет уникальный ID, который может быть любой строкой. Так можно хранить несколько независимых наборов значений в одном файле. Изначально создается одна базовая ячейка памяти (с пустым ID).\n" +
                            "\nКоманды:\n" +
                            "\"id\" - Показать ID текущей ячейки памяти\n" +
                            "\"idlist\" - Показать список всех использующихся ID\n" +
                            "\"copyto [ID]\" - Копировать текущие значения в другую ячейку памяти с заданным ID\n" +
                            "\"delete\" - Удалить текущую ячейку памяти (кроме базовой)\n" +
                            "\"nullify\" - Удалить все ячейки памяти (кроме базовой)";
                        }
                        else
                        {
                            helpHeader = "Help[2]";
                            helpText =
                            "Hotkeys+:\n" +
                            "[F] Speed up scrolling\n" +
                            "[A] Add value into \"Now:\" field\n" +
                            "[F1] Hotkeys help\n" +
                            "[Alt+H] Change window's header\n" +
                            "[Alt+Q] Choose/create memory cell. Requires ID (empty for basic cell)\n" +
                            "Each memory cell keep their own data and have an unique ID (any string). So, you may keep multiple values and switch them. There's also basic memory cell (with empty ID).\n" +
                            "\nCommands:\n" +
                            "\"id\" - Show current memory cell's ID\n" +
                            "\"idlist\" - Show all active ID's\n" +
                            "\"copyto [ID]\" - Copy current values into another memory cell with this ID\n" +
                            "\"delete\" - Delete current memory cell (but not basic)\n" +
                            "\"nullify\" - Delete all memory cells (but not basic)";
                        }
                        notes notes = new notes(helpHeader, helpText);
                        notes.Show();
                    }
                }
                e.Handled = true;
            }

        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e) //bring proportions 4 good
        {
            if (Width > Height * 3)
                Width = Height * 2.842;
            if (Height > Width / 3)
                Height = Width / 2.842;
            nowL.FontSize = Height / 14.25;
            nowM.FontSize = Height / 14.25;
            startlineDTP.FontSize = Height / 18.36;
            deadlineDTP.FontSize = Height / 18.36;
            nowNUD.FontSize = Height / 14.25;
            goalNUD.FontSize = Height / 14.25;
        }

        protected override void OnStateChanged(EventArgs e) //even in fullscreen
        {
            if (WindowState == WindowState.Maximized && !changed)
            {
                nowL.FontSize = 40;
                nowM.FontSize = 40;
                nowNUD.FontSize = 40;
                goalNUD.FontSize = 40;
                startlineDTP.FontSize = 50;
                deadlineDTP.FontSize = 50;
                changed = true;
                WindowState = System.Windows.WindowState.Normal;
            }
            else if (WindowState == WindowState.Normal && changed) { WindowState = System.Windows.WindowState.Maximized; changed = false; }
        }

        private void startlineDTP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (clock != null && clock.IsEnabled) clock.Stop();
            if (loaded) refresh();
        }

        private void deadlineDTP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (clock != null && clock.IsEnabled) clock.Stop();
            if (loaded) refresh();
        }
    }
}
