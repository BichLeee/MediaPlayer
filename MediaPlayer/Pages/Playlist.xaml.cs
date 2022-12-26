using System;
using System.Collections.Generic;
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
    /// Interaction logic for Playlist.xaml
    /// </summary>
    public partial class Playlist : Page
    {
        public Playlist(IPlaylist playlist)
        {

            InitializeComponent();

            DataContext = playlist;
            dataGrid.ItemsSource = playlist.listSongs;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addMediaFile(object sender, RoutedEventArgs e)
        {

        }

        private void deleteMediaFile(object sender, RoutedEventArgs e)
        {

        }
    }
}
