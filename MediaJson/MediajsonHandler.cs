using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaJson.Models;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Web.Hosting;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace MediaJson
{
    public class MediajsonHandler
    {
        private static MediajsonHandler instance;
        private static Queue<FileProperties> writeQueue;
        private static int queueSize = int.Parse("1");
        private static int maxLogAge = 2000000;
        private static DateTime LastWriteTime = DateTime.Now;
        private static string jsonFilePathMedia = ConfigurationManager.AppSettings["JsonMediaPath"].ToString();
        private static string jsonFilePathMediaTemplate = ConfigurationManager.AppSettings["JsonMediaTemplatePath"].ToString();
        private static string jsonFilePathPpShowlist = ConfigurationManager.AppSettings["JsonPPShowlistPath"].ToString();
        private static string jsonFileNamePpShowlist = "pp_showlist.json";
        private static JObject templateImage;
        private static JObject templateVideo;
        private static string imageLocationRoot;
        private static string videoLocationRoot;

        /// <summary>
        /// Private constructor to prevent instance creation
        /// </summary>
        private MediajsonHandler() { }

        /// <summary>
        /// An LogWriter instance that exposes a single instance
        /// </summary>
        public static MediajsonHandler Instance
        {
            get
            {
                // If the instance is null then create one and init the Queue
                if (instance == null)
                {
                    instance = new MediajsonHandler();
                    var templateJson = File.ReadAllText(jsonFilePathMedia);

                    JObject templateobject = JObject.Parse(templateJson);

                    IList<JObject> alltemplates = templateobject["tracks"].Select(t => (JObject)t).ToList();

                    foreach (JObject obj in alltemplates)
                    {
                        if ((string)obj["type"] == "video")
                        {
                            templateVideo = obj;
                            videoLocationRoot = (string)templateVideo["location"];
                        }
                        else if ((string)obj["type"] == "image")
                        {
                            templateImage = obj;
                            imageLocationRoot = (string)templateImage["location"];
                        }
                    }


                    writeQueue = new Queue<FileProperties>();
                }
                return instance;
            }
        }

        public void writeToMediaJson(string jsonFilePath, FileData fileData, bool isDeleteOperation)
        {
            lock (writeQueue)
            {
                // Create the entry and push to the Queue
                FileProperties fileProperties = new FileProperties(jsonFilePath, fileData, isDeleteOperation);
                writeQueue.Enqueue(fileProperties);

                // If we have reached the Queue Size then flush the Queue
                if (writeQueue.Count >= queueSize || DoPeriodicFlush())
                {
                    readAndWriteToMediaJson();
                }
            }
        }

        private bool DoPeriodicFlush()
        {
            TimeSpan logAge = DateTime.Now - LastWriteTime;
            if (logAge.TotalSeconds >= maxLogAge)
            {
                LastWriteTime = DateTime.Now;
                return true;
            }
            else
            {
                return false;
            }
        }



        private void readAndWriteToMediaJson()
        {
            while (writeQueue.Count > 0)
            {
                FileProperties properties = writeQueue.Dequeue();
                var mediatype = properties.data.MediaType();

                JObject parsedMedia = new JObject();
                if (mediatype == MEDIATYPE.Image)
                {
                    JObject image = templateImage;
                    image["location"] = imageLocationRoot + properties.data.fileName;
                    parsedMedia = image;
                }
                else if (mediatype == MEDIATYPE.Video)
                {
                    JObject video = templateVideo;
                    video["location"] = videoLocationRoot + properties.data.fileName;
                    parsedMedia = video;
                }
                else if (mediatype == MEDIATYPE.Web)
                {
                    //TODO: Add logic for web

                    //Web video = new Web();
                    //Web = properties.data.filePath;   
                }

                try
                {
                    if (!File.Exists(properties.jsonFilePath))
                    {
                        File.Copy(jsonFilePathMediaTemplate, properties.jsonFilePath);
                        File.Copy(jsonFilePathPpShowlist, Path.Combine(new FileInfo(properties.jsonFilePath).Directory.FullName, jsonFileNamePpShowlist));
                    }
                    var fileContent = File.ReadAllText(properties.jsonFilePath);
                    var mediaInfo = JsonConvert.DeserializeObject<Media>(fileContent);

                    mediaInfo.tracks.Add(parsedMedia);
                    var mediaJsonString = JsonConvert.SerializeObject(mediaInfo);
                    File.WriteAllText(properties.jsonFilePath, mediaJsonString);

                    if (properties.data.approved)
                    {
                        string pendingjsonPath = properties.jsonFilePath.Replace("Approved", "Pending");
                        var pendingFileContent = File.ReadAllText(pendingjsonPath);
                        var pendingMediaInfo = JsonConvert.DeserializeObject<Media>(pendingFileContent);
                        pendingMediaInfo.tracks.RemoveAll(p => p.ToString().Contains(properties.data.fileName));
                        var updatedApprovalmediaJson = JsonConvert.SerializeObject(pendingMediaInfo);
                        File.WriteAllText(pendingjsonPath, updatedApprovalmediaJson);
                    }
                }
                catch
                {

                }



            }
        }


        public Media readMediaJson(string jsonFilePath, MEDIATYPE mediatype)
        {
            var fileContent = File.ReadAllText(jsonFilePath);
            var mediaInfo = JsonConvert.DeserializeObject<Media>(fileContent);
            List<Object> newTracks = new List<object>();
            foreach (object o in mediaInfo.tracks)
            {
                //object parsedMedia = new object();
                if (mediatype == MEDIATYPE.Image)
                {
                    var img = JsonConvert.DeserializeObject<Image>(o.ToString());
                    newTracks.Add(img);
                }
                else if (mediatype == MEDIATYPE.Video)
                {
                    var video = JsonConvert.DeserializeObject<Video>(o.ToString());
                    newTracks.Add(video);
                }
                else if (mediatype == MEDIATYPE.Web)
                {
                    var web = JsonConvert.DeserializeObject<Web>(o.ToString());
                    newTracks.Add(web);
                }
            }
            mediaInfo.tracks = newTracks;


            return mediaInfo;
        }



    }

    public class FileProperties
    {
        public string jsonFilePath;
        public FileData data;
        public bool isDeleteoperation;
        public FileProperties(string _jsonFilePath, FileData fileData, bool isDelete)
        {
            jsonFilePath = _jsonFilePath;
            data = fileData;
            isDeleteoperation = isDelete;
        }
    }

}
