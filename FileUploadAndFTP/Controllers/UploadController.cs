using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Configuration;
using MediaJson.Models;
using MediaJson;

namespace FileUploadAndFTP.Controllers
{


    public class UploadController : ApiController
    {
        LogHelper logger = new LogHelper();

        string filePath = HostingEnvironment.MapPath("~/UI/Data.json");
        string imagesFolder = HostingEnvironment.MapPath("~/Upload");
        string Locations = HostingEnvironment.MapPath("~/UI/Locations.json");
        string customers = HostingEnvironment.MapPath("~/UI/customers.json");
        //string jsonFilePath1 = HostingEnvironment.MapPath("~/JSONFiles/media.json");
        //string jsonFilePathTemplate = HostingEnvironment.MapPath("~/JSONFiles/mediatemplate.json");
        //string jsonFilePath2 = HostingEnvironment.MapPath("~/JSONFiles/pp_showlist.json");
        //string jsonFileName1 = "mediashow.json";
        //string jsonFileName2 = "pp_showlist.json";

        //string approvedStatus = "approved";
        //string pendingStatus = "pending";
        DataHelper dataHelper = new DataHelper();

        [HttpGet]
        [Route("api/upload/GetFileList")]
        public List<FileData> GetFileList()
        {
            var fileContent = File.ReadAllText(filePath);
            List<FileData> fileList = JsonConvert.DeserializeObject<List<FileData>>(fileContent);
            return fileList;
        }

        [HttpPost]
        [Route("api/upload/GetAllFileList")]
        //public List<FileData> GetAllFileList(int CustomerId , int Role)
        public List<FileData> GetAllFileList(JObject userObject)
        {
            int CustomerId = Convert.ToInt32(userObject["CustomerId"].ToString());
            int Role = Convert.ToInt32(userObject["Role"].ToString());
            //string CustomerId = userObject["CustomerId"].ToString();
            //string Role = userObject["Role"].ToString();
            List<FileData> fileList = dataHelper.GetAllFiles(CustomerId, Role);
            return fileList;
        }

        //[HttpPost]
        //[Route("api/upload/approvefile")]
        //public void ApproveFile(FileData data)
        //{
        //    var fileContent = File.ReadAllText(filePath);
        //    var fileList = JsonConvert.DeserializeObject<List<FileData>>(fileContent);
        //    data.approved = true;

        //    var fileToApprove = fileList.Find(m => m.id == data.id);

        //    if (fileToApprove != null)
        //    {
        //        string location = data.locationName;
        //        string strSourcePath = "/" + location.Replace(' ', '-').ToLower() + "/Pending/";
        //        string strDestPath = "/" + location.Replace(' ', '-').ToLower() + "/Approved/";
        //        //mediaJson = dirPath + jsonFileName1;
        //        if (!Directory.Exists(imagesFolder + strDestPath))
        //        {
        //            DirectoryInfo di = Directory.CreateDirectory(imagesFolder + strDestPath);
        //            try
        //            {

        //                File.Copy(jsonFilePathTemplate, di.FullName + jsonFileName1);
        //                File.Copy(jsonFilePath2, di.FullName + jsonFileName2);
        //            }
        //            catch
        //            {

        //            }
        //        }
        //        if (File.Exists(imagesFolder + strSourcePath + fileToApprove.fileName))
        //        {
        //            File.Move(imagesFolder + strSourcePath + fileToApprove.fileName, imagesFolder + strDestPath + fileToApprove.fileName);
        //            fileToApprove.filePath = strDestPath + fileToApprove.fileName;
        //            string mediajsonpath = Path.Combine(imagesFolder + strDestPath + jsonFileName1);
        //            MediajsonHandler mh = MediajsonHandler.Instance;
        //            mh.writeToMediaJson(mediajsonpath, data, false);
        //        }
        //        fileToApprove.fileStatus = approvedStatus;
        //    }

        //    var allstring = JsonConvert.SerializeObject(fileList);
        //    File.WriteAllText(filePath, allstring);

        //    return;
        //}

        //[HttpPost]
        //[Route("api/upload/deletefile")]
        //public void DeleteFile(FileData data)
        //{
        //    try
        //    {
        //        var fileContent = File.ReadAllText(filePath);
        //        var fileList = JsonConvert.DeserializeObject<List<FileData>>(fileContent);

