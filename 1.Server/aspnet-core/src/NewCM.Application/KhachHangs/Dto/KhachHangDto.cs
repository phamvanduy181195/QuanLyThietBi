using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using NewCM.DbEntities;
using System;

namespace NewCM.KhachHangs.Dto
{
    [AutoMap(typeof(KhachHang))]
    public class KhachHangDto: EntityDto<long>
    {
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public int? DistrictId { get; set; }
        public int? ProvinceId { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public long? UserId { get; set; }

        // Thêm trường
        public string UserName { get; set; }
        public string AddressFull { get; set; }
        public bool? IsActived { get; set; }
        public bool? IsLocked { get; set; }

        public string Status { get; set; }
    }
}
