using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project_Drosophila
{
    public class Data
    {
        public ObservableCollection<Student> Students { get; private set; }

        public Data()
        {
            Students = new ObservableCollection<Student>();
        }
    }
}
