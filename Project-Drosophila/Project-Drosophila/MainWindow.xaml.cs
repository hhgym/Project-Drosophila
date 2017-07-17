using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Project_Drosophila
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Data Data { get; private set; }
        private Student selectedStudent;

        public event PropertyChangedEventHandler PropertyChanged;

        public Student SelectedStudent
        {
            get { return selectedStudent; }
            set
            {
                if (selectedStudent == value)
                    return;
                selectedStudent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStudent)));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            List<byte> classLevels = new List<byte>();
            for (byte i = Defaults.CLASS_LEVEL_MIN; i <= Defaults.CLASS_LEVEL_MAX; i++)
                classLevels.Add(i);
            studentClassLevel.ItemsSource = classLevels;

            List<byte> classNumbers = new List<byte>();
            for (byte i = Defaults.CLASS_NUMBER_MIN; i <= Defaults.CLASS_NUMBER_MAX; i++)
                classNumbers.Add(i);
            studentClassNumber.ItemsSource = classNumbers;

            Data = new Data();
        }

        private void studentsImportStudents_Click(object sender, RoutedEventArgs e)
        {
            Data.ImportStudents();
        }

        private void studentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedStudent = Data.Students[studentsList.SelectedIndex];
        }
    }
}
