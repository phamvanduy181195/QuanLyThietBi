using NewCM.Data.Dto;
using NewCM.DbEntities;
using System.Collections.Generic;

namespace NewCM.KhachHangs.Importing
{
    public interface IKhachHangExcelImporter
    {
        ReadFromExcelDto<KhachHang> ReadFromExcel(string FilePath);
    }
}