        //        var fileToDeleteIndex = fileList.FindIndex(m => m.id == data.id);
        //        fileList.RemoveAt(fileToDeleteIndex);
        //        var allstring = JsonConvert.SerializeObject(fileList);
        //        File.WriteAllText(filePath, allstring);

        //        if (fileToDeleteIndex >= 0)
        //        {
        //            string location = data.locationName;
        //            string strApprovedPath = imagesFolder + "/" + location.Replace(' ', '-').ToLower() + "/Approved/" + data.fileName;
        //            string strUploadPath = imagesFolder + "/" + location.Replace(' ', '-').ToLower() + "/Pending/" + data.fileName;
        //            string strApprovedJsonPath = imagesFolder + "/" + location.Replace(' ', '-').ToLower() + "/Approved/" + jsonFileName1;
        //            string strPendingJsonPath = imagesFolder + "/" + location.Replace(' ', '-').ToLower() + "/Pending/" + jsonFileName1;
        //            if (data.fileStatus == approvedStatus)
        //            {
        //                if (File.Exists(strApprovedPath))
        //                    DeleteMediaShowJsonObject(data, strApprovedPath, strApprovedJsonPath);
        //            }
        //            else if (data.fileStatus == pendingStatus)
        //            {
        //                if (File.Exists(strUploadPath))
        //                    DeleteMediaShowJsonObject(data, strUploadPath, strPendingJsonPath);
        //            }
        //        }
        //        //return Ok("File deleted.");

        //    }
        //    catch (Exception ex)
        //    {
        //       // return InternalServerError(ex);

        //    }
        //}

        private static void DeleteMediaShowJsonObject(FileData data, string strPath, string strJsonPath)
        {
            File.Delete(strPath);
            var strFileContent = File.ReadAllText(strJsonPath);
            var FileList = JsonConvert.DeserializeObject<Media>(strFileContent);
            JObject templateobject = JObject.Parse(strFileContent);

            IList<JObject> alltemplates = templateobject["tracks"].Select(t => (JObject)t).ToList();
            int i = 0;
            foreach (JObject obj in alltemplates)
            {
                if ((string)obj["location"] == "/home/pi/" + data.fileName)
                    break;
                i++;
            }
            FileList.tracks.RemoveAt(i);
            var mediaJsonString = JsonConvert.SerializeObject(FileList);
            File.WriteAllText(strJsonPath, mediaJsonString);
        }

        //[HttpPost]
        //[ActionName("api/upload/uploadfile")]
        //public HttpResponseMessage UploadFile()
        //{
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;
        //        if (httpRequest.Files.Count > 0)
        //        {
        //            var docfiles = new List<string>();
        //            foreach (string file in httpRequest.Files)
        //            {
        //                var postedFile = httpRequest.Files[file];
        //                FileData data = new FileData();
        //                data.fileName = postedFile.FileName;
        //                if (data.fileName.LastIndexOf("\\") > 0)
        //                {
        //                    data.fileName = data.fileName.Substring(data.fileName.LastIndexOf("\\") + 1);
        //                }
        //                data.fileStatus = pendingStatus;
        //                data.fileSize = postedFile.ContentLength.ToString();
        //                data.uploadedDate = HttpContext.Current.Request.Form["startDate"] != "" ? HttpContext.Current.Request.Form["startDate"] : DateTime.Now.ToString("dd-MM-yyyy");
        //                data.locationName = HttpContext.Current.Request.Form["locations"];
        //                data.customerAdminId = HttpContext.Current.Request.Form["CustomerId"];
        //                data.customerName = HttpContext.Current.Request.Form["customerName"];
        //                data.approved = false;

        //                List<string> locationsList = JsonConvert.DeserializeObject<List<string>>(data.locationName);
        //                foreach (string location in locationsList)
        //                {
        //                    if (location != "")
        //                    {
        //                        string strPath = "\\" + location.Replace(' ', '-').ToLower() + "\\Pending\\";
        //                        string dirPath = imagesFolder + strPath;
        //                        string mediaJson = "";
        //                        mediaJson = dirPath + jsonFileName1;
        //                        if (!Directory.Exists(dirPath))
        //                        {
        //                            DirectoryInfo di = Directory.CreateDirectory(dirPath);

