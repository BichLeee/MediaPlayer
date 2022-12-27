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
using System.Collections.ObjectModel;

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
        ObservableCollection<IPlaylist> _myPlaylists = new ObservableCollection<IPlaylist>();
        
        ObservableCollection<ISong> _songList = new ObservableCollection<ISong>();

        bool isPlay = false;
        MyPlaylists myPlaylist ;
        RecentlyPlayed recentlyPlayed = new RecentlyPlayed();
        Teams team = new Teams();
        ISong CurrentPlaying = new ISong();
        int indexCurrent = 0;

        DispatcherTimer _timer;
        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = sidebar.SelectedItem as NavButton;
            int index = sidebar.SelectedIndex; 

            navframe.Visibility = Visibility;
            if (index == 1)
                navframe!.NavigationService.Navigate(myPlaylist);
            else if (index == 2)
                navframe!.NavigationService.Navigate(recentlyPlayed);
            else if (index == 3)
                navframe!.NavigationService.Navigate(team);
            else
            {
                navframe.Visibility = Visibility.Hidden;
            }
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
           sidebar.SelectedIndex = 0;

            _myPlaylists = new ObservableCollection<IPlaylist>()
            {
                new IPlaylist() { name = "Love Song", date = "20/12/2022", listSongs = null, history = null }
            };

            myPlaylist = new MyPlaylists(_myPlaylists,_songList);
            myPlaylist.PlaylistsChanged += MyPlaylist_PlaylistsChanged;
            myPlaylist.SongListChanged += MyPlaylist_SongListChanged;
        }

        private void MyPlaylist_SongListChanged(ObservableCollection<ISong> newValue)
        {
            if (newValue.Count>0)
            {
                _songList = newValue;
                CurrentPlaying = _songList[0];
                indexCurrent = 0;
                playMediaFile();

            }
            
        }

        private void MyPlaylist_PlaylistsChanged(ObservableCollection<IPlaylist> newValue)
        {
            _myPlaylists = newValue;
        }

        public void playMediaFile()
        {
            player.Source = new Uri(CurrentPlaying.path, UriKind.Absolute);
            player.Play();
            player.Stop();

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            _timer.Tick += _timer_Tick;

            PlayButton_Path.Data = Geometry.Parse("M6 5h4v14H6zm8 0h4v14h-4z");
            isPlay = true;
            player.Play();
            _timer.Start();

        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
       {
            if (CurrentPlaying.path==null)
                return;

            if (isPlay == false)
            {
                playMediaFile();

            }
            else
            {
                PlayButton_Path.Data = Geometry.Parse("M4610 6399 l0 -2881 43 25 c195 114 4144 2392 4494 2593 339 194 448 262 440 270 -7 7 -743 434 -1637 949 -894 516 -2001 1155 -2460 1420 -459 265 -845 487 -857 494 l-23 12 0 -2882z");
                isPlay = false;
                player.Pause();
                _timer.Stop();
               
            }
        }

        private void nextButton(object sender, RoutedEventArgs e)
        {
           
            indexCurrent += 1;
            if (indexCurrent >= _songList.Count)
                return; // che do lap lai chua code
            else
                CurrentPlaying = _songList[indexCurrent];

            playMediaFile();
        }



        private void _timer_Tick(object? sender, EventArgs e)
        {
            int hours = player.Position.Hours;
            int minutes = player.Position.Minutes;
            int seconds = player.Position.Seconds;
            currentPosition.Text = $"{hours}:{minutes}:{seconds}";

            if (player.Position.TotalSeconds > 0)
            {
                if (player.NaturalDuration.TimeSpan.TotalSeconds > 0)
                {
                    progressSlider.Value = player.Position.TotalSeconds;
                                    
                }
            }
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
            indexCurrent += 1;
            if (indexCurrent >= _songList.Count)
                return; // che do lap lai chua code
            else
                CurrentPlaying = _songList[indexCurrent];
            playMediaFile();
        }
    }
}
