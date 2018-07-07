using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaJson.Models
{
    public class Media
    {
        public string issue { get; set; }
        public List<object> tracks { get; set; }
    }
    public enum MEDIATYPE
    {
        Video = 1,
        Image = 2,
        Web = 3
    }
}