        //                            if (Directory.Exists(dirPath))
        //                            {
        //                                try
        //                                {

        //                                    File.Copy(jsonFilePathTemplate, di.FullName + jsonFileName1);
        //                                    File.Copy(jsonFilePath2, di.FullName + jsonFileName2);
        //                                }
        //                                catch
        //                                {

        //                                }
        //                            }
        //                        }


        //                        string fullpath = imagesFolder + strPath + data.fileName;

        //                        postedFile.SaveAs(fullpath);
        //                        data.id = Guid.NewGuid().ToString();
        //                        data.filePath = strPath + data.fileName;
        //                        data.fullPath = fullpath;
        //                        data.locationName = location;
        //                        var fileContent = File.ReadAllText(filePath);
        //                        var fileList = JsonConvert.DeserializeObject<List<FileData>>(fileContent);
        //                        if (fileList == null)
        //                            fileList = new List<FileData>();
        //                        fileList.Add(data);

        //                        var allstring = JsonConvert.SerializeObject(fileList);
        //                        File.WriteAllText(filePath, allstring);


        //                        MediajsonHandler mh = MediajsonHandler.Instance;
        //                        mh.writeToMediaJson(mediaJson, data, false);
        //                    }
        //                }


        //            }
        //        }
        //        var response = Request.CreateResponse(HttpStatusCode.Moved);
        //        string rootUrl = ConfigurationManager.AppSettings["Root"].ToString();
        //        response.Headers.Location = new Uri(rootUrl + "UI/index.html#/file-list");
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex = new Exception("custom exception for testing");
        //        throw ex;
        //    }
        //}


        //[HttpGet]
        //[ActionName("api/upload/getallstates")]
        //public IHttpActionResult GetAllStates()
        //{
        //    try
        //    {
        //        var fileContent = File.ReadAllText(Locations);
        //        var getData = JsonConvert.DeserializeObject<JArray>(fileContent);
        //        return Ok(getData);
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}


