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

        public Playlist(IPlaylist playlist, ListSong _songList )
        {
            InitializeComponent();
            _playlist = (IPlaylist) playlist.Clone();
            DataContext = _playlist;
            songList = (ListSong) _songList.Clone();
        }
        public delegate void PlaylistValueChangeHandler(IPlaylist newValue);
        public event PlaylistValueChangeHandler PlaylistChanged;

        public delegate void SongListValueChangeHandler(ListSong newValue);
        public event SongListValueChangeHandler SongListChanged;

        ListSong songList = new ListSong()
        {

            listSongs = null,
            currentIndex = 0,
        };


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
            if(songList.listSongs == null)
            {
                songList = new ListSong()
                {

                    listSongs = new ObservableCollection<ISong>(),
                    currentIndex = 0,
                };
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

        public enum TypeFile
        {
            VIDEO,
            AUDIO,
            OTHER
        }

        private TypeFile identifyFileType(String path)
        {
            string[] videoExtensions = {
                                           ".AVI", ".MP4", ".DIVX", ".WMV", //etc  // Video
                                       };

            string[] audioExtensions = {
                                           ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA", //etc // Audio
                                       };


            if (Array.IndexOf(videoExtensions, System.IO.Path.GetExtension(path).ToUpperInvariant()) != -1)
            {
                return TypeFile.VIDEO;
            }

            if (Array.IndexOf(audioExtensions, System.IO.Path.GetExtension(path).ToUpperInvariant()) != -1)
            {
                return TypeFile.AUDIO;
            }

            return TypeFile.OTHER;
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
                    bool isExist = false;
                    foreach (var x in _playlist.listSongs)
                    {
                        if (x.path == _path)
                        {
                            isExist = true; break;
                        }

                    }
                    if (isExist)
                        continue;

                    PreviewMedia.Source = new Uri(_path, UriKind.Absolute);
                    PreviewMedia.Play();
                    PreviewMedia.Stop();
                    ISong song;
                    
                    
                    if (identifyFileType(_path) == TypeFile.VIDEO)
                    {
                        string title = _path.Split('\\')[^1];
                        song = new ISong()
                        {
                            title = title,
                            singer = null,
                            time = _time, 
                            path = _path,
                            currentTime = 0,
                            image = "Images/onelasttime.jpg" // chua lay hinh anh
                        };
                        _playlist.listSongs.Add(song);
                    }
                    else if(identifyFileType(_path) == TypeFile.AUDIO)
                    {
                       
                        string[] info = GetAudioFileInfo(_path);
                        song = new ISong()
                        {
                            title = info[0],
                            singer = info[1],
                            time = _time, 
                            path = _path,
                            currentTime = 0,
                            image = "Images/onelasttime.jpg" // chua lay hinh anh
                        };

                        _playlist.listSongs.Add(song);
                    }
                    else
                    {

                    }

                    
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
            songList.listSongs.Clear();
            foreach (ISong song in _playlist.listSongs)
            {
                songList.listSongs.Add((ISong)song.Clone());
            }

            songList.currentIndex = 0;
            SongListChanged?.Invoke(songList);

        }

        private void PlayItemInPlaylist(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
