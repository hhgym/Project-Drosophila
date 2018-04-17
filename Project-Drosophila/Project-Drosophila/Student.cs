using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Drosophila
{
    public class Student : INotifyPropertyChanged
    {
        private string firstName;
        private string lastName;
        private string email;
        private byte classLevel;
        private byte classNumber;
        public event PropertyChangedEventHandler PropertyChanged;

        private static byte nextId;
        public ushort Id { get; private set; }
        public string FirstName
        {
            get { return firstName; }
            set
            {
                if (firstName == value)
                    return;
                firstName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FirstName)));
            }
        }
        public string LastName
        {
            get { return lastName; }
            set
            {
                if (lastName == value)
                    return;
                lastName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastName)));
            }
        }
        public string Email
        {
            get { return email; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    try
                    {
                        new System.Net.Mail.MailAddress(value);
                    }
                    catch
                    {
                        return;
                    }
                else
                    value = null;
                if (email == value)
                    return;
                email = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
            }
        }

        public byte ClassLevel
        {
            get { return classLevel; }
            set
            {
                value = Utils.Constrain(value, Defaults.CLASS_LEVEL_MIN, Defaults.CLASS_LEVEL_MAX);
                if (classLevel == value)
                    return;
                classLevel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClassLevel)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Class)));
            }
        }
        public byte ClassNumber
        {
            get { return classNumber; }
            set
            {
                value = Utils.Constrain(value, Defaults.CLASS_NUMBER_MIN, Defaults.CLASS_NUMBER_MAX);
                if (classNumber == value)
                    return;
                classNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClassNumber)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Class)));
            }
        }
        public string Class => $"{ClassLevel}.{ClassNumber}";

        private Wishlist wishes;
        public Wishlist Wishes
        {
            get { return wishes; }
            set
            {
                if (wishes == value)
                    return;
                wishes = value;
                wishes.CollectionChanged += (sender, e)
                    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wishes)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wishes)));
            }
        }

        static Student()
        {
            nextId = 0;
        }
        public Student()
        {
            Id = nextId++;
            FirstName = "";
            LastName = "";
            Email = null;
            ClassLevel = Defaults.STUDENT_CLASS_LEVEL;
            ClassNumber = Defaults.STUDENT_CLASS_NUMBER;
            //Wishes = new Wishlist();
        }

        public override string ToString()
            => $"{FirstName} {LastName}, {Class}";
    }
}
