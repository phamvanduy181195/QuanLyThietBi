using Abp.Application.Services;
using NewCM.HeThong.KhuyenMai.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewCM.HeThong.KhuyenMai
{
    public interface IKhuyenMaiAppService : IApplicationService
    {
        Task<List<KhuyenMaiDto>> GetAll();
    }
}
