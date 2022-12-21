using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Interface
{
    public class ISong
    {
        public string title { get; set; }

        public string singer { get; set; }

        public string image { get; set; }

        public double time { get; set; }
    }

    public class IPlaylist   {  

        public string name { get; set; }

        public ObservableCollection<ISong> listSongs { get; set; }

        public string date { get; set; }

        public History history { get; set; }

    }

    public class History
    {
        public ISong song { get; set; }

        public double time { get; set; }
    }
}
