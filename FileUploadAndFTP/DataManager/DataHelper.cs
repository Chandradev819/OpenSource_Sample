using MediaJson.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Newtonsoft.Json.Linq;
using MediaJson;

namespace FileUploadAndFTP
{
    public class DataHelper
    {
        public string filePath = HostingEnvironment.MapPath("~/UI/Data.json");
        string imagesFolder = HostingEnvironment.MapPath("~/Upload");
        string jsonMediaFilePath = ConfigurationManager.AppSettings["JsonMediaPath"].ToString();
        string jsonMediaTemplateFilePath = ConfigurationManager.AppSettings["JsonMediaTemplatePath"].ToString();
        string jsonPpShowlistFilePath = ConfigurationManager.AppSettings["JsonPPShowlistPath"].ToString();
        string jsonFileNameMeidaShow = "mediashow.json";
        string jsonFileNamePpShowlist = "pp_showlist.json";

        LogHelper logger = new LogHelper();
        string connectionString = ConfigurationManager.ConnectionStrings[Constants.ConnectionString].ConnectionString;
        #region Application User / Customer

        public ApplicationUser Login(string userName, string Password, ref string Role)
        {
            ApplicationUser appUser = new ApplicationUser();
            try
            {
                logger.LogEvent("Login:", "Begin");
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    logger.LogEvent("Login:", "Connection opened");
                    string sql = "SELECT a.applicationuserid, a.roleId, c.customerId, c.customerName, a.FirstName FROM applicationuser a join customer c where loginname = @Username AND password = @Password AND statusid = @Status";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.Add(new MySqlParameter("@Username", userName));
                    cmd.Parameters.Add(new MySqlParameter("@Password", Password));
                    cmd.Parameters.Add(new MySqlParameter("@Status", (int)Constants.Status.Active));
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            logger.LogEvent("Login:", "Data Read");
                            appUser.UserId = Convert.ToInt32(rdr[Constants.AppUserId].ToString());
                            ////appUser.Role = Convert.ToInt16(rdr[Constants.Role].ToString()) == (int)Constants.RoleType.Admin ? Constants.RoleType.Admin.ToString() : Constants.RoleType.User.ToString();
                            appUser.Role = rdr[Constants.Role].ToString();
                            appUser.CustomerId = Convert.ToInt32(rdr[Constants.CustomerId].ToString());
                            appUser.UserName = rdr["FirstName"].ToString();
                            appUser.CustomerName = rdr["customerName"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogEvent("Login:", ex.ToString());
            }
            logger.LogEvent("Login:", "End");
            return appUser;
        }

        public List<Customers> GetAllCustomers()
        {
            string sql = "Select customerid,customername,Description,addressline1,addressline2 from customer order by customerid desc;";
            List<Customers> lstCustomer = new List<Customers>();
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter mDa = new MySqlDataAdapter(cmd);
                mDa.Fill(dt);
            }
            var vCustomer = (from DataRow dr in dt.Rows
                             select new Customers
                             {
                                 CustomerId = Convert.ToInt32(dr["customerid"].ToString()),
                                 CustomerName = dr["customername"].ToString(),
                                 CustomerDescription = dr["Description"].ToString(),
                             }).ToList();
            lstCustomer = vCustomer.ToList();
            return lstCustomer;
        }
        #endregion

