using Abp.Application.Services;
using NewCM.DanhMuc.HangMucs.Dto;

namespace NewCM.DanhMuc.HangMucs
{
    public interface IDanhMucHangMucAppService : IAsyncCrudAppService<DanhMucHangMucDto, int, GetAllDanhMucHangMucInput, CreateDanhMucHangMucDto, DanhMucHangMucDto>
    {
    }
}
