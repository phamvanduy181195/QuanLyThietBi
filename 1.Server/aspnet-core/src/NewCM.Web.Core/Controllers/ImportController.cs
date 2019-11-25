using Abp.Authorization;
using Abp.IO.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCM.Global;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NewCM.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ImportController : NewCMControllerBase
    {
        private readonly IAppFolders _appFolders;
        private const int MaxFileSize = 5242880; //5MB
        private readonly IGlobalCache _globalCache;

        public ImportController(IAppFolders appFolders,
            IGlobalCache globalCache)
        {
            _appFolders = appFolders;
            _globalCache = globalCache;
        }

        [HttpPost]
        [AbpAuthorize]
        public async Task<string> CongViec()
        {
            return await ImportFile(Request.Form.Files, _appFolders.CongViecImportFolder);
        }

        [HttpPost]
        [AbpAuthorize]
        public async Task<string> KhachHang()
        {
            return await ImportFile(Request.Form.Files, _appFolders.KhachHangImportFolder);
        }

        private async Task<string> ImportFile(IFormFileCollection Files, string ImportFolderPath)
        {
            try
            {
                var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);
                if (UserCache.Id == 0 || UserCache.IsCustomer)
                    throw new UserFriendlyException(L("UserIsNotStaff"));

                // Process new upload files
                if (Files == null || Files.Count <= 0)
                {
                    throw new UserFriendlyException(L("FileNotFound"));
                }

                var ImportFile = Files[0];

                return SaveFile(ImportFolderPath, ImportFile, UserCache.Id);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Incorrect Content-Type"))
                    throw new UserFriendlyException(L("ModelStateInValid"));

                throw new UserFriendlyException(ex.Message);
            }
        }

        private string SaveFile(string FolderPath, IFormFile ImportFile, long UserId)
        {
            byte[] fileBytes;
            using (var stream = ImportFile.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }

            string uploadFileName = string.Format("{0:yyyyMMdd_hhmmss}_{1}", DateTime.Now, UserId) + ImportFile.FileName.Substring(ImportFile.FileName.LastIndexOf('.'));

            // Set full path to upload file
            string uploadFilePath = Path.Combine(FolderPath, uploadFileName);

            // Save new file
            System.IO.File.WriteAllBytes(uploadFilePath, fileBytes);

            return uploadFileName;
        }
    }
}
