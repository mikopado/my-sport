using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBarApp.Models.DAL
{
    public interface IRepository<T> : IDisposable
    {
        void Add(T element);
        T GetElement(Func<T, bool> func);
        void Commit();
        void Remove(T element);
        void Update(T element);
        List<T> GetElements(Func<T, bool> func);
        

    }
}
