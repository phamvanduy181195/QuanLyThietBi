using Abp.Application.Services;
using NewCM.DanhMuc.NhomDichVus.Dto;

namespace NewCM.DanhMuc.NhomDichVus
{
    public interface IDanhMucNhomDichVuAppService : IAsyncCrudAppService<DanhMucNhomDichVuDto, int, GetAllDanhMucNhomDichVuInput, CreateDanhMucNhomDichVuDto, DanhMucNhomDichVuDto>
    {
    }
}
