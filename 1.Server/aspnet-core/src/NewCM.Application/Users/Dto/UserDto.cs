using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using NewCM.Authorization.Users;

namespace NewCM.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserDto : EntityDto<long>
    {
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        //[Required]
        //[StringLength(AbpUserBase.MaxSurnameLength)]
        //public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [Required]
        [Phone]
        [StringLength(AbpUserBase.MaxPhoneNumberLength)]

        public string PhoneNumber { get; set; }

        public bool IsCustomer { get; set; }

        public int? TramDichVuId { get; set; }

        public int[] NhomDichVuIds { get; set; }

        public int[] PhuTrachTinhThanhIds { get; set; }

        public int[] PhuTrachQuanHuyenIds { get; set; }

        public DateTime? Birthday { get; set; }

        public bool IsActive { get; set; }

        //public string FullName { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime CreationTime { get; set; }

        public string[] RoleNames { get; set; }

        public string ProfilePictureBase64 { get; set; }

        public string Description { get; set; }
    }
}
