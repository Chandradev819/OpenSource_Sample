using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using VerifyDevice.Helper;

namespace VerifyDevice
{
    public class deviceManager
    {
        LogHelper logger = new LogHelper();
        string filePath = Directory.GetCurrentDirectory().ToString() + "\\JSONFiles\\deviceList.json";
        public List<deviceLocation> GetDeviceList()
        {
            List<deviceLocation> lstDevice = new List<deviceLocation>();
            //var fileContent = File.ReadAllText(filePath);
            //var fileList = JsonConvert.DeserializeObject<JArray>(fileContent);
            //return fileList.ToList<object>();
            logger.LogEvent(filePath, "filepath");
            logger.LogEvent(File.Exists(filePath).ToString(), "Exists");
            var fileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : "";
            lstDevice = JsonConvert.DeserializeObject<List<deviceLocation>>(fileContent);
            return lstDevice;
        }
    }
    public class deviceLocation
    {
        public string DeviceId { get; set; }
        public string Location { get; set; }
    }
}
