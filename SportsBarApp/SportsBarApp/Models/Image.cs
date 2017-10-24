using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }
        public byte[] BinImage { get; set; }
        public HttpPostedFileBase UploadImage { get; set; }
    }
}