        #region Location code added by chandradev
        public int AddUpdateLocation(JObject state)
        {
            int i = 0;
            try
            {
                string id = (string)state["id"];
                string sql = "";
                string sqlSel = "";
                logger.LogEvent("AddUpdatelocation:", state.ToString());

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    sqlSel = "Select LocationId from location where LocationName = @LocationName";
                    MySqlCommand cmdSel = new MySqlCommand(sqlSel, conn);
                    cmdSel.Parameters.Add(new MySqlParameter("@LocationName", state["Location"].ToString()));
                    using (MySqlDataReader rdr = cmdSel.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            //logger.LogEvent("AddUpdatelocation Read():", state["LocationName"].ToString());
                            id = rdr["LocationId"].ToString();
                            logger.LogEvent("AddUpdatelocation:", id.ToString());

                        }
                    }
                    if (string.IsNullOrEmpty(id))
                        id = "0";
                    if (id == "0")
                        sql = "INSERT INTO location(StreetName,City,State,Zip,Country,LocationName,Locationcustomerid,StatusId,traffictypeid,createddate,Live,cretaedid,updatedid,updatedate,locationtypeid) VALUES " +
                                                                   "(@StreetName,@City,@State,@Zip,@Country,@LocationName,@Locationcustomerid,@StatusId,@traffictypeid,@createddate,@Live,@cretaedid,@updatedid,@updatedate,@locationtypeid);";
                    else
                        sql = "UPDATE location SET " +
                              "StreetName = @StreetName, City = @City, State = @State, Zip = @Zip,Country=@Country, LocationName = @LocationName,Locationcustomerid=@Locationcustomerid, StatusId = @StatusId, traffictypeid = @traffictypeid,createddate=@createddate,Live=@Live,cretaedid=@cretaedid,updatedate=@updatedate,locationtypeid=@locationtypeid" +
                              " WHERE LocationId = @LocationId";
                    logger.LogEvent("AddUpdatelocation query():", sql);


                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.Add(new MySqlParameter("@StreetName", state["Street"].ToString()));
                    cmd.Parameters.Add(new MySqlParameter("@City", state["City"].ToString()));
                    cmd.Parameters.Add(new MySqlParameter("@State", state["State"].ToString()));
                    cmd.Parameters.Add(new MySqlParameter("@Zip", state["Zip"].ToString()));
                    cmd.Parameters.Add(new MySqlParameter("@Country", state["Country"].ToString()));

                    cmd.Parameters.Add(new MySqlParameter("@LocationName", state["LocationVendor"].ToString()));
                    cmd.Parameters.Add(new MySqlParameter("@Locationcustomerid", state["Locationcustomerid"]));
                    cmd.Parameters.Add(new MySqlParameter("@StatusId", state["StatusId"].ToString()));
                    cmd.Parameters.Add(new MySqlParameter("@traffictypeid", state["Trafic"]));
                    cmd.Parameters.Add(new MySqlParameter("@createddate", DateTime.Now));
                    cmd.Parameters.Add(new MySqlParameter("@Live", state["LiveLoc"].ToString()));
                    //cmd.Parameters.Add(new MySqlParameter("@cretaedid", Convert.ToInt32(state["cretaedid"])));
                    //cmd.Parameters.Add(new MySqlParameter("@updatedid", Convert.ToInt32(state["updatedid"])));
                    cmd.Parameters.Add(new MySqlParameter("@updatedate", DateTime.Now));
                    //cmd.Parameters.Add(new MySqlParameter("@locationtypeid", Convert.ToInt32(state["locationtypeid"])));
                                       

                    logger.LogEvent("AddUpdatelocation upd or insert:", sql);

                    i = cmd.ExecuteNonQuery();
                    logger.LogEvent("AddUpdatelocation:", i.ToString());

                }
            }
            catch (Exception ex)
            {
                logger.LogEvent("AddUpdatelocation:", ex.ToString());

            }
            return i;

        }

        public List<Locations> GetAllLocations()
        {
            try
            {

            
            string sql = "Select * from location where StatusId = @StatusId;";
            List<Locations> lstLocation = new List<Locations>();
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@StatusId", (int)Constants.Status.Active));
                MySqlDataAdapter mDa = new MySqlDataAdapter(cmd);
                mDa.Fill(dt);
            }
            var locations = (from DataRow dr in dt.Rows
                             select new Locations
                             {
                                 LocationId = Convert.ToInt32(dr["LocationId"].ToString()),
                                 StreetName = dr["StreetName"].ToString(),
                                 City = dr["City"].ToString(),
                                 State = dr["State"].ToString(),
                                 Zip = dr["Zip"].ToString(),
                                 Country = dr["Country"].ToString(),
                                 LocationName= dr["LocationName"].ToString(),

                                 //Locationcustomerid= dr["Locationcustomerid"].ToString(),
                                 //StatusId= dr["StatuId"].ToString(),
                                 //traffictypeid = dr["StatusId"].ToString(),
                                 //createddate= dr["createddate"].ToString(),
                                 Live = dr["Live"].ToString()
                                 //cretaedid= dr["cretaedid"].ToString(),
                                 //updatedid= dr["updatedid"].ToString(),

                                 //updatedate= dr["updatedate"].ToString(),
                                 //locationtypeid = dr["locationtypeid"].ToString()
                             }).ToList();
            lstLocation = locations.ToList();
            return lstLocation;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int DeleteLocation(JObject state)
        {
            logger.LogEvent("DelLoc", state.ToString());
            int i = 0;
            try
            {
                string sql = "Update CustomerResource set fileStatusid = @fileStatusId, lastUpdatedDateTime = current_timestamp" +
                         " WHERE Locationid = @LocationId;" +
                         " UPDATE location SET " +
                          "StatusId = 0" +
                          " WHERE LocationId = @LocationId";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.Add(new MySqlParameter("@LocationId", (int)state["id"]));
                    cmd.Parameters.Add(new MySqlParameter("@Status", 0));
                    cmd.Parameters.Add(new MySqlParameter("@fileStatusId", (int)Constants.FileStatus.deleted));
                    i = cmd.ExecuteNonQuery();

                }
                string dirPath = imagesFolder + "\\" + (string)state["Location"];
                logger.LogEvent("Delete Path:", dirPath);

                if (Directory.Exists(dirPath))
                    Directory.Delete(dirPath, true);
            }
            catch (Exception ex)
            {
                logger.LogEvent("Delete:", ex.ToString());

            }
            return i;
        }

        #endregion

        #region File List Or Customer Resource

        /// <summary>
        /// Upload Files - Add to Customer Resource
        /// </summary>
        /// <param name="fCont"></param>
        /// <returns></returns>
        public string AddFile(bool isChange)
        {
            List<FileContent> lstFiles = new List<FileContent>();
            try
            {
                //string loc = CheckFileExists();
                //if (loc.Trim().Length > 0)
                //    return loc;
                int status = 0;
                int fileId = 0;
                string createDate = "";
                string sqlSel = "";
                lstFiles = UploadFile(isChange);
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    foreach (FileContent fCont in lstFiles)
                    {
                        sqlSel = "Select customerresourceId, CR.locationId, L.LocationName, CR.fileStatusId, CR.uploadeddatetime from customerresource CR left join location L on CR.LocationId = L.LocationId where filename = @filename and  L.LocationName = @LocationName;";
                        MySqlCommand cmdSel = new MySqlCommand(sqlSel, conn);
                        cmdSel.Parameters.Add(new MySqlParameter("@filename", fCont.origFileName));
                        cmdSel.Parameters.Add(new MySqlParameter("@LocationName", fCont.locationName));
                        using (MySqlDataReader rdr = cmdSel.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                status = Convert.ToInt32(rdr["fileStatusId"].ToString());
                                fileId = Convert.ToInt32(rdr["customerresourceId"].ToString());
                                createDate = rdr["uploadeddatetime"].ToString();
                            }
                        }
                        if (fileId > 0)
                        {
                            if (status == (int)Constants.FileStatus.deleted)
                            {
                                string sqlUpd = "Update customerresource set fileName = @fileName where customerresourceId = @customerresourceId";
                                string[] fileNameArray = fCont.fileName.Split('.');
                                string fileName = fileNameArray[0].Split('_')[0] + "_" + Convert.ToDateTime(createDate).ToString("MMddyyhhmmsstt") + "." + fileNameArray[1];
                                MySqlCommand cmdUpd = new MySqlCommand(sqlUpd, conn);
                                cmdUpd.Parameters.Add(new MySqlParameter("@customerresourceId", fileId));
                                cmdUpd.Parameters.Add(new MySqlParameter("@fileName", fileName));//+ "_" + createDate

                                cmdUpd.ExecuteNonQuery();
                            }
                            else
                            {
                                string sqlDel = "Delete from customerresource where customerresourceId = @customerresourceId";
                                MySqlCommand cmdDel = new MySqlCommand(sqlDel, conn);
                                cmdDel.Parameters.Add(new MySqlParameter("@customerresourceId", fileId));
                                //cmdUpd.Parameters.Add(new MySqlParameter("@fileName", fCont.fileName + "_" + createDate));
                                cmdDel.ExecuteNonQuery();
                                DeleteFilePath(fCont.origFileName, fCont.locationName, Enum.GetName(typeof(Constants.FileStatus), status));
                            }

                        }
                        string sqlInsert = "INSERT INTO customerresource(createdapplicationuserid,fileName,fileStatusid,fileSize,Locationid,filePath,fullPath,mediaType,customerid)" +
                               "VALUES(@appUserId, @fileName, @fileStatusId, @fileSize, (select locationId from Location where LocationName = @locationName) ,@filePath, @fullPath, @mediaType, @customerId)";
                        MySqlCommand cmd = new MySqlCommand(sqlInsert, conn);
                        cmd.Parameters.Add(new MySqlParameter("@appUserId", fCont.appUserId));
                        cmd.Parameters.Add(new MySqlParameter("@fileName", fCont.fileName));
                        cmd.Parameters.Add(new MySqlParameter("@fileStatusId", (int)Constants.FileStatus.pending));
                        //cmd.Parameters.Add(new MySqlParameter("@uploadDateTime", fCont.uploadDateTime));
                        cmd.Parameters.Add(new MySqlParameter("@fileSize", fCont.fileSize));
                        cmd.Parameters.Add(new MySqlParameter("@locationName", fCont.locationName));
                        cmd.Parameters.Add(new MySqlParameter("@filePath", fCont.filePath));
                        cmd.Parameters.Add(new MySqlParameter("@fullPath", fCont.fullPath));
                        cmd.Parameters.Add(new MySqlParameter("@mediaType", fCont.mediaTypeId));
                        cmd.Parameters.Add(new MySqlParameter("@customerId", fCont.customerId));
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogEvent("AddFile:", ex.ToString());
            }

            return Constants.successMessage;

        }

        public List<FileContent> UploadFile(bool isChangeName)
        {
            List<FileContent> lstFiles = new List<FileContent>();
            try
            {
                string origFilename = "";
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        FileData data = new FileData();
                        //bool isChangeName = Convert.ToBoolean(HttpContext.Current.Request.Form["isChangeName"].ToString());
                        data.fileName = postedFile.FileName;
                        if (data.fileName.LastIndexOf("\\") > 0)
                        {
                            data.fileName = data.fileName.Substring(data.fileName.LastIndexOf("\\") + 1);
                        }
                        origFilename = data.fileName;
                        data.fileName = isChangeName ? origFilename.Split('.')[0] + "_" + DateTime.Now.ToString("MMddyyhhmmsstt") + "." + origFilename.Split('.')[1] : data.fileName;
                        data.fileStatus = "1";
                        data.fileSize = postedFile.ContentLength.ToString();
                        //data.locationIds = HttpContext.Current.Request.Form["locationIds"];
                        // data.uploadDateTime = HttpContext.Current.Request.Form["startDate"] != "" ? HttpContext.Current.Request.Form["startDate"] : DateTime.Now.ToString("dd-MM-yyyy");
                        data.locationName = HttpContext.Current.Request.Form["locations"];
                        //data.customerIds = HttpContext.Current.Request.Form["CustomerId"];
                        data.customerAdminId = HttpContext.Current.Request.Form["CustomerId"];
                        data.customerName = HttpContext.Current.Request.Form["customerName"];
                        List<string> locationsList = JsonConvert.DeserializeObject<List<string>>(data.locationName);
                        foreach (string location in locationsList)
                        {

                            // (@appUserId,@fileName,@fileStatusId,@uploadDateTime,@fileSize,@locationId,@filePath,@fullPath,@mediaType,@customerId)";
                            if (location != "")
                            {
                                FileContent fCont = new FileContent();
                                string strPath = "\\" + location.Replace(' ', '-').ToLower() + "\\Pending\\";
                                string dirPath = imagesFolder + strPath;
                                string mediaJson = "";
                                mediaJson = dirPath + jsonFileNameMeidaShow;
                                if (!Directory.Exists(dirPath))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(dirPath);

                                    if (Directory.Exists(dirPath))
                                    {
                                        try
                                        {

                                            File.Copy(jsonMediaTemplateFilePath, di.FullName + jsonFileNameMeidaShow);
                                            File.Copy(jsonPpShowlistFilePath, di.FullName + jsonFileNamePpShowlist);
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }

                                string fullpath = imagesFolder + strPath + data.fileName;
                                postedFile.SaveAs(fullpath);
                                data.filePath = strPath + data.fileName;
                                data.fullPath = fullpath;
                                data.locationName = location;

                                fCont.appUserId = Convert.ToInt32(HttpContext.Current.Request.Form["UserId"]);
                                fCont.fileName = data.fileName;
                                fCont.fileStatusId = (int)Constants.FileStatus.pending;
                                fCont.fileSize = data.fileSize;
                                fCont.customerId = Convert.ToInt32(HttpContext.Current.Request.Form["CustomerId"]);
                                fCont.appUserId = Convert.ToInt32(HttpContext.Current.Request.Form["appUserId"]);
                                fCont.locationName = data.locationName;
                                fCont.filePath = data.filePath;
                                fCont.fullPath = data.fullPath;
                                fCont.origFileName = origFilename;
                                fCont.mediaTypeId = (int)(Constants.MediaExtnType)Enum.Parse(typeof(Constants.MediaExtnType), data.MediaType().ToString());
                                fCont.origFilePath = strPath + origFilename;
                                fCont.origFullPath = imagesFolder + strPath + origFilename;
                                lstFiles.Add(fCont);


                                MediajsonHandler mh = MediajsonHandler.Instance;
                                mh.writeToMediaJson(mediaJson, data, false);
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            { }
            return lstFiles;
        }
        //        ex = new Exception("custom exception for testing");
        //        throw ex;
        //    }
        //}

        public string CheckFileExists()
        {
            string fileName = "";
            string locationName = "";
            string loclstName = "";
            string sqlSel = "";
            string locations = "";
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                List<string> locationsList = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    fileName = postedFile.FileName;
                    if (fileName.LastIndexOf("\\") > 0)
                    {
                        fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                    }
                    locationName = HttpContext.Current.Request.Form["locations"].Trim();
                    locationsList = JsonConvert.DeserializeObject<List<string>>(locationName);
                    loclstName = string.Format("'{0}'", string.Join("','", locationsList.Where(x => x != "")));
                }
            }
            sqlSel = "Select customerresourceId, CR.locationId, L.LocationName from customerresource CR left join location L on CR.LocationId = L.LocationId where filename = @filename and  L.LocationName in (" + loclstName + ");";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmdSel = new MySqlCommand(sqlSel, conn);
                cmdSel.Parameters.Add(new MySqlParameter("@filename", fileName));
                //cmdSel.Parameters.Add(new MySqlParameter("@LocationName", loclstName));
                MySqlDataAdapter sqlDa = new MySqlDataAdapter(cmdSel);
                DataTable dtLoc = new DataTable();
                sqlDa.Fill(dtLoc);

                var locName = from row in dtLoc.AsEnumerable() select row.Field<string>("LocationName");
                locations = string.Join(", ", locName.ToArray());
                //var fileIds = from row in dtLoc.AsEnumerable() select row.Field<int>("customerresourceId");
                //string CusResId = string.Join(", ", fileIds.ToArray());
                return locations.Trim() != "" ? locations : Constants.successMessage;
            }

        }

        public int ApproveFile(FileData file)
        {
            file = MoveApproveFile(file);
            int i = 0;
            string sql = "Update CustomerResource set fileStatusid = @fileStatusId, filePath = @filePath, fullPath = @fullPath, approvedDateTime = current_timestamp, lastUpdatedDateTime = current_timestamp" +
                         " WHERE customerresourceid = @CustomerResourceId;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@CustomerResourceId", file.id));
                cmd.Parameters.Add(new MySqlParameter("@fileStatusId", (int)Constants.FileStatus.approved));
                cmd.Parameters.Add(new MySqlParameter("@filePath", file.filePath));
                cmd.Parameters.Add(new MySqlParameter("@fullPath", file.fullPath));
                i = cmd.ExecuteNonQuery();
            }
            return i;
        }

        public FileData MoveApproveFile(FileData file)
        {
            string location = file.locationName;
            string strSourcePath = "/" + location.Replace(' ', '-').ToLower() + "/Pending/";
            string strDestPath = "/" + location.Replace(' ', '-').ToLower() + "/Approved/";
            if (!Directory.Exists(imagesFolder + strDestPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(imagesFolder + strDestPath);
                try
                {
                    File.Copy(jsonMediaTemplateFilePath, di.FullName + jsonFileNameMeidaShow);
                    File.Copy(jsonPpShowlistFilePath, di.FullName + jsonFileNamePpShowlist);
                }
                catch
                {

                }
            }
            if (File.Exists(imagesFolder + strSourcePath + file.fileName))
            {
                File.Move(imagesFolder + strSourcePath + file.fileName, imagesFolder + strDestPath + file.fileName);
                file.filePath = strDestPath + file.fileName;
                file.fullPath = imagesFolder + strDestPath + file.fileName;
                string strUploadPath = imagesFolder + "/" + file.locationName.Replace(' ', '-').ToLower() + "/Pending/" + file.fileName;
                string strPendingJsonPath = imagesFolder + "/" + location.Replace(' ', '-').ToLower() + "/Pending/" + jsonFileNameMeidaShow;
                DeleteMediaShowJsonObject(file.fileName, strUploadPath, strPendingJsonPath);
                logger.LogEvent(strUploadPath, strPendingJsonPath);
                string mediajsonpath = Path.Combine(imagesFolder + strDestPath + jsonFileNameMeidaShow);
                MediajsonHandler mh = MediajsonHandler.Instance;
                mh.writeToMediaJson(mediajsonpath, file, false);
            }
            return file;
        }

        public int DeleteFile(FileData file)
        {
            int i = 0;
            try
            {
                DeleteFilePath(file.fileName, file.locationName, file.fileStatus);
                string sql = "Update CustomerResource set fileStatusid = @fileStatusId, lastUpdatedDateTime = current_timestamp, filename = @filename" +
                             " WHERE customerresourceid = @CustomerResourceId;";
                // string sqlDel = "Delete from  CustomerResource" +
                //            " WHERE customerresourceid = @CustomerResourceId;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string[] fileNameArray = file.fileName.Split('.');
                    string fileName = fileNameArray[0] + "_" + DateTime.Now.ToString("MMddyyhhmmsstt") + "." + fileNameArray[1];
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.Add(new MySqlParameter("@CustomerResourceId", file.id));
                    cmd.Parameters.Add(new MySqlParameter("@fileStatusId", (int)Constants.FileStatus.deleted));
                    cmd.Parameters.Add(new MySqlParameter("@filename", fileName));
                    i = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

            }
            return i;
        }

        private void DeleteFilePath(string fileName, string locationName, string fileStatus)
        {
            string location = locationName;
            string strApprovedPath = imagesFolder + "/" + location.Replace(' ', '-').ToLower() + "/Approved/" + fileName;
            string strUploadPath = imagesFolder + "/" + location.Replace(' ', '-').ToLower() + "/Pending/" + fileName;
            string strApprovedJsonPath = imagesFolder + "/" + location.Replace(' ', '-').ToLower() + "/Approved/" + jsonFileNameMeidaShow;
            string strPendingJsonPath = imagesFolder + "/" + location.Replace(' ', '-').ToLower() + "/Pending/" + jsonFileNameMeidaShow;
            if (fileStatus == Constants.FileStatus.approved.ToString())
            {
                if (File.Exists(strApprovedPath))
                    File.Delete(strApprovedPath);
                DeleteMediaShowJsonObject(fileName, strApprovedPath, strApprovedJsonPath);
            }
            else if (fileStatus == Constants.FileStatus.pending.ToString())
            {
                if (File.Exists(strUploadPath))
                    File.Delete(strUploadPath);
                DeleteMediaShowJsonObject(fileName, strUploadPath, strPendingJsonPath);
            }
        }

        private static void DeleteMediaShowJsonObject(string fileName, string strPath, string strJsonPath)
        {
            LogHelper logger = new LogHelper();
            //File.Delete(strPath);
            logger.LogEvent("DeleteMediaShowJsonObject:", "start");

            var strFileContent = File.ReadAllText(strJsonPath);
            var FileList = JsonConvert.DeserializeObject<Media>(strFileContent);
            JObject templateobject = JObject.Parse(strFileContent);
            logger.LogEvent("strJsonPath:", strJsonPath.ToString());
            //  string fileLocationPath = ConfigurationManager.AppSettings["fileLocationPath"].ToString();

            IList<JObject> alltemplates = templateobject["tracks"].Select(t => (JObject)t).ToList();
            int i = 0;
            foreach (JObject obj in alltemplates)
            {
                string locname = (string)obj["location"];
                //     if (locname == fileLocationPath + fileName)
                if (locname.Contains("/" + fileName))
                {
                    logger.LogEvent("DeleteMediaShowJsonObject:", i.ToString());
                    FileList.tracks.RemoveAt(i);
                    break;
                }
                i++;
            }
            var mediaJsonString = JsonConvert.SerializeObject(FileList);
            File.WriteAllText(strJsonPath, mediaJsonString);
            logger.LogEvent("DeleteMediaShowJsonObject end:", mediaJsonString);

        }

        public List<FileData> GetAllFiles(int CustomerId, int Role)
        {
            List<FileData> lstFiles = new List<FileData>();
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string sql = "";
                conn.Open();
                if (Role == (int)Constants.RoleType.Admin)
                    sql = "SELECT customerresourceid, CR.customerId, fileName, fileStatusId, fileSize, filePath, uploadedDateTime, CR.LocationId, L.locationName, fullPath, C.customerName, CR.mediaTypeId  FROM customerresource CR" +
                                        " left join customer C on CR.customerId = C.CustomerId" +
                                        " left join location L on CR.LocationId = L.locationId" +
                                        " where fileStatusId != @StatusId";
                else
                    sql = "SELECT customerresourceid, CR.customerId, fileName, fileStatusId, fileSize, filePath, uploadedDateTime, CR.LocationId, L.locationName, fullPath, C.customerName, CR.mediaTypeId  FROM customerresource CR" +
                                        " left join customer C on CR.customerId = C.CustomerId" +
                                        " left join location L on CR.LocationId = L.locationId" +
                                        " where CR.customerId = @CustomerId and fileStatusId != @StatusId;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("@CustomerId", CustomerId));
                cmd.Parameters.Add(new MySqlParameter("@StatusId", (int)Constants.FileStatus.deleted));

                MySqlDataAdapter mDa = new MySqlDataAdapter(cmd);
                mDa.Fill(dt);
            }
            var emp = (from DataRow dr in dt.Rows
                       select new FileData
                       {
                           id = dr["customerresourceid"].ToString(),
                           customerAdminId = dr["customerId"].ToString(),
                           fileName = dr["fileName"].ToString(),
                           fileStatus = Enum.GetName(typeof(Constants.FileStatus), Convert.ToInt32(dr["fileStatusId"].ToString())),
                           fileSize = dr["fileSize"].ToString(),
                           filePath = dr["filePath"].ToString(),
                           uploadedDate = dr["uploadedDateTime"].ToString(),
                           locationName = dr["locationName"].ToString(),
                           customerName = dr["customerName"].ToString(),
                           approved = Enum.GetName(typeof(Constants.FileStatus), Convert.ToInt32(dr["fileStatusId"].ToString())) == Constants.FileStatus.approved.ToString() ? true : false,
                           fullPath = dr["fullPath"].ToString(),
                       }).ToList();
            lstFiles = emp.ToList();
            return lstFiles;
        }

        #endregion

        #region Import Data from JSON

        public void BulkInsertFileContent()
        {        //(createdapplicationuserid,fileName,fileStatusid,uploadedDatetime,fileSize,Locationid,filePath,fullPath,mediaType,customerid,effectivedate,expiredate,approvedDateTime);

            FileContent fCont = new FileContent();
            var fileContent = File.ReadAllText(filePath);
            List<FileData> fileList = JsonConvert.DeserializeObject<List<FileData>>(fileContent);
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();


                foreach (FileData item in fileList)
                {
                    fCont.appUserId = 1;
                    fCont.fileName = item.fileName;
                    fCont.fileStatusId = item.fileStatus == Constants.FileStatus.pending.ToString() ? (int)Constants.FileStatus.pending : (int)Constants.FileStatus.approved;
                    fCont.uploadDateTime = !String.IsNullOrEmpty(item.uploadedDate) ? Convert.ToDateTime(item.uploadedDate) : DateTime.MinValue;
                    fCont.fileSize = item.fileSize;
                    fCont.locationName = item.locationName;
                    fCont.filePath = item.filePath;
                    fCont.fullPath = item.fullPath;
                    fCont.mediaTypeId = (int)item.mediaType;
                    fCont.customerId = 1;
                    // fCont.effectiveDate = null;
                    //fCont.expireDate = null;
                    //fCont.approvedDateTime = null;
                    string sqlInsert = "INSERT INTO customerresource(createdapplicationuserid,fileName,fileStatusid,uploadedDatetime,fileSize,Locationid,filePath,fullPath,mediaType,customerid)" +
                                       "VALUES(@appUserId,@fileName,@fileStatusId,@uploadDateTime,@fileSize,(select locationId from location where locationname = @locationName),@filePath,@fullPath,@mediaType,@customerId)";
                    MySqlCommand cmd = new MySqlCommand(sqlInsert, conn);
                    cmd.Parameters.Add(new MySqlParameter("@appUserId", fCont.appUserId));
                    cmd.Parameters.Add(new MySqlParameter("@fileName", fCont.fileName));
                    cmd.Parameters.Add(new MySqlParameter("@fileStatusId", fCont.fileStatusId));
                    cmd.Parameters.Add(new MySqlParameter("@uploadDateTime", fCont.uploadDateTime));
                    cmd.Parameters.Add(new MySqlParameter("@fileSize", fCont.fileSize));
                    cmd.Parameters.Add(new MySqlParameter("@locationName", fCont.locationName));
                    cmd.Parameters.Add(new MySqlParameter("@filePath", fCont.filePath));
                    cmd.Parameters.Add(new MySqlParameter("@fullPath", fCont.fullPath));
                    cmd.Parameters.Add(new MySqlParameter("@mediaType", fCont.mediaType));
                    cmd.Parameters.Add(new MySqlParameter("@customerId", fCont.customerId));
                    int i = cmd.ExecuteNonQuery();
                }
            }
        }


        #endregion
    }

    public class ApplicationUser
    {
        public int UserId { get; set; }
        public string Role { get; set; }
        public int CustomerId { get; set; }
        public string UserName { get; set; }
        public string CustomerName { get; set; }
    }

    public class FileContent
    {
        public int appUserId { get; set; }
        public string origFileName { get; set; }
        public string origFilePath { get; set; }
        public string origFullPath { get; set; }
        public string fileName { get; set; }
        public int fileStatusId { get; set; }
        public DateTime uploadDateTime { get; set; }
        public string fileSize { get; set; }
        public int locationId { get; set; }
        public string locationName { get; set; }
        public string filePath { get; set; }
        public string fullPath { get; set; }
        public int mediaTypeId { get; set; }
        public int customerId { get; set; }
        public DateTime effectiveDate { get; set; }
        public DateTime expireDate { get; set; }
        public DateTime approvedDateTime { get; set; }
        public string locationIds { get; set; }
        public string customerIds { get; set; }
        public string customerName { get; set; }
        public bool isChangeName { get; set; }
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

    public class Locations
    {
        public int LocationId { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string LocationName { get; set; }
        public string Locationcustomerid { get; set; }
        public string StatusId { get; set; }
        public string traffictypeid { get; set; }
        public string createddate { get; set; }
        public string Live { get; set; }
        public string cretaedid { get; set; }

        public string updatedid { get; set; }
        public string updatedate { get; set; }
        public string locationtypeid { get; set; }

        public string freeeligible { get; set; }












    }

    public class Customers
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerDescription { get; set; }
        public string addressline1 { get; set; }
        public string addressline2 { get; set; }


    }
}