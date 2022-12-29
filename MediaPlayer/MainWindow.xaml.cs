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
using System.ComponentModel;

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
        

        public bool isPlay = false;
        MyPlaylists myPlaylist ;
        RecentlyPlayed recentlyPlayed = new RecentlyPlayed();
        Teams team = new Teams();

        ListSong listSong = new();
        ISong CurrentPlaying = new ISong();

        BindingList<ISong> recentlyList;
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
        public void saveRecently()
        {
            ISong newFile =(ISong) CurrentPlaying.Clone();
            newFile.currentTime = player.Position.TotalSeconds;

            foreach(var song in recentlyList)
            {
                if(newFile.title == song.title&& newFile.singer == song.singer && newFile.path == song.path)
                {
                    recentlyList.Remove(song);
                }
            }
            recentlyList.Reverse();
            recentlyList.Add(newFile);
            recentlyList.Reverse();

            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
           sidebar.SelectedIndex = 0;
            player.Volume = (double)volumeSlider.Value;

            _myPlaylists = new ObservableCollection<IPlaylist>()
            {
                new IPlaylist() { name = "Love Song", date = "20/12/2022", listSongs = null, }
            };

            myPlaylist = new MyPlaylists(_myPlaylists, listSong);
            myPlaylist.PlaylistsChanged += MyPlaylist_PlaylistsChanged;
            myPlaylist.SongListChanged += MyPlaylist_SongListChanged; ;
        }

        private void MyPlaylist_SongListChanged(ListSong newValue)
        {
            if (newValue.listSongs.Count > 0)
            {
                listSong = (ListSong) newValue.Clone();
                CurrentPlaying = listSong.listSongs[listSong.currentIndex];
               

                playMediaFile();
                
            }
        }

        private void MyPlaylist_PlaylistsChanged(ObservableCollection<IPlaylist> newValue)
        {
            _myPlaylists = newValue;
        }

        public void playMediaFile()
        {

            isPlay = true;
            player.Source = new Uri(CurrentPlaying.path, UriKind.Absolute);
            player.Play(); 
            player.Stop();
            
            title.Content = CurrentPlaying.title;
            perform.Content = CurrentPlaying.singer;

            titleHome.Content = CurrentPlaying.title;
            performHome.Content = CurrentPlaying.singer;

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            _timer.Tick += _timer_Tick;

            PlayButton_Path.Data = Geometry.Parse("M6 5h4v14H6zm8 0h4v14h-4z");
            
            player.Play();
            _timer.Start();
            TimeSpan newPosition = TimeSpan.FromSeconds(CurrentPlaying.currentTime);
            player.Position = newPosition;
        }

        
        private void PlayButton_Click(object sender, RoutedEventArgs e)
       {
            if (CurrentPlaying.path == null)
                return;

            if (isPlay == false)
            {
                playMediaFile();
            }
            else
            {
                PlayButton_Path.Data = Geometry.Parse("M4610 6399 l0 -2881 43 25 c195 114 4144 2392 4494 2593 339 194 448 262 440 270 -7 7 -743 434 -1637 949 -894 516 -2001 1155 -2460 1420 -459 265 -845 487 -857 494 l-23 12 0 -2882z");
                isPlay = false;
                CurrentPlaying.currentTime = player.Position.TotalSeconds;
                player.Pause();
                _timer.Stop();
            }
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

            listSong.currentIndex += 1;
            if (listSong.currentIndex >= listSong.listSongs.Count)
            {
                listSong.currentIndex -= 1;
                return; // che do lap lai chua code
            }
            listSong.listSongs[listSong.currentIndex - 1].currentTime = 0;
            CurrentPlaying = listSong.listSongs[listSong.currentIndex];
            playMediaFile();
        }
        private void nextButton(object sender, RoutedEventArgs e)
        {
            if (listSong.listSongs == null)
                return;
            // listSong.listSongs[listSong.currentIndex].currentTime = player.Position.TotalSeconds;
            listSong.currentIndex += 1;
            if (listSong.currentIndex >= listSong.listSongs.Count)
            {
                listSong.currentIndex -= 1;
                return; // che do lap lai chua code
            }

            else
            {
                listSong.listSongs[listSong.currentIndex - 1].currentTime = 0;
                CurrentPlaying = listSong.listSongs[listSong.currentIndex];
            }
              
           
            playMediaFile();
        }
        private void prevButton(object sender, RoutedEventArgs e)
        {
            if(listSong.listSongs == null)
                return;
            listSong.currentIndex -= 1;
            if (listSong.currentIndex < 0)
            {
                listSong.currentIndex += 1;
                return;
            }
            listSong.listSongs[listSong.currentIndex + 1].currentTime = 0;
            CurrentPlaying = listSong.listSongs[listSong.currentIndex];

           
            playMediaFile();
        }

        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = (double)volumeSlider.Value;
        }

        bool volume = false;
        private void volumn_click(object sender, RoutedEventArgs e)
        {
            if(volume == false) {
                volumeSlider.Visibility = Visibility.Visible;
                volume = true;
            }
            else{
                volumeSlider.Visibility = Visibility.Hidden;
                volume = false;
            }
               
        }


        bool isMuted = false;
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space)
            {
                PlayButton_Click(sender, e);
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.M)
            {
                isMuted = !isMuted;
                player.IsMuted = isMuted;
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Right)
            {
                nextButton(sender, e);
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Left)
            {
                prevButton(sender, e);
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Up)
            {
                volumeSlider.Value += 0.05;
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Down)
            {
                volumeSlider.Value -= 0.05;
            }

        }
    }
}
