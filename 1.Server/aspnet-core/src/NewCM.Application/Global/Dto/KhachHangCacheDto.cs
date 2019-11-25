namespace NewCM.Global.Dto
{
    public class KhachHangCacheDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? DistrictId { get; set; }
        public int? ProvinceId { get; set; }
        public string Location { get; set; }
    }
}
