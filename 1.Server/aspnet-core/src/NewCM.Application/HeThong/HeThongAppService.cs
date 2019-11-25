using Abp.Authorization;
using Abp.Domain.Repositories;
using NewCM.Authorization;
using NewCM.DbEntities;
using NewCM.Global;
using NewCM.HeThong.Dto;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace NewCM.HeThong
{
    public class HeThongAppService: NewCMAppServiceBase, IHeThongAppService
    {
        private readonly IRepository<TinTuc> _tinTucRepository;
        private readonly IGlobalCache _globalCache;

        public HeThongAppService(IRepository<TinTuc> tinTucRepository,
            IGlobalCache globalCache)
        {
            _tinTucRepository = tinTucRepository;
            _globalCache = globalCache;
        }

        public async Task<string> GetDieuKhoanSuDung()
        {
            var DieuKhoanSuDung = await _tinTucRepository.FirstOrDefaultAsync(w => w.Type == (int)GlobalConst.LoaiTinTuc.DieuKhoanSuDung);

            if (DieuKhoanSuDung == null)
                return "";

            return DieuKhoanSuDung.Content;
        }

        [AbpAuthorize(PermissionNames.Pages_QuanLyHeThong)]
        public async Task CreateDieuKhoanSuDung(CreateDieuKhoanSuDungDto input)
        {
            var DieuKhoanSuDung = await _tinTucRepository.FirstOrDefaultAsync(w => w.Type == (int)GlobalConst.LoaiTinTuc.DieuKhoanSuDung);

            if (DieuKhoanSuDung != null)
                DieuKhoanSuDung.Content = input.NoiDung;
            else
            {
                await _tinTucRepository.InsertAsync(new TinTuc
                {
                    Name = "Điều khoản sử dụng",
                    Description = "Điều khoản sử dụng",
                    Content = input.NoiDung,
                    Type = (int)GlobalConst.LoaiTinTuc.DieuKhoanSuDung,
                    IsActive = true,
                });
            }
        }

        public async Task<ThongTinHeThongDto> GetThongTinHeThong()
        {
            ThongTinHeThongDto result = new ThongTinHeThongDto();

            var ThongTinHeThong = await _tinTucRepository.FirstOrDefaultAsync(w => w.Type == (int)GlobalConst.LoaiTinTuc.ThongTinHeThong);
            if (ThongTinHeThong != null)
            {
                result = JsonConvert.DeserializeObject<ThongTinHeThongDto>(ThongTinHeThong.Content);
            }

            return result;
        }

        [AbpAuthorize(PermissionNames.Pages_QuanLyHeThong)]
        public async Task CreateThongTinHeThong(ThongTinHeThongDto input)
        {
            input.Location = await GlobalFunction.GetLongLatFromAddress(_globalCache.GetGoogleApiKey(), input.DiaChi);

            var ThongTinHeThong = await _tinTucRepository.FirstOrDefaultAsync(w => w.Type == (int)GlobalConst.LoaiTinTuc.ThongTinHeThong);

            if (ThongTinHeThong != null)
            {
                ThongTinHeThong.Content = JsonConvert.SerializeObject(input);
            }
            else
            {
                await _tinTucRepository.InsertAsync(new TinTuc
                {
                    Name = "Thông tin hệ thống",
                    Description = "Thông tin hệ thống",
                    Content = JsonConvert.SerializeObject(input),
                    Type = (int)GlobalConst.LoaiTinTuc.ThongTinHeThong,
                    IsActive = true,
                });
            }
        }
    }
}
