using Abp.Application.Services.Dto;

namespace NewCM.KhachHangs.Dto
{
    public class KhachHangLookupTableDto: EntityDto<long>
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
    }
}