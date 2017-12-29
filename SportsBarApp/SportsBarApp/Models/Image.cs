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
        public string FileName { get; set; }



    }
}