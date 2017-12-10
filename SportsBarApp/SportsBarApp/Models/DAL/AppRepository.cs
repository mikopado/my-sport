using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models.DAL
{
    public class AppRepository<T> : IDisposable, IRepository<T> where T: class
    {
        private DbContext dbContext;
        private DbSet<T> entities;

        public AppRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
            entities = dbContext.Set<T>();
        }

        public void Add(T element)
        {
            entities.Add(element);
            Commit();
            
        }

        public void Commit()
        {
            dbContext.SaveChanges();
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        public T GetElement(Func<T, bool> func)
        {
            return entities.Where(func).FirstOrDefault();
        }

        public List<T> GetElements(Func<T, bool> func)
        {
            return entities.Where(func).ToList();
        }

        public void Remove(T element)
        {
            entities.Remove(element);
            Commit();
        }

        public void Update(T element)
        {
            dbContext.Entry(element).State = EntityState.Modified;
            Commit();
        }
    }
}