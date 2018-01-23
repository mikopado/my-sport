using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsBarApp.Cookies
{
    public static class AppCookie
    {
        public static void SaveCookie(Controller contr, string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                HttpCookie cook = new HttpCookie(key)
                {
                    Value = value
                };
                if (contr.Request.Cookies[key] != null)
                {

                    contr.Response.SetCookie(cook);
                }
                else
                {
                    contr.Response.Cookies.Add(cook);
                }
            }

        }
        public static void SaveCookie(Controller contr, string key, List<string> value)
        {
            if (value != null && value.Count > 0)
            {
                HttpCookie cook = new HttpCookie(key)
                {
                    Value = value.Aggregate((x, y) => x + "," + y)
                };
                if (contr.Request.Cookies[key] != null)
                {

                    contr.Response.SetCookie(cook);
                }
                else
                {
                    contr.Response.Cookies.Add(cook);
                }
            }

        }
        public static List<string> GetCookie(Controller contr, string key)
        {

            if (contr.Request.Cookies[key] != null)
            {
                List<string> data = new List<string>();
                var val = contr.Request.Cookies.Get(key).Value;
                if (val.Contains(","))
                {
                    data.AddRange(val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                }
                else
                {
                    data.Add(val);
                }
                return data;
            }
            return null;
        }

        public static void DeleteCookie(Controller contr, string key)
        {
            string[] cookies = contr.Request.Cookies.AllKeys;
            foreach (string cookie in cookies)
            {
                if (cookie.Contains(key))
                {
                    contr.Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
                }
            }

        }




    }
}