        [HttpPost]
        [Route("api/upload/deletestate")]
        public IHttpActionResult DeleteState(JObject state)
        {
            try
            {
                var fileContent = File.ReadAllText(Locations);
                var stateList = JsonConvert.DeserializeObject<JArray>(fileContent);
                JToken deletedState = null;
                string locationName = "";
                foreach (var newstate in stateList)
                {
                    if (newstate["id"].ToString() == state["id"].ToString())
                    {
                        deletedState = newstate;
                        locationName = newstate["Location"].ToString();
                    }
                }
                stateList.Remove(deletedState);
                var allstring = JsonConvert.SerializeObject(stateList);
                File.WriteAllText(Locations, allstring);
                if (locationName != "")
                {
                    string path = imagesFolder + "/" + locationName.ToLower();
                    Directory.Delete(path, true);
                }
                var mainFileContent = File.ReadAllText(filePath);
                var mfileList = JsonConvert.DeserializeObject<List<FileData>>(mainFileContent);
                mfileList.RemoveAll(m => m.locationName == locationName);
                var mediaFileList = JsonConvert.SerializeObject(mfileList);
                File.WriteAllText(filePath, mediaFileList);
                return Ok("State deleted.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/upload/getallcustomers")]
        public IHttpActionResult GetAllCustomers()
        {
            try
            {
                List<Customers> lstCustomer = new List<Customers>();
                lstCustomer = dataHelper.GetAllCustomers();
                return Ok(lstCustomer);
                //var fileContent = File.ReadAllText(customers);
                //var getData = JsonConvert.DeserializeObject<JArray>(fileContent);
                //foreach (var k in getData)
                //{
                //    if (!string.IsNullOrEmpty(k["Password"].ToString()))
                //    {
                //        k["Password"] = "";
                //    }
                //}
                //return Ok(getData);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        [Route("api/upload/adminpwd")]
        public IHttpActionResult AdminPwd(JObject userobject)
        {
            string reasoncode = "";
            try
            {
                string ap = ConfigurationManager.AppSettings["AdminP"].ToString();
                string u = userobject["username"].ToString();
                string p = userobject["password"].ToString();

                if (u == "admin" && p == ap)
                {
                    return Ok();
                }
                else
                {
                    reasoncode = "wrongpwd";
                    throw new Exception();
                }

            }
            catch (Exception ex)
            {
                if (reasoncode == "wrongpwd")
                    return Unauthorized();
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/upload/userpwd")]
        public IHttpActionResult UserPwd(JObject userobject)
        {
            string reasonCode = "";
            try
            {
                string u = userobject["username"]["CustomerName"].ToString();
                string p = userobject["password"].ToString();

                var fileContent = File.ReadAllText(customers);
                var getData = JsonConvert.DeserializeObject<JArray>(fileContent);

                foreach (var k in getData)
                {
                    if (k["CustomerName"].ToString() == u)
                    {
                        if (k["Password"].ToString() == p)
                            return Ok();
                        else
                        {
                            reasonCode = "wrongpwd";
                            throw new Exception();
                        }

                    }
                }
                throw new Exception();
            }
            catch (Exception ex)
            {
                if (reasonCode == "wrongpwd")
                    return Unauthorized();
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/upload/login")]
        public IHttpActionResult Login(JObject userObject)
        {
            logger.LogEvent("Login:", "Login API Call Start");
            try
            {
                ApplicationUser appUser = new ApplicationUser();
                string userName = userObject["username"].ToString();
                string userPassword = userObject["password"].ToString();
                string Role = "";
                appUser = dataHelper.Login(userName, userPassword, ref Role);
                logger.LogEvent("Login:", "Login API Call End");
                if (appUser != null && appUser.UserId > 0)
                    return Ok(appUser);
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                logger.LogEvent("Login:", "Error " + ex.ToString());
                return InternalServerError(ex);

            }
        }

        [HttpPost]
        [Route("api/upload/SaveOrUpdateLocation")]
        public IHttpActionResult SaveOrUpdateState(JObject state)
        {
            logger.LogEvent("SaveOrUpdateLocation:", "SaveOrUpdateLocation API Call Start");

            try
            {
                dataHelper.AddUpdateLocation(state);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogEvent("SaveOrUpdateLocation:", "Error " + ex.ToString());
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ActionName("api/upload/getallstates")]
        public IHttpActionResult GetAllLocations()
        {
            try
            {

                var lstLocations = dataHelper.GetAllLocations();
                return Ok(lstLocations);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/upload/DeleteLocation")]
        public IHttpActionResult DeleteLocation(JObject state)
        {
            dataHelper.DeleteLocation(state);
            return Ok();
        }

        [HttpPost]
        [Route("api/upload/ApproveFile")]
        public IHttpActionResult ApproveSelFile(FileData file)
        {
            dataHelper.ApproveFile(file);
            return Ok();
        }

        [HttpPost]
        [Route("api/upload/DeleteFile")]
        public IHttpActionResult DeleteSelFile(FileData file)
        {
            dataHelper.DeleteFile(file);
            return Ok();

        }

        [HttpPost]
        [ActionName("api/Upload/uploadfile")]
        public IHttpActionResult AddFile()
        {
            string fileAddStatus = dataHelper.CheckFileExists();
            // bool isChangeName = Convert.ToBoolean(HttpContext.Current.Request.Form["isChangeName"].ToString());
            if (fileAddStatus != Constants.successMessage)
                return Ok(Constants.duplicateFileLocation.Replace("@locList", fileAddStatus));
            else
            {
                logger.LogEvent("AddFile:", "uploadfile");
                fileAddStatus = dataHelper.AddFile(false);
                return Ok(fileAddStatus);

            }
        }

        [HttpPost]
        [Route("api/upload/insertFile")]
        public IHttpActionResult insertFile()
        {
            string fileAddStatus = dataHelper.AddFile(true);
            //if(fileAddStatus != Constants.successMessage)
            //    return Request.CreateResponse(Constants.duplicateFileLocation.Replace("@locList", fileAddStatus));
            //var response = Request.CreateResponse(HttpStatusCode.Moved);
            //string rootUrl = ConfigurationManager.AppSettings["Root"].ToString();
            //response.Headers.Location = new Uri(rootUrl + "UI/index.html#/file-list");
            return Ok(fileAddStatus);
        }

        [HttpPost]
        [Route("api/ImportFiles")]
        public IHttpActionResult ImportFiles()
        {
            dataHelper.BulkInsertFileContent();
            return Ok();

        }
    }
}