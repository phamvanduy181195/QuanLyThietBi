using System;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.SignalR;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.Configuration;
using NewCM.Authentication.JwtBearer;
using NewCM.Configuration;
using NewCM.EntityFrameworkCore;
using System.IO;
using Abp.IO;

namespace NewCM
{
    [DependsOn(
         typeof(NewCMApplicationModule),
         typeof(NewCMEntityFrameworkModule),
         typeof(AbpAspNetCoreModule)
        , typeof(AbpAspNetCoreSignalRModule)
     )]
    public class NewCMWebCoreModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public NewCMWebCoreModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                NewCMConsts.ConnectionStringName
            );

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(NewCMApplicationModule).GetAssembly()
                 //moduleName: "app",
                 //useConventionalHttpVerbs: false
                 );

            ConfigureTokenAuth();
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(NewCMWebCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            SetAppFolders();
            base.PostInitialize();
        }

        private void SetAppFolders()
        {
            var appFolders = IocManager.Resolve<AppFolders>();

            appFolders.ImportSampleFolder = Path.Combine(_env.ContentRootPath, $"App_Data{Path.DirectorySeparatorChar}Import");

            appFolders.TempFileDownloadFolder = Path.Combine(_env.WebRootPath, $"Common{Path.DirectorySeparatorChar}Downloads");
            appFolders.CongViecUploadFolder = Path.Combine(_env.WebRootPath, $"Common{Path.DirectorySeparatorChar}CongViecs");

            appFolders.CongViecImportFolder = Path.Combine(_env.WebRootPath, $"Common{Path.DirectorySeparatorChar}Import{Path.DirectorySeparatorChar}CongViec");
            appFolders.KhachHangImportFolder = Path.Combine(_env.WebRootPath, $"Common{Path.DirectorySeparatorChar}Import{Path.DirectorySeparatorChar}KhachHang");

            // TẠO ĐƯỜNG DẪN NẾU CHƯA CÓ
            try
            {
                DirectoryHelper.CreateIfNotExists(appFolders.ImportSampleFolder);

                // Tạo thư muc Common
                DirectoryHelper.CreateIfNotExists(Path.Combine(_env.WebRootPath, $"Common"));
                DirectoryHelper.CreateIfNotExists(Path.Combine(_env.WebRootPath, $"Common{Path.DirectorySeparatorChar}Import"));

                DirectoryHelper.CreateIfNotExists(appFolders.TempFileDownloadFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.CongViecUploadFolder);

                DirectoryHelper.CreateIfNotExists(appFolders.CongViecImportFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.KhachHangImportFolder);
            }
            catch { }
        }
    }
}
