using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Interface;
using Microsoft.Win32;
using Ulti;

namespace MediaPlayer.Pages
{
    /// <summary>
    /// Interaction logic for Playlist.xaml
    /// </summary>
    public partial class Playlist : Page
    {
        public Playlist(IPlaylist playlist, ObservableCollection<ISong> _songList )
        {
            InitializeComponent();
            _playlist = (IPlaylist) playlist.Clone();
            DataContext = _playlist;
            songList = _songList;
        }
        public delegate void PlaylistValueChangeHandler(IPlaylist newValue);
        public event PlaylistValueChangeHandler PlaylistChanged;

        public delegate void SongListValueChangeHandler(ObservableCollection<ISong> newValue);
        public event SongListValueChangeHandler SongListChanged;

        ObservableCollection<ISong> songList = new ObservableCollection<ISong>();
        IPlaylist _playlist { get; set; }
        MediaElement PreviewMedia = new MediaElement();
        string _time = "";
        DispatcherTimer _timer;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PreviewMedia.MediaOpened += PreviewMedia_MediaOpened;
            PreviewMedia.LoadedBehavior = MediaState.Manual;
            dataGrid.ItemsSource = _playlist.listSongs;

            if(_playlist.listSongs == null)
            {
                _playlist.listSongs = new ObservableCollection<ISong>();
            }

        }

        private void PreviewMedia_MediaOpened(object sender, RoutedEventArgs e)
        {
            int hours = PreviewMedia.NaturalDuration.TimeSpan.Hours;
            int minutes = PreviewMedia.NaturalDuration.TimeSpan.Minutes;
            int seconds = PreviewMedia.NaturalDuration.TimeSpan.Seconds;
             _time = $"{hours}:{minutes}:{seconds}";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

        }
        private void addMediaFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog screen = new OpenFileDialog();
            screen.Multiselect = true;
            if (screen.ShowDialog() == true)
            {
                string[] paths = screen.FileNames;

                foreach(var _path in paths)
                {
                    // chua check co phai file media hay k
                    // phan biet cai nao video va audio
                    // video thi lay thong tin kieu khac
                    PreviewMedia.Source = new Uri(_path, UriKind.Absolute);
                    PreviewMedia.Play();
                    PreviewMedia.Stop();


                    string[] info = GetAudioFileInfo(_path);
                    ISong song = new ISong()
                    {
                        title = info[0],
                        singer = info[1],
                        time = _time, // chua lay dc thoi gian (khoi lay cung dc)
                        path = _path,
                        image = "Images/onelasttime.jpg" // chua lay hinh anh
                    };

                    foreach (var x in _playlist.listSongs)
                    {
                        if (x.title == song.title && x.path == song.path && x.singer == song.singer)
                        {
                            return;
                        }

                    }
                    _playlist.listSongs.Add(song);
                }
               
                PlaylistChanged?.Invoke(_playlist);
                dataGrid.ItemsSource =null;
                dataGrid.ItemsSource = _playlist.listSongs;
                
            }
        }

        private void deleteMediaFile(object sender, RoutedEventArgs e)
        {

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

        private void Button_Play(object sender, RoutedEventArgs e)
        {
            if (_playlist.listSongs.Count == 0)
                return;
            foreach (var song in _playlist.listSongs)
            {
                songList.Add(song);
            }
            SongListChanged?.Invoke(songList);

        }

        private void PlayItemInPlaylist(object sender, MouseButtonEventArgs e)
        {
           
        }
    }
}
