using Abp.Dependency;
using NewCM.Global.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewCM.Global
{
    public interface IGlobalCache : ITransientDependency
    {
        Task<UserCacheDto> GetUserCache(long Id);

        Task<List<LookupTableDto>> GetTinhThanhCache();

        Task<List<LookupTableDto>> GetQuanHuyenCache(int[] TinhThanhIds);

        string GetGoogleApiKey();

        Task<string> GetTinhThanhName(int? TinhThanhId);

        Task<string> GetQuanHuyenName(int? TinhThanhId, int? QuanHuyenId);
    }
}
