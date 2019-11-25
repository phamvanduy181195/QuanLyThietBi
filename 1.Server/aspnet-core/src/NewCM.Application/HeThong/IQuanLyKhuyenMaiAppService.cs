using Abp.Application.Services;
using NewCM.HeThong.Dto;

namespace NewCM.HeThong
{
    public interface IQuanLyKhuyenMaiAppService : IAsyncCrudAppService<TinTucDto, int, GetAllTinTucInput, CreateTinTucDto, TinTucDto>
    {

    }
}
