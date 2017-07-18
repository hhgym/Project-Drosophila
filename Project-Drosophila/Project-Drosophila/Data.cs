using Microsoft.Win32;
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
        private static Data instance;
        public static Data Instance
        {
            get
            {
                if (instance == null)
                    instance = new Data();
                return instance;
            }
        }

        public ObservableCollection<Project> Projects { get; private set; }
        public ObservableDictionary<ushort, Student> Students { get; private set; }

        private Data()
        {
            Projects = new ObservableCollection<Project>();
            Students = new ObservableDictionary<ushort, Student>();
        }

        public void ImportStudents()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Schüler importieren...";
            dialog.Filter = "Alle unterstützten Schülerdateien (*.csv)|*.csv|CSV (*.csv)|*.csv";
            if (dialog.ShowDialog() != true)
                return;

            List<Student> students = new List<Student>();
            try
            {
                string[] lines = File.ReadAllLines(dialog.FileName);
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] studentData = line.Split(new[] { ',', ';', '\t', '|', '/' });
                    if (!Utils.UserErrorWrapper(() =>
                        (studentData.Length != 5) ? $"Die Anzahl der Werte in Zeile {i} stimmt nicht" : null))
                        return;
                    for (int j = 0; j < studentData.Length; j++)
                        studentData[j] = studentData[j].Trim();

                    string firstName = "";
                    string lastName = "";
                    string email = null;
                    byte classLevel = 0;
                    byte classNumber = 0;

                    if (!Utils.UserErrorWrapper(() =>
                    {
                        firstName = studentData[0];
                        if (string.IsNullOrWhiteSpace(firstName))
                            return "Bitte einen Vornamen eingeben";

                        lastName = studentData[1];
                        if (string.IsNullOrWhiteSpace(lastName))
                            return "Bitte einen Nachnamen eingeben";

                        email = studentData[2];
                        if (!string.IsNullOrWhiteSpace(email))
                            try
                            {
                                System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(email);
                            }
                            catch
                            {
                                return "Die E-Mail-Adresse ist ungültig";
                            }
                        else
                            email = null;

                        if (!byte.TryParse(studentData[3], out classLevel))
                            return "Die Klassenstufe ist keine gültige Zahl";
                        if (classLevel != Utils.Constrain(classLevel, Defaults.CLASS_LEVEL_MIN, Defaults.CLASS_LEVEL_MAX))
                            return "Die Klassenstufe ist ungültig";

                        if (!byte.TryParse(studentData[4], out classNumber))
                            return "Die Klassennummer ist keine gültige Zahl";
                        if (classNumber != Utils.Constrain(classNumber, Defaults.CLASS_NUMBER_MIN, Defaults.CLASS_NUMBER_MAX))
                            return "Die Klassenstufe ist ungültig";

                        return null;
                    }, i))
                        return;

                    Student student = new Student()
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        ClassLevel = classLevel,
                        ClassNumber = classNumber
                    };
                    students.Add(student);
                }
                foreach (Student student in students)
                    Students.Add(student.Id, student);
            }
            catch (IOException e)
            {
                MessageBox.Show("Fehler beim Öffnen der Datei mit den Schülerdaten:\n" + e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unbekannter Fehler beim Einlesen der Schülerdaten:\n" + e.Message);
            }
        }
        public void ExportStudentsForOnlineTool()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Schüler für Onlinetool exportieren...";
            dialog.Filter = "CSV für Onlinetool (*.csv)|*.csv";
            if (dialog.ShowDialog() != true)
                return;

            try
            {
                ushort studentCount = (ushort)Students.Count;
                string[] lines = new string[studentCount];
                int i = 0;
                ICollection<Student> students = Students.Values;
                foreach (Student student in Students.Values)
                {
                    lines[i] = $"{student.Id};{student.FirstName};{student.LastName};{student.Email};{student.ClassLevel};{student.ClassNumber}";
                    ++i;
                }
                File.WriteAllLines(dialog.FileName, lines, Encoding.Unicode);
            }
            catch (IOException e)
            {
                MessageBox.Show("Fehler beim Speichern der Schülerdaten:\n" + e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unbekannter Fehler beim Exportieren der Schülerdaten:\n" + e.Message);
            }
        }
    }
}
