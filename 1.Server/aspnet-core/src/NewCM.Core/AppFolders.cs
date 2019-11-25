using Abp.Dependency;

namespace NewCM
{
    public class AppFolders : IAppFolders, ISingletonDependency
    {
        public string ImportSampleFolder { get; set; }

        public string TempFileDownloadFolder { get; set; }

        public string CongViecUploadFolder { get; set; }

        public string CongViecFileFolder { get; set; }

        public string CongViecImportFolder { get; set; }

        public string KhachHangImportFolder { get; set; }
    }
}
