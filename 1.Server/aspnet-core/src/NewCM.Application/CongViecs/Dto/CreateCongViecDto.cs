using Abp.AutoMapper;
using NewCM.DbEntities;
using System;

namespace NewCM.CongViecs.Dto
{
    [AutoMap(typeof(CongViec))]
    public class CreateCongViecDto
    {
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string GhiChuNhanVien { get; set; }
        public int? DichVuId { get; set; }
        public double? PhuPhi { get; set; }
        public double? ThanhTien { get; set; }
        public int? TramDichVuId { get; set; }
        public long? NhanVienId { get; set; }
        public long? KhachHangId { get; set; }
        public string KhachHangName { get; set; }
        public DateTime? NgayGioHen { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public int? DiaChiQuanHuyenId { get; set; }
        public int? DiaChiTinhThanhId { get; set; }
        public string Location { get; set; }
        public string LyDoHuy { get; set; }
        public int TrangThaiId { get; set; }
        public int? LoaiCongViecId { get; set; }
    }
}