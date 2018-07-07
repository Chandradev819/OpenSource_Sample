using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MediaJson.Models
{
    /*
"animate-clear": "no",
"display-show- background": "yes",
"display-show- text": "yes",
"freeze-at- end": "no",
"location": "/home/pi/adzgeekassets/patel.mp4",
"omx-audio": "hdmi",
"seamless-loop": "no",
"title": "patel",
"track-text- colour": "white",
"track-text- font": "Helvetica 20 bold",
"track-text- x": "0",
"track-text- y": "40",
"type": "video"
     */
    public class Video
    {
        private string _animateclear = "no";
        private string _displayshowbackground = "yes";
        private string _displayshowtext = "yes";
        private string _freezeatend = "no";
        private string _omxaudio = "hdmi";
        private string _seamlessloop = "no";
        private string _title = "adzgeek";
        private string _tracktextcolour = "white";
        private string _tracktextfont = "Helvetica 20 bold";
        private string _tracktextx = "0";
        private string _tracktexty = "40";
        private string _type = "video";
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

        [JsonProperty("freeze-at-end")]
        public string freezeatend { get { return _freezeatend; } set { _freezeatend = value; } }

        public string links { get { return _emptyString; } set { _emptyString = ""; } }
        public string location { get { return _location; } set { _location = value; } }

        [JsonProperty("omx-audio")]
        public string omxaudio { get { return _omxaudio; } set { _omxaudio = value; } }

        [JsonProperty("omx-other-options")]
        public string omxotheroptions { get { return _emptyString; } set { _emptyString = ""; } }

        [JsonProperty("omx-volume")]
        public string omxvolume { get { return _emptyString; } set { _emptyString = ""; } }

        [JsonProperty("omx-window")]
        public string omxwindow { get { return _emptyString; } set { _emptyString = ""; } }

        public string plugin { get { return _emptyString; } set { _emptyString = ""; } }

        [JsonProperty("seamless-loop")]
        public string seamlessloop { get { return _seamlessloop; } set { _seamlessloop = value; } }

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

        public string type { get { return _type; } set { _type = value; } }

    }   
}
