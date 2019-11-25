using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using NewCM.Authorization.Users;
using System;

namespace NewCM.Authorization.Accounts.Dto
{
    [AutoMapFrom(typeof(User))]
    public class GetInfoDto: EntityDto<long>
    {
        public string UserName { get; set; }

        public string Name { get; set; }

        public DateTime? Birthday { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public string Address { get; set; }

        public int? DistrictId { get; set; }

        public int? ProvinceId { get; set; }

        public string ChuyenMon { get; set; }

        public string PhuTrach { get; set; }

        public string TramDichVu { get; set; }

        public string Description { get; set; }

        public string ProfilePicture { get; set; }

        public bool IsCustomer { get; set; }
    }
}
