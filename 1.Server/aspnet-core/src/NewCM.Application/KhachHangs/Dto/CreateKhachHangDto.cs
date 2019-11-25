using System;

namespace NewCM.KhachHangs.Dto
{
    public class CreateKhachHangDto
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

        public string Password { get; set; }
    }
}
