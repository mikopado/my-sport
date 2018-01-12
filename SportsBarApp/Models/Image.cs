using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Drawing;
using System.IO;

namespace SportsBarApp.Models
{
    public class Image
    {
        
        public int Id { get; set; }
        public byte[] Content { get; set; }
        private string filename;
        public string FileName
        {
            get
            {
                return filename;
            }
            set
            {
                if (Content != null && Content.Length > 0)
                {
                    filename = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(Content));
                }
                else
                {
                    filename = value;
                }
            }
        }  

    }
}