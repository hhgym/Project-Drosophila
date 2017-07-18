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

        public ObservableDictionary<ushort, Student> Students { get; private set; }
        public ObservableDictionary<byte, Project> Projects { get; private set; }

        private Data()
        {
            Projects = new ObservableDictionary<byte, Project>();
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
                        lastName = studentData[0];
                        if (string.IsNullOrWhiteSpace(lastName))
                            return "Bitte einen Nachnamen eingeben";

                        firstName = studentData[1];
                        if (string.IsNullOrWhiteSpace(firstName))
                            return "Bitte einen Vornamen eingeben";

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
        public void ImportProjects()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Projekte importieren...";
            dialog.Filter = "Alle unterstützten Projektdateien (*.csv)|*.csv|CSV (*.csv)|*.csv";
            if (dialog.ShowDialog() != true)
                return;

            List<Project> projects = new List<Project>();
            try
            {
                string[] lines = File.ReadAllLines(dialog.FileName);
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] projectData = line.Split(new[] { ';', '\t', '|' });
                    if (!Utils.UserErrorWrapper(() =>
                        (projectData.Length != 7) ? $"Die Anzahl der Werte in Zeile {i} stimmt nicht" : null))
                        return;
                    for (int j = 0; j < projectData.Length; j++)
                        projectData[j] = projectData[j].Trim();
                    
                    Project project = new Project();

                    if (!Utils.UserErrorWrapper(() =>
                    {
                        project.Name = projectData[0];
                        if (string.IsNullOrWhiteSpace(project.Name))
                            return "Bitte einen Projektnamen eingeben";
                        
                        if (!string.IsNullOrWhiteSpace(projectData[1]))
                            project.Description = projectData[1].Trim();

                        if (!string.IsNullOrWhiteSpace(projectData[2]))
                        {
                            string[] managersRaw = projectData[2].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < managersRaw.Length; j++)
                            {
                                string manager = managersRaw[j].Trim();
                                if (string.IsNullOrWhiteSpace(manager))
                                    continue;

                                string[] info = manager.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (info.Length != 2)
                                    return $"Die Anzahl der Eigenschaften von Projektleiter {j + 1} stimmt nicht";

                                info[0] = info[0].Trim();
                                info[1] = info[1].Trim();
                                Student student = GetStudentByName(info[0], info[1]);
                                if (student == null)
                                    return $"Der/die Schüler*in {info[1]} {info[0]} wurde nicht gefunden";

                                project.Managers.Add(student.Id);
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(projectData[3]))
                        {
                            byte participantsMin;
                            if (!byte.TryParse(projectData[3], out participantsMin))
                                return "Die minimale Teilnehmerzahl ist keine gültige Zahl";
                            project.ParticipantsMin = participantsMin;
                        }

                        if (!string.IsNullOrWhiteSpace(projectData[4]))
                        {
                            byte participantsMax;
                            if (!byte.TryParse(projectData[4], out participantsMax))
                                return "Die maximale Teilnehmerzahl ist keine gültige Zahl";
                            project.ParticipantsMax = participantsMax;
                        }

                        if (!string.IsNullOrWhiteSpace(projectData[5]))
                        {
                            byte classLevelMin;
                            if (!byte.TryParse(projectData[5], out classLevelMin))
                                return "Die minimale Klassenstufe ist keine gültige Zahl";
                            project.ClassLevelMin = classLevelMin;
                        }

                        if (!string.IsNullOrWhiteSpace(projectData[6]))
                        {
                            byte classLevelMax;
                            if (!byte.TryParse(projectData[6], out classLevelMax))
                                return "Die maximale Klassenstufe ist keine gültige Zahl";
                            project.ClassLevelMax = classLevelMax;
                        }

                        return null;
                    }, i))
                        return;

                    projects.Add(project);
                }
                foreach (Project project in projects)
                    Projects.Add(project.Id, project);
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

        public Student GetStudentByName(string lastName, string firstName)
        {
            ICollection<Student> students = Students.Values;
            foreach (Student student in students)
                if (string.Equals(student.LastName, lastName, StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(student.FirstName, firstName, StringComparison.CurrentCultureIgnoreCase))
                    return student;
            return null;
        }
    }
}
