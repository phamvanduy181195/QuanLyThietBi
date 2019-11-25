using NewCM.Data.Dto;
using NewCM.DbEntities;
using System.Threading.Tasks;

namespace NewCM.CongViecs.Importing
{
    public interface ICongViecExcelImporter
    {
        Task<ReadFromExcelDto<CongViec>> ReadFromExcel(string FilePath);
    }
}
