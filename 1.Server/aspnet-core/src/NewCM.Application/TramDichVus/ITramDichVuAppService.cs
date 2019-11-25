using Abp.Application.Services;
using NewCM.TramDichVus.Dto;

namespace NewCM.TramDichVus
{
    public interface ITramDichVuAppService : IAsyncCrudAppService<TramDichVuDto, int, GetAllTramDichVuInput, CreateTramDichVuDto, TramDichVuDto>
    {

    }
}
