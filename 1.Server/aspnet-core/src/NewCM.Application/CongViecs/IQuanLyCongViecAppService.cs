using Abp.Application.Services;
using NewCM.CongViecs.Dto;

namespace NewCM.CongViecs
{
    public interface IQuanLyCongViecAppService : IAsyncCrudAppService<CongViecDto, long, GetAllCongViecInput, CreateCongViecDto, CongViecDto>
    {
    }
}
