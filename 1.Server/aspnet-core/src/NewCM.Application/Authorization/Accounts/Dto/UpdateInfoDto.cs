using Abp.Application.Services.Dto;
using System;

namespace NewCM.Authorization.Accounts.Dto
{
    public class UpdateInfoDto
    {
        public string ProfilePicture { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? Birthday { get; set; }

        public string Address { get; set; }

        public int DistrictId { get; set; }

        public int ProvinceId { get; set; }

        public string Description { get; set; }
    }
}
