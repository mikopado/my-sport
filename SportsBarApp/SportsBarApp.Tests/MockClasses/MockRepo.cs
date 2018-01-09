using SportsBarApp.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SportsBarApp.Tests.MockClasses
{
    public class MockRepo<T> : IRepository<T>
    {
        public ISet<T> Entities { get; set; }

        public MockRepo()
        {
            Entities = new HashSet<T>();
        }
        
        public void Add(T element)
        {
            Entities.Add(element);
        }

        public int Count()
        {
            return Entities.Count;
        }

        public T GetElement(Func<T, bool> func)
        {
            return Entities.Where(func).FirstOrDefault();
        }

        public IEnumerable<T> GetElements(Func<T, bool> func)
        {
            return Entities.Where(func);
        }

        public void Remove(T element)
        {
            Entities.Remove(element);
        }
    }
}
