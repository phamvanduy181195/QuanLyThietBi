using System;
using System.Collections.Generic;
using Abp.Authorization.Users;
using Abp.Extensions;
using NewCM.DbEntities;

namespace NewCM.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";

        public virtual bool IsCustomer { get; set; }
        public virtual int? TramDichVuId { get; set; }
        public virtual DateTime? Birthday { get; set; }
        public virtual byte[] ProfilePicture { get; set; }
        public virtual string Description { get; set; }

        public virtual ICollection<NhanVienNhomDichVu> NhanVienNhomDichVus { get; set; }

        public virtual ICollection<NhanVienTinhThanh> NhanVienTinhThanhs { get; set; }

        public virtual ICollection<NhanVienQuanHuyen> NhanVienQuanHuyens { get; set; }

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                //Surname = AdminUserName,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            return user;
        }
    }
}
