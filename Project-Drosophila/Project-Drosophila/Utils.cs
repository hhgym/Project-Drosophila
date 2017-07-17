using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project_Drosophila
{
    public class Utils
    {
        public static T Constrain<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;
            return value;
        }

        public static bool UserErrorWrapper(Func<string> function)
        {
            string result = function.Invoke();
            if (result == null)
                return true;
            MessageBox.Show(result);
            return false;
        }
        public static bool UserErrorWrapper(Func<string> function, int line)
        {
            string result = function.Invoke();
            if (result == null)
                return true;
            MessageBox.Show($"Zeile {line + 1}: {result}");
            return false;
        }
        public static bool UserErrorWrapper(Func<object, string> function, object data)
        {
            string result = function.Invoke(data);
            if (result == null)
                return true;
            MessageBox.Show(result);
            return false;
        }
    }
}
