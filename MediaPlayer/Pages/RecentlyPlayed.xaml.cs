using Interface;
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

namespace MediaPlayer.Pages
{
    /// <summary>
    /// Interaction logic for RecentlyPlayed.xaml
    /// </summary>
    public partial class RecentlyPlayed : Page
    {
        public RecentlyPlayed()
        {
            InitializeComponent();


        }
        ObservableCollection<ISong> listSongs = new ObservableCollection<ISong>();

        IPlaylist myplaylist = new();
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            

            myplaylist = new IPlaylist() { name = "Love Song", date = "20/12/2022", listSongs = null };
            DataContext = myplaylist;

            dataGrid.ItemsSource = myplaylist.listSongs;
        }
    }
}
