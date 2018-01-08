using SportsBarApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsBarApp.Session
{
    public static class SessionState
    {
        public static void SaveData<T>(Controller contr, string key, List<T> friendRequests)
        {
            contr.Session[key] = friendRequests;
        }

        public static List<T> GetDataFromSession<T>(Controller contr, string data)
        {
                
            if (contr.Session[data] != null)
            {
                return (List<T>)contr.Session[data];
            }
            
            
            return null;
        }

    }
}