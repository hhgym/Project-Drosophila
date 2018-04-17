using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Drosophila
{
    public class Wishlist : INotifyCollectionChanged
    {
        private byte[] wishes;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public Project this[byte index]
        {
            get { return Data.Instance.Projects[wishes[index]]; }
            set
            {
                if (wishes[index] == value.Id)
                    return;
                byte oldId = wishes[index];
                wishes[index] = value.Id;
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                    value.Id, (object)oldId, index));
            }
        }

        public Wishlist()
        {
            wishes = new byte[Defaults.STUDENT_WISHES_COUNT];
        }

        public override string ToString()
            => string.Join(", ", (object[])Array.ConvertAll(wishes, (id) => Data.Instance.Projects[id].Name));
    }
}
