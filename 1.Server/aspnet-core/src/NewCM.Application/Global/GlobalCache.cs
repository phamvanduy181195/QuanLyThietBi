using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using NewCM.Authorization.Users;
using NewCM.Configuration;
using NewCM.DbEntities;
using NewCM.Global.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace NewCM.Global
{
    public class GlobalCache : IGlobalCache
    {
        private readonly string GoogleApiKey;

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<KhachHang, long> _khachHangRepository;
        private readonly IRepository<DanhMucTinhThanh> _danhMucTinhThanhRepository;
        private readonly IRepository<DanhMucQuanHuyen> _danhMucQuanHuyenRepository;

        public GlobalCache(ICacheManager cacheManager,
            
            IRepository<User, long> userRepository,
            IRepository<KhachHang, long> khachHangRepository,
            IRepository<DanhMucTinhThanh> danhMucTinhThanhRepository,
            IRepository<DanhMucQuanHuyen> danhMucQuanHuyenRepository,
            IHostingEnvironment env)
        {
            _cacheManager = cacheManager;
            
            _userRepository = userRepository;
            _khachHangRepository = khachHangRepository;
            _danhMucTinhThanhRepository = danhMucTinhThanhRepository;
            _danhMucQuanHuyenRepository = danhMucQuanHuyenRepository;

            var _appConfiguration = env.GetAppConfiguration();
            GoogleApiKey = _appConfiguration["App:GoogleApiKey"] ?? "AIzaSyDqxCkR5AVT2d9ZvXJnh4mQX5okA9kFqA4";
        }

        public async Task<UserCacheDto> GetUserCache(long Id)
        {
            return await _cacheManager.GetCache("UserCache").GetAsync(Id.ToString(), () => GetUserCacheFromDB(Id)) as UserCacheDto;
        }

        public async Task<List<LookupTableDto>> GetTinhThanhCache()
        {
            return await _cacheManager.GetCache("DanhMuc").GetAsync("TinhThanh", () => GetTinhThanhCacheFromDB()) as List<LookupTableDto>;
        }

        public async Task<List<LookupTableDto>> GetQuanHuyenCache(int[] TinhThanhIds)
        {
            List<LookupTableDto> result = new List<LookupTableDto>();
            foreach (var Id in TinhThanhIds)
            {
                var DanhSachQuanHuyen = await _cacheManager.GetCache("DanhMucQuanHuyen").GetAsync(Id.ToString(), () => GetQuanHuyenCacheFromDB(Id)) as List<LookupTableDto>;
                result.AddRange(DanhSachQuanHuyen);
            }

            return result;
        }

        public async Task<string> GetTinhThanhName(int? TinhThanhId)
        {
            if (!TinhThanhId.HasValue)
                return "";

            var TinhThanhCache = await GetTinhThanhCache();

            string TinhThanhName = TinhThanhCache.Where(w => w.Id == TinhThanhId).Select(s => s.DisplayName).FirstOrDefault();

            return TinhThanhName;
        }

        public async Task<string> GetQuanHuyenName(int? TinhThanhId, int? QuanHuyenId)
        {
            if (!TinhThanhId.HasValue || !QuanHuyenId.HasValue)
                return "";

            var QuanHuyenCache = await GetQuanHuyenCache(new int[] { (int)TinhThanhId });

            string QuanHuyenName = QuanHuyenCache.Where(w => w.Id == QuanHuyenId).Select(s => s.DisplayName).FirstOrDefault();

            return QuanHuyenName;
        }

        public string GetGoogleApiKey()
        {
            return GoogleApiKey;
        }

        private async Task<UserCacheDto> GetUserCacheFromDB(long Id)
        {
            var query = await (from User in _userRepository.GetAll().Where(w => w.Id == Id)
                               join KhachHang in _khachHangRepository.GetAll() on User.Id equals KhachHang.UserId into J1
                               from KhachHang in J1.DefaultIfEmpty()
                               select new
                               {
                                   User.Id,
                                   User.UserName,
                                   User.Name,
                                   User.IsCustomer,
                                   CustomerId = KhachHang != null ? KhachHang.Id : 0
                               }).FirstOrDefaultAsync();

            var result = new UserCacheDto();
            if (query != null)
            {
                result.Id = query.Id;
                result.UserName = query.UserName;
                result.Name = query.Name;
                result.IsCustomer = query.IsCustomer;

                if (query.CustomerId > 0)
                    result.CustomerId = query.CustomerId;
            }

            return result;
        }

        private async Task<List<LookupTableDto>> GetTinhThanhCacheFromDB()
        {
            var query = _danhMucTinhThanhRepository.GetAll().OrderBy(o => o.Name)
                        .Select(s => new LookupTableDto
                        {
                            Id = s.Id,
                            DisplayName = s.Name
                        });

            return await query.ToListAsync();
        }

        private async Task<List<LookupTableDto>> GetQuanHuyenCacheFromDB(int TinhThanhId)
        {
            var query = _danhMucQuanHuyenRepository.GetAll().Where(w => w.TinhThanhId == TinhThanhId)
                        .OrderBy(o => o.Name)
                        .Select(s => new LookupTableDto
                        {
                            Id = s.Id,
                            DisplayName = s.Name
                        });

            return await query.ToListAsync();
        }
    }
}
