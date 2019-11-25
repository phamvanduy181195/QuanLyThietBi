using Abp.Application.Services;
using NewCM.DanhMuc.DichVus.Dto;

namespace NewCM.DanhMuc.DichVus
{
    public interface IDanhMucDichVuAppService : IAsyncCrudAppService<DanhMucDichVuDto, int, GetAllDanhMucDichVuInput, CreateDanhMucDichVuDto, DanhMucDichVuDto>
    {
    }
}
