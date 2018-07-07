using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MediaJson.Models
{
    public class Image
    {
        private string _animateclear = "no";
        private string _displayshowbackground = "yes";
        private string _displayshowtext = "yes";
        private string _pausetext = "Paused......";
        private string _pausetextcolour = "white";
        private string _pausetextfont = "Helvetica 20 bold";
        private string _pausetextx = "10";
        private string _pausetexty = "40";
        private string _title = "adzgeek";
        private string _tracktextcolour = "white";
        private string _tracktextfont = "Helvetica 20 bold";
        private string _tracktextx = "0";
        private string _tracktexty = "40";
        private string _type = "image";
        private string _emptyString = String.Empty;
        private string _location = String.Empty;

        [JsonProperty("animate-begin")]
        public string AnimateBegin { get { return _emptyString; } set { _emptyString = ""; } }

        [JsonProperty("animate-clear")]
        public string animateclear { get { return _animateclear; } set { _animateclear = value; } }

        [JsonProperty("animate-end")]
        public string animateend { get { return _emptyString; } set { _emptyString = ""; } }

        [JsonProperty("background-colour")]
        public string backgroundcolour { get { return _emptyString; } set { _emptyString = ""; } }

        [JsonProperty("background-image")]
        public string backgroundimage { get { return _emptyString; } set { _emptyString = ""; } }

        [JsonProperty("display-show-background")]
        public string displayshowbackground { get { return _displayshowbackground; } set { _displayshowbackground = value; } }

        [JsonProperty("display-show-text")]
        public string displayshowtext { get { return _displayshowtext; } set { _displayshowtext = value; } }

        public string duration { get { return _emptyString; } set { _emptyString = ""; } }

        public string imagerotate { get { return _emptyString; } set { _emptyString = ""; } }
        public string imagewindow { get { return _emptyString; } set { _emptyString = ""; } }
        public string links { get { return _emptyString; } set { _emptyString = ""; } }
        public string location { get { return _location; } set { _location = value; } }

        [JsonProperty("pause-text")]
        public string pausetext { get { return _pausetext; } set { _pausetext = value; } }
        [JsonProperty("pause-text-colour")]
        public string pausetextcolour { get { return _pausetextcolour; } set { _pausetextcolour = value; } }
        [JsonProperty("pause-text-font")]
        public string pausetextfont { get { return _pausetextfont; } set { _pausetextfont = value; } }
        [JsonProperty("pause-text-x")]
        public string pausetextx { get { return _pausetextx; } set { _pausetextx = value; } }
        [JsonProperty("pause-text-y")]
        public string pausetexty { get { return _pausetexty; } set { _pausetexty = value; } }
        public string plugin { get { return _emptyString; } set { _emptyString = ""; } }
        [JsonProperty("show-control-begin")]
        public string showcontrolbegin { get { return _emptyString; } set { _emptyString = ""; } }
        [JsonProperty("show-control-end")]
        public string showcontrolend { get { return _emptyString; } set { _emptyString = ""; } }
        public string thumbnail { get { return _emptyString; } set { _emptyString = ""; } }
        public string title { get { return _title; } set { _title = value; } }
        [JsonProperty("track-ref")]
        public string trackref { get { return _emptyString; } set { _emptyString = ""; } }
        [JsonProperty("track-text")]
        public string tracktext { get { return _emptyString; } set { _emptyString = ""; } }
        [JsonProperty("track-text-colour")]
        public string tracktextcolour { get { return _tracktextcolour; } set { _tracktextcolour = value; } }
        [JsonProperty("track-text-font")]
        public string tracktextfont { get { return _tracktextfont; } set { _tracktextfont = value; } }
        [JsonProperty("track-text-x")]
        public string tracktextx { get { return _tracktextx; } set { _tracktextx = value; } }
        [JsonProperty("track-text-y")]
        public string tracktexty { get { return _tracktexty; } set { _tracktexty = value; } }

        public string transition { get { return _emptyString; } set { _emptyString = ""; } }
        public string type { get { return _type; } set { _type = value; } }
    }
}
