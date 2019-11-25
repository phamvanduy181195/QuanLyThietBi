using Abp.Application.Services;
using NewCM.CongViecs.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewCM.CongViecs
{
    public interface ICongViecAppService: IApplicationService
    {
        Task<List<CongViecDto>> GetAll(DanhSachCongViecInput input);

        Task<string> Create(CreateYeuCauDto input);
    }
}
