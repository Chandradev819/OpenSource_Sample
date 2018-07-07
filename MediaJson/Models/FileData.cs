using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaJson.Models
{
    public class FileData
    {
        public string id { get; set; }
        public string customerAdminId { get; set; }

        public string fileName { get; set; }
        public string fileStatus { get; set; }
        public string uploadedDate { get; set; }
        public string fileSize { get; set; }
        public string locationName { get; set; }
        public string filePath { get; set; }
        public string customerName { get; set; }
        public string fullPath { get; set; }
        public bool approved { get; set; }
        public int locationId { get; set; }
        public int fileId { get; set; }

        public MEDIATYPE mediaType { get { return MediaType(); } }

        string[] imageExtensions = {
                ".PNG", ".JPG", ".JPEG", ".BMP", ".GIF", //etc                
        };
        string[] videoExtensions = {
                ".AVI", ".MP4", ".DIVX", ".WMV",".MOV",".MPEG",".MPG" //etc
        };

        public MEDIATYPE MediaType()
        {
            var retVal = Array.IndexOf(imageExtensions, Path.GetExtension(filePath).ToUpperInvariant());
            if (retVal == -1)
            {
                retVal = Array.IndexOf(videoExtensions, Path.GetExtension(filePath).ToUpperInvariant());
                return MEDIATYPE.Video;
            }
            else
            {
                return MEDIATYPE.Image;
            }
        }

    }
}
