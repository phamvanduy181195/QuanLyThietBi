using Abp.Application.Services;
using NewCM.DanhMuc.Hangs.Dto;

namespace NewCM.DanhMuc.Hangs
{
    public interface IDanhMucHangAppService : IAsyncCrudAppService<DanhMucHangDto, int, GetAllDanhMucHangInput, CreateDanhMucHangDto, DanhMucHangDto>
    {
    }
}
