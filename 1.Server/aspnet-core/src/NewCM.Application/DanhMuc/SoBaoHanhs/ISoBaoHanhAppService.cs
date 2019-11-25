using Abp.Application.Services;
using NewCM.DanhMuc.SoBaoHanhs.Dto;

namespace NewCM.DanhMuc.SoBaoHanhs
{
    public interface ISoBaoHanhAppService : IAsyncCrudAppService<SoBaoHanhDto, int, GetAllSoBaoHanhInput, CreateSoBaoHanhDto, SoBaoHanhDto>
    {
    }
}
