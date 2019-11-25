using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Abp.UI;
using Abp.Domain.Repositories;
using NewCM.DbEntities;
using System.IO;
using Abp.IO.Extensions;
using NewCM.Global;
using Abp.Authorization;
using Abp.IO;

namespace NewCM.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UploadController : NewCMControllerBase
    {
        private readonly IAppFolders _appFolders;
        private const int MaxFileSize = 5242880; //5MB
        private readonly IRepository<CongViec, long> _congViecRepository;
        private readonly IGlobalCache _globalCache;

        private const string CongViecHoanThanh = "imagehoanthanh";
        private const string CongViecKhachHang = "imagekhachhang";

        public UploadController(IAppFolders appFolders,
            IRepository<CongViec, long> congViecRepository,
            IGlobalCache globalCache)
        {
            _appFolders = appFolders;
            _congViecRepository = congViecRepository;
            _globalCache = globalCache;
        }

        [HttpPost]
        [AbpAuthorize]
        public async Task<string> CongViecImage(long Id)
        {
            return await UploadImage(Id, false);
        }

        [HttpPost]
        [AbpAuthorize]
        public async Task<string> CongViecHoanThanhImage(long Id)
        {
            return await UploadImage(Id, false, CongViecHoanThanh);
        }

        [HttpPost]
        [AbpAuthorize]
        public async Task<string> CongViecKhachHangImage(long Id)
        {
            return await UploadImage(Id, true, CongViecKhachHang);
        }

        private async Task<CongViec> GetCongViec(long CongViecId, bool IsCustomer = false)
        {
            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);
            if (IsCustomer)
            {
                if(UserCache.Id == 0 || !UserCache.IsCustomer)
                    throw new UserFriendlyException(L("UserIsNotCustomer"));

                return await _congViecRepository.FirstOrDefaultAsync(w => w.Id == CongViecId && w.KhachHangId == UserCache.CustomerId);
            }
            else
            {
                if (UserCache.Id == 0 || UserCache.IsCustomer)
                    throw new UserFriendlyException(L("UserIsNotStaff"));

                return await _congViecRepository.FirstOrDefaultAsync(w => w.Id == CongViecId && w.NhanVienId == UserCache.Id);
            }
        }

        private async Task<string> UploadImage(long Id, bool IsCustomer, string FileNamePrefix = "image")
        {
            try
            {
                var CongViec = await GetCongViec(Id, IsCustomer);

                if (CongViec == null)
                    throw new UserFriendlyException(L("CongViecIsNotFound"));

                // Process new upload files
                if (Request.Form.Files == null || Request.Form.Files.Count <= 0)
                {
                    throw new UserFriendlyException(L("FileNotFound"));
                }

                //
                // Setup image folders path
                //
                string ImageFolderPath = Path.Combine(_appFolders.CongViecUploadFolder, CongViec.TimeId);
                DirectoryHelper.CreateIfNotExists(ImageFolderPath);

                var Image1 = Request.Form.Files[FileNamePrefix + "1"];
                var Image2 = Request.Form.Files[FileNamePrefix + "2"];
                var Image3 = Request.Form.Files[FileNamePrefix + "3"];

                if (Image1 == null && Image2 == null && Image3 == null)
                    throw new UserFriendlyException(L("FileNotFound"));

                if (Image1 != null)
                {
                    string ImagePath = SaveFile(ImageFolderPath, Image1, CongViec.Id, 1, FileNamePrefix);

                    switch (FileNamePrefix)
                    {
                        case CongViecHoanThanh: CongViec.ImageHoanThanh1 = ImagePath; break;
                        case CongViecKhachHang: CongViec.ImageKhachHang1 = ImagePath; break;
                        default: CongViec.Image1 = ImagePath; break;
                    }

                }
                if (Image2 != null)
                {
                    string ImagePath = SaveFile(ImageFolderPath, Image2, CongViec.Id, 2, FileNamePrefix);

                    switch (FileNamePrefix)
                    {
                        case CongViecHoanThanh: CongViec.ImageHoanThanh2 = ImagePath; break;
                        case CongViecKhachHang: CongViec.ImageKhachHang2 = ImagePath; break;
                        default: CongViec.Image2 = ImagePath; break;
                    }
                }
                if (Image3 != null)
                {
                    string ImagePath = SaveFile(ImageFolderPath, Image3, CongViec.Id, 3, FileNamePrefix);

                    switch (FileNamePrefix)
                    {
                        case CongViecHoanThanh: CongViec.ImageHoanThanh3 = ImagePath; break;
                        case CongViecKhachHang: CongViec.ImageKhachHang3 = ImagePath; break;
                        default: CongViec.Image3 = ImagePath; break;
                    }
                }

                return "OK";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Incorrect Content-Type"))
                    throw new UserFriendlyException(L("ModelStateInValid"));

                throw new UserFriendlyException(ex.Message);
            }
        }

        private string SaveFile(string ImageFolderPath, IFormFile Image, long CongViecId, int ImageNum, string FileNamePrefix = "image")
        {
            byte[] fileBytes;
            using (var stream = Image.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }

            string uploadFileName = string.Format("{0}_{1}{2}", CongViecId, FileNamePrefix, ImageNum) + Image.FileName.Substring(Image.FileName.LastIndexOf('.'));

            // Set full path to upload file
            string uploadFilePath = Path.Combine(ImageFolderPath, uploadFileName);

            // Save new file
            System.IO.File.WriteAllBytes(uploadFilePath, fileBytes);

            return uploadFileName;
        }
    }
}
