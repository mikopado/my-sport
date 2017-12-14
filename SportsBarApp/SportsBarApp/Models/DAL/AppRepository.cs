using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SportsBarApp.Models.DAL
{
    public class AppRepository<T> : IRepository<T> where T: class
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
            
        }

        public T GetElement(Expression<Func<T, bool>> func)
        {
            return entities.Where(func).FirstOrDefault();
        }

        public IEnumerable<T> GetElements(Expression<Func<T, bool>> func)
        {
            return entities.Where(func);
        }

        public void Remove(T element)
        {
            entities.Remove(element);
            
        }

        
    }
}