using Abp.AutoMapper;
using NewCM.DbEntities;
using System;

namespace NewCM.CongViecs.Dto
{
    [AutoMap(typeof(CongViec))]
    public class CreateYeuCauDto
    {
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public int? DichVuId { get; set; }
        public DateTime? NgayGioHen { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public int? DiaChiQuanHuyenId { get; set; }
        public int? DiaChiTinhThanhId { get; set; }
    }
}
