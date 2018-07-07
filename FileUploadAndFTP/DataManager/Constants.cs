using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace FileUploadAndFTP
{
    public static class Constants
    {
        public const string ConnectionString = "AdGeekzConnectionString";
        public enum RoleType { Admin = 1, User = 2 };
        public enum Status { Active = 1, Inactive = 0 };
        public const string AppUserId = "applicationuserid";
        public const string Role = "roleId";
        public const string CustomerId = "customerid";
        public enum FileStatus { pending = 1, approved = 2, deleted = 3 };
        public enum MediaExtnType { Video = 1, Image = 2, Web = 3 };
        public const string successMessage = "Success";
        public const string duplicateFileLocation = "File with the same name exists for location(s) @locList. Please click ok to update or upload with a new name";
    }
}