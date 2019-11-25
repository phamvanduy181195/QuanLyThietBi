using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using NewCM.Authorization;
using NewCM.Authorization.Roles;
using NewCM.Authorization.Users;
using NewCM.Roles.Dto;
using NewCM.Users.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using NewCM.DbEntities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using Abp.Authorization.Users;

namespace NewCM.Users
{
    [AbpAuthorize(PermissionNames.Pages_Users)]
    public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>, IUserAppService
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAbpSession _abpSession;
        private readonly LogInManager _logInManager;
        private readonly IRepository<TramDichVu> _tramDichVuRepository;

        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IPasswordHasher<User> passwordHasher,
            IAbpSession abpSession,
            LogInManager logInManager,
            IRepository<TramDichVu> tramDichVuRepository) : base(repository)
        {
            _userRepository = repository;
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _abpSession = abpSession;
            _logInManager = logInManager;
            _tramDichVuRepository = tramDichVuRepository;

            LocalizationSourceName = NewCMConsts.LocalizationSourceName;
        }

        public override async Task<UserDto> Get(EntityDto<long> input)
        {
            var user = await GetEntityByIdAsync(input.Id);

            UserDto result = MapToEntityDto(user);
            result.ProfilePictureBase64 = user.ProfilePicture != null ? Convert.ToBase64String(user.ProfilePicture) : AppConsts.ProfilePicture;
            result.NhomDichVuIds = user.NhanVienNhomDichVus.Select(s => s.NhomDichVuId).ToArray();
            result.PhuTrachTinhThanhIds = user.NhanVienTinhThanhs.Select(s => s.TinhThanhId).ToArray();
            result.PhuTrachQuanHuyenIds = user.NhanVienQuanHuyens.Select(s => s.QuanHuyenId).ToArray();

            return result;
        }

        public async Task<PagedResultDto<GetUserForView>> GetAllNew(PagedUserResultRequestDto input)
        {
            var query = from User in _userRepository.GetAllIncluding(x => x.Roles)
                            .Where(w => w.IsCustomer == false)
                            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.UserName.Contains(input.Keyword) || x.Name.Contains(input.Keyword) || x.PhoneNumber.Contains(input.Keyword) || x.EmailAddress.Contains(input.Keyword))
                            .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive)

                        join TramDichVu in _tramDichVuRepository.GetAll() on User.TramDichVuId equals TramDichVu.Id into J1
                        from TramDichVu in J1.DefaultIfEmpty()

                        where input.TramDichVuId == null || TramDichVu.Id == input.TramDichVuId

                        select new
                        {
                            User,
                            TramDichVuName = TramDichVu != null ? TramDichVu.Name : "",
                            TramTruongId = TramDichVu != null ? TramDichVu.TramTruongId : 0
                        };

            var TotalCount = await query.CountAsync();
            var PagedUser = await query.OrderBy(o => o.User.UserName).PageBy(input).ToListAsync();

            var PagedUserForView = PagedUser.Select(s => new GetUserForView
            {
                User = ObjectMapper.Map<UserDto>(s.User),
                TramDichVuName = s.TramDichVuName,
                VaiTroName = s.User.Id == s.TramTruongId ? "Trạm trưởng" : s.TramTruongId > 0 ? "Nhân viên" : ""
            }).ToList();

            return new PagedResultDto<GetUserForView>(
                TotalCount,
                PagedUserForView
            );
        }

        public override async Task<UserDto> Create(CreateUserDto input)
        {
            CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);

            user.TenantId = AbpSession.TenantId;
            user.IsEmailConfirmed = true;
            user.Surname = "";
            if (!string.IsNullOrWhiteSpace(input.ProfilePictureBase64))
                user.ProfilePicture = Convert.FromBase64String(input.ProfilePictureBase64);

            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            if (input.NhomDichVuIds != null)
            {
                user.NhanVienNhomDichVus = new Collection<NhanVienNhomDichVu>();
                foreach (var Id in input.NhomDichVuIds)
                {
                    user.NhanVienNhomDichVus.Add(new NhanVienNhomDichVu
                    {
                        NhomDichVuId = Id
                    });
                }
            }
            if (input.PhuTrachTinhThanhIds != null)
            {
                user.NhanVienTinhThanhs = new Collection<NhanVienTinhThanh>();
                foreach (var Id in input.PhuTrachTinhThanhIds)
                {
                    user.NhanVienTinhThanhs.Add(new NhanVienTinhThanh
                    {
                        TinhThanhId = Id
                    });
                }
            }
            if (input.PhuTrachQuanHuyenIds != null)
            {
                user.NhanVienQuanHuyens = new Collection<NhanVienQuanHuyen>();
                foreach (var Id in input.PhuTrachQuanHuyenIds)
                {
                    user.NhanVienQuanHuyens.Add(new NhanVienQuanHuyen
                    {
                        QuanHuyenId = Id
                    });
                }
            }
            try
            {
                CheckErrors(await _userManager.CreateAsync(user, input.Password));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Email") && ex.Message.Contains("is already taken"))
                {
                    throw new UserFriendlyException(L("Identity.DuplicateEmail", user.EmailAddress));
                }
                else if (ex.Message.Contains("User") && ex.Message.Contains("is already taken"))
                {
                    throw new UserFriendlyException(L("Identity.DuplicateUserName", user.UserName));
                }
                else
                {
                    throw new UserFriendlyException("Unknow Error: " + ex.Message);
                }
            }
            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(user);
        }

        [HttpPost]
        public override async Task<UserDto> Update(UserDto input)
        {
            CheckUpdatePermission();

            var user = await GetEntityByIdAsync(input.Id);

            MapToEntity(input, user);
            if (!string.IsNullOrWhiteSpace(input.ProfilePictureBase64))
                user.ProfilePicture = Convert.FromBase64String(input.ProfilePictureBase64);

            // Xử lý danh sách chuyên môn
            var DanhSachXoa_ChuyenMon = user.NhanVienNhomDichVus.Where(w => !input.NhomDichVuIds.Contains(w.NhomDichVuId)).ToList();
            foreach (var item in DanhSachXoa_ChuyenMon)
                user.NhanVienNhomDichVus.Remove(item);
            var DanhSachMoi_ChuyenMon = input.NhomDichVuIds.Where(w => !user.NhanVienNhomDichVus.Select(s => s.NhomDichVuId).Contains(w)).ToList();
            foreach (var Id in DanhSachMoi_ChuyenMon)
                user.NhanVienNhomDichVus.Add(new NhanVienNhomDichVu { NhomDichVuId = Id });

            // Xử lý danh sách tỉnh thành
            var DanhSachXoa_TinhThanh = user.NhanVienTinhThanhs.Where(w => !input.PhuTrachTinhThanhIds.Contains(w.TinhThanhId)).ToList();
            foreach (var item in DanhSachXoa_TinhThanh)
                user.NhanVienTinhThanhs.Remove(item);
            var DanhSachMoi_TinhThanh = input.PhuTrachTinhThanhIds.Where(w => !user.NhanVienTinhThanhs.Select(s => s.TinhThanhId).Contains(w)).ToList();
            foreach (var Id in DanhSachMoi_TinhThanh)
                user.NhanVienTinhThanhs.Add(new NhanVienTinhThanh { TinhThanhId = Id });

            // Xử lý danh sách quận huyện
            var DanhSachXoa_QuanHuyen = user.NhanVienQuanHuyens.Where(w => !input.PhuTrachQuanHuyenIds.Contains(w.QuanHuyenId)).ToList();
            foreach (var item in DanhSachXoa_QuanHuyen)
                user.NhanVienQuanHuyens.Remove(item);
            var DanhSachMoi_QuanHuyen = input.PhuTrachQuanHuyenIds.Where(w => !user.NhanVienQuanHuyens.Select(s => s.QuanHuyenId).Contains(w)).ToList();
            foreach (var Id in DanhSachMoi_QuanHuyen)
                user.NhanVienQuanHuyens.Add(new NhanVienQuanHuyen { QuanHuyenId = Id });

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }
            

            return await Get(input);
        }

        public override async Task Delete(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.DeleteAsync(user);
        }

        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserDto MapToEntityDto(User user)
        {
            var roles = _roleManager.Roles.Where(r => user.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName);
            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();

            return userDto;
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding(x => x.Roles, x => x.NhanVienNhomDichVus, x => x.NhanVienQuanHuyens, x => x.NhanVienTinhThanhs).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            return user;
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedUserResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Roles)
                .Where(w => w.IsCustomer == false)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.UserName.Contains(input.Keyword) || x.Name.Contains(input.Keyword) || x.PhoneNumber.Contains(input.Keyword) || x.EmailAddress.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedUserResultRequestDto input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        //public async Task<bool> ChangePassword(ChangePasswordDto input)
        //{
        //    if (_abpSession.UserId == null)
        //    {
        //        throw new UserFriendlyException(L("LoginRequired"));
        //    }

        //    long userId = _abpSession.UserId.Value;
        //    var user = await _userManager.GetUserByIdAsync(userId);
        //    var loginAsync = await _logInManager.LoginAsync(user.UserName, input.CurrentPassword, shouldLockout: false);
        //    if (loginAsync.Result != AbpLoginResultType.Success)
        //    {
        //        throw new UserFriendlyException(L("WrongCurrentPassword"));
        //    }

        //    // Yêu cầu complex password
        //    //if (!new Regex(AccountAppService.PasswordRegex).IsMatch(input.NewPassword))
        //    //{
        //    //    throw new UserFriendlyException("Passwords must be at least 8 characters, contain a lowercase, uppercase, and number.");
        //    //}

        //    user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
        //    CurrentUnitOfWork.SaveChanges();
        //    return true;
        //}

        public async Task<bool> ResetPassword(ResetPasswordDto input)
        {
            if (_abpSession.UserId == null)
            {
                throw new UserFriendlyException(L("LoginRequired"));
            }
            long currentUserId = _abpSession.UserId.Value;
            var currentUser = await _userManager.GetUserByIdAsync(currentUserId);
            var loginAsync = await _logInManager.LoginAsync(currentUser.UserName, input.AdminPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException(L("WrongAdminPassword"));
            }
            if (currentUser.IsDeleted || !currentUser.IsActive)
            {
                return false;
            }
            var roles = await _userManager.GetRolesAsync(currentUser);
            if (!roles.Contains(StaticRoleNames.Tenants.Admin))
            {
                throw new UserFriendlyException(L("RoleAdminRequired"));
            }

            var user = await _userManager.GetUserByIdAsync(input.UserId);
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
                CurrentUnitOfWork.SaveChanges();
            }

            return true;
        }
    }
}

