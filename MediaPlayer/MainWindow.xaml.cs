using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Ulti;
using Interface;
using MediaPlayer.Pages;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Threading;
using System.Numerics;
using System.IO;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           //sidebar.SelectedIndex = 0;
        }

        private string _currentPlaying = string.Empty;
        bool isPlay = false;
       
        //MyPlaylists myPlaylist = new MyPlaylists();
        //RecentlyPlayed recentlyPlayed = new RecentlyPlayed();
        //Teams team = new Teams();`

        public delegate void PlayingValueChangeHandler(string newValue);
        public event PlayingValueChangeHandler PlayingChanged;

        

        Home home;
        DispatcherTimer _timer;
        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = sidebar.SelectedItem as NavButton;
            int index = sidebar.SelectedIndex;
   
            if (index == 0)
                navframe!.NavigationService.Navigate(home);
            //else if (index == 1)
            //    navframe!.NavigationService.Navigate(myPlaylist);
            //else if (index == 2)
            //    navframe!.NavigationService.Navigate(recentlyPlayed);
            //else if (index == 3)
            //    navframe!.NavigationService.Navigate(team);
            //else
            //{
            //    //do something
            //}
            else
             navframe!.Navigate(selected.Navlink);
           // navframe.Content = home;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           home = new Home(_currentPlaying);
           sidebar.SelectedIndex = 0;
        }


        private void PlayButton_Click(object sender, RoutedEventArgs e)
       {
            if (isPlay == false)
            {
                PlayButton_Path.Data = Geometry.Parse("M6 5h4v14H6zm8 0h4v14h-4z");
                isPlay = true;
                player.Play();
                _timer.Start();
               // home.play();

            }
            else
            {
                PlayButton_Path.Data = Geometry.Parse("M4610 6399 l0 -2881 43 25 c195 114 4144 2392 4494 2593 339 194 448 262 440 270 -7 7 -743 434 -1637 949 -894 516 -2001 1155 -2460 1420 -459 265 -845 487 -857 494 l-23 12 0 -2882z");
                isPlay = false;
                player.Pause();
                _timer.Stop();
                //home.pause();
            }
        }

        private void nextButton(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            if (screen.ShowDialog() == true)
            {
                _currentPlaying = screen.FileName;

                string[] info = GetAudioFileInfo(_currentPlaying);

                ISong song = new ISong(){
                   title = info[0], singer =info[1],time =0,path = _currentPlaying };

                player.Source = new Uri(_currentPlaying, UriKind.Absolute);
                player.Play();
                player.Stop();

                _timer = new DispatcherTimer();
                _timer.Interval = new TimeSpan(0, 0, 0, 1, 0); ;
                _timer.Tick += _timer_Tick;


                // home.currentPlaying = _currentPlaying;

            }
        }

        public string[] GetAudioFileInfo(string path)
        {
            path = Uri.UnescapeDataString(path);

            byte[] b = new byte[128];
            string[] infos = new string[5]; //Title; Singer; Album; Year; Comm;
            bool isSet = false;

            //Read bytes
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                fs.Seek(-128, SeekOrigin.End);
                fs.Read(b, 0, 128);
                //Set flag
                String sFlag = System.Text.Encoding.Default.GetString(b, 0, 3);
                if (sFlag.CompareTo("TAG") == 0) isSet = true;

                if (isSet)
                {
                    infos[0] = System.Text.Encoding.Default.GetString(b, 3, 30); //Title
                    infos[1] = System.Text.Encoding.Default.GetString(b, 33, 30); //Singer
                    //infos[2] = System.Text.Encoding.Default.GetString(b, 63, 30); //Album
                    //infos[3] = System.Text.Encoding.Default.GetString(b, 93, 4); //Year
                    //infos[4] = System.Text.Encoding.Default.GetString(b, 97, 30); //Comm
                }
                fs.Close();
                fs.Dispose();
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return infos;
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            int hours = player.Position.Hours;
            int minutes = player.Position.Minutes;
            int seconds = player.Position.Seconds;
            currentPosition.Text = $"{hours}:{minutes}:{seconds}";

        }

        private void progressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = progressSlider.Value;
            TimeSpan newPosition = TimeSpan.FromSeconds(value);
            player.Position = newPosition;

        }

        private void player_MediaOpened(object sender, RoutedEventArgs e)
        {
            int hours = player.NaturalDuration.TimeSpan.Hours;
            int minutes = player.NaturalDuration.TimeSpan.Minutes;
            int seconds = player.NaturalDuration.TimeSpan.Seconds;
            totalPosition.Text = $"{hours}:{minutes}:{seconds}";

            progressSlider.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;

        }

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {

        }
    }
}
