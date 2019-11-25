namespace NewCM
{
    public interface IAppFolders
    {
        string ImportSampleFolder { get; }

        string TempFileDownloadFolder { get; }

        string CongViecUploadFolder { get; set; }

        string CongViecFileFolder { get; set; }

        string CongViecImportFolder { get; set; }

        string KhachHangImportFolder { get; set; }
    }
}
