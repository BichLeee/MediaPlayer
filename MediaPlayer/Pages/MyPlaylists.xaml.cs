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
        public MyPlaylists()
        {
            InitializeComponent();
        }

        ObservableCollection<ISong> listSongs = new ObservableCollection<ISong>();

        ObservableCollection<IPlaylist> myplaylist = new();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            listSongs = new ObservableCollection<ISong>()
            {
                new ISong(){title = "Chúng ta không thuộc về nhau", singer = "Sơn Tùng M-TP", time=4.03, image="Images/chungtakhongthuocvenhau.jpg"},
                new ISong(){title = "Gặp nhưng không ở lại", singer = "Hiền Hồ", time=4.05,image="Images/gapnhungkhongolai.jpg"},
                new ISong(){title = "One last time", singer = "Ariana Grande", time=4.05,image="Images/onelasttime.jpg"},
                new ISong(){title = "a", singer = "Sơn Tùng M-TP", time=4.03, image="Images/chungtakhongthuocvenhau.jpg"},
                new ISong(){title = "b", singer = "Hiền Hồ", time=4.05,image="Images/gapnhungkhongolai.jpg"},
                new ISong(){title = "c", singer = "Ariana Grande", time=4.05,image="Images/onelasttime.jpg"},               
                new ISong(){title = "e", singer = "Ariana Grande", time=4.05,image="Images/onelasttime.jpg"},           
                new ISong(){title = "f", singer = "Hiền Hồ", time=4.05,image="Images/gapnhungkhongolai.jpg"},
                new ISong(){title = "g", singer = "Ariana Grande", time=4.05,image="Images/onelasttime.jpg"},                
                new ISong(){title = "h", singer = "Hiền Hồ", time=4.05,image="Images/gapnhungkhongolai.jpg"},
                new ISong(){title = "k", singer = "Ariana Grande", time=4.05,image="Images/onelasttime.jpg"}
            };

            myplaylist = new ObservableCollection<IPlaylist>()
            {
                new IPlaylist(){name = "Love Song", date = "20/12/2022", listSongs = listSongs , history=new History(){song=listSongs[0],time=2.00} },
                new IPlaylist(){name = "Good Song", date = "20/12/2022", listSongs = listSongs , history=new History() { song = listSongs[2], time = 2.00 }}

            };




            DataContext = myplaylist;

            PLaylistsListView.ItemsSource = myplaylist;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    object value = new Playlist((IPlaylist)myplaylist[(int)(window as MainWindow).sidebar.SelectedIndex]);
                    (window as MainWindow).navframe.Navigate(value);
                    return;
                }
            }

        }
    }
}
