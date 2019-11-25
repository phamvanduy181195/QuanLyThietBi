using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using NewCM.DbEntities;
using NewCM.Global;
using NewCM.HeThong.KhuyenMai.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewCM.HeThong.KhuyenMai
{
    [AbpAuthorize]
    public class KhuyenMaiAppService: NewCMAppServiceBase, IKhuyenMaiAppService
    {
        IRepository<TinTuc> _tinTucRepository;

        public KhuyenMaiAppService(IRepository<TinTuc> tinTucRepository)
        {
            _tinTucRepository = tinTucRepository;
        }

        public async Task<List<KhuyenMaiDto>> GetAll()
        {
            List<KhuyenMaiDto> result = new List<KhuyenMaiDto>();

            var TinTucs = await _tinTucRepository.GetAll()
                                .Where(w => w.Type == (int)GlobalConst.LoaiTinTuc.KhuyenMai && w.IsActive)
                                .OrderByDescending(o => o.Id)
                                .ToListAsync();

            foreach (var item in TinTucs)
            {
                var KhuyenMai = ObjectMapper.Map<KhuyenMaiDto>(item);
                KhuyenMai.ImageBase64 = Convert.ToBase64String(item.Image);

                result.Add(KhuyenMai);
            }

            return result;
        }
    }
}
