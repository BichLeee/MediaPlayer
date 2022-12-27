using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

using Interface;
using Ulti;

namespace MediaPlayer.Pages
{
    /// <summary>
    /// Interaction logic for MyPlaylists.xaml
    /// </summary>
    public partial class MyPlaylists : Page
    {
        public MyPlaylists(ObservableCollection<IPlaylist> _myplaylist, ObservableCollection<ISong> _songList )
        {

            InitializeComponent();
            myplaylist =_myplaylist;
            DataContext = myplaylist;

            songList = _songList;
        }

        ObservableCollection<IPlaylist> myplaylist = new();
        ObservableCollection<ISong> songList = new ObservableCollection<ISong>();
        public delegate void PlaylistsValueChangeHandler(ObservableCollection<IPlaylist> newValue);
        public event PlaylistsValueChangeHandler PlaylistsChanged;

        public delegate void SongListValueChangeHandler(ObservableCollection<ISong> newValue);
        public event SongListValueChangeHandler SongListChanged;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            PLaylistsListView.ItemsSource = myplaylist;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void select_Playlists(object sender, SelectionChangedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    int i = PLaylistsListView.SelectedIndex;
                    if (i == -1)
                        return;

                    var playlist = myplaylist[i];
                    var value = new Playlist((IPlaylist)playlist,songList);
                    value.PlaylistChanged += Value_PlaylistChanged;
                    value.SongListChanged += Value_SongListChanged;
                    (window as MainWindow).navframe.Navigate(value);
                    return;
                }
            }
        }

        private void Value_SongListChanged(ObservableCollection<ISong> newValue)
        {
            songList = newValue;
            SongListChanged?.Invoke(songList);
        }

        private void Value_PlaylistChanged(IPlaylist newValue)
        {
            int i = PLaylistsListView.SelectedIndex;
            if (i == -1)
                return;
            myplaylist[i] = (IPlaylist) newValue.Clone();

            PlaylistsChanged?.Invoke(myplaylist);
            

        }

        private void AddPlaylist(object sender, RoutedEventArgs e)
        {

            var screen = new AddPlaylist();
            screen.ShowDialog();
            
        }
    }
}
