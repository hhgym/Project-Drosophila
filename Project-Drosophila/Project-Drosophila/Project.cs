using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Project_Drosophila
{
    public class Project : INotifyPropertyChanged
    {
        private string name;
        private string description;
        private ObservableCollection<Student> managers;
        private byte participantsMin;
        private byte participantsMax;
        private byte classLevelMin;
        private byte classLevelMax;
        public event PropertyChangedEventHandler PropertyChanged;

        private static byte nextId;
        public byte Id { get; private set; }
        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    value = "";
                else
                    value = value.Trim();
                if (name == value)
                    return;
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        public string Description
        {
            get { return description; }
            set
            {
                if (value == null)
                    value = "";
                else
                    value = value.Trim();
                if (description == value)
                    return;
                description = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
            }
        }

        public ObservableCollection<Student> Managers
        {
            get { return managers; }
            set
            {
                if (managers == value)
                    return;
                managers = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Managers)));
            }
        }
        public string ManagersAsString
            => string.Join(", ", (object[])Array.ConvertAll(Managers.ToArray(), (student)
                => $"{student.FirstName} {student.LastName}"));

        public byte ParticipantsMin
        {
            get { return participantsMin; }
            set
            {
                Utils.Constrain(value, Defaults.PROJECT_PARTICIPANTS_MIN, ParticipantsMax);
                if (participantsMin == value)
                    return;
                participantsMin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParticipantsMin)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParticipantCounts)));
            }
        }
        public byte ParticipantsMax
        {
            get { return participantsMax; }
            set
            {
                Utils.Constrain(value, ParticipantsMin, Defaults.PROJECT_PARTICIPANTS_MAX);
                if (participantsMax == value)
                    return;
                participantsMax = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParticipantsMax)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParticipantCounts)));
            }
        }
        public string ParticipantCounts => $"{ParticipantsMin} - {ParticipantsMax}";

        public byte ClassLevelMin
        {
            get { return classLevelMin; }
            set
            {
                Utils.Constrain(value, Defaults.CLASS_LEVEL_MIN, ClassLevelMax);
                if (value < Defaults.CLASS_LEVEL_MIN)
                    value = Defaults.CLASS_LEVEL_MIN;
                if (classLevelMin == value)
                    return;
                classLevelMin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClassLevelMin)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClassLevels)));
            }
        }
        public byte ClassLevelMax
        {
            get { return classLevelMax; }
            set
            {
                Utils.Constrain(value, ClassLevelMin, Defaults.CLASS_LEVEL_MAX);
                if (classLevelMax == value)
                    return;
                classLevelMax = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClassLevelMax)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClassLevels)));
            }
        }
        public string ClassLevels => $"{ClassLevelMin} - {ClassLevelMax}";

        public ICommand RemoveManager { get; private set; }

        static Project()
        {
            nextId = 0;
        }
        public Project()
        {
            Id = nextId++;
            Name = "";
            Description = "";
            ParticipantsMin = Defaults.PROJECT_PARTICIPANTS_MIN_DEFAULT;
            ParticipantsMax = Defaults.PROJECT_PARTICIPANTS_MAX_DEFAULT;
            ClassLevelMin = Defaults.PROJECT_CLASS_LEVEL_MIN_DEFAULT;
            ClassLevelMax = Defaults.PROJECT_CLASS_LEVEL_MAX_DEFAULT;

            Managers = new ObservableCollection<Student>();
            Managers.CollectionChanged += (sender, e) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Managers)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ManagersAsString)));
            };
            RemoveManager = new RemoveManagerCommand() { Project = this };
        }

        public override string ToString()
            => $"{Name}: P: {ParticipantsMin}-{ParticipantsMax}, C: {ClassLevelMin}-{ClassLevelMax}";

        private class RemoveManagerCommand : ICommand
        {
            public Project Project { get; set; }
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
                => Project.Managers.Contains((Student)parameter);
            public void Execute(object parameter)
                => Project.Managers.Remove((Student)parameter);
        }
    }
}
