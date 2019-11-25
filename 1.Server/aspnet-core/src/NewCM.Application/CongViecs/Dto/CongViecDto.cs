using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using NewCM.DbEntities;
using System;
using System.Collections.Generic;

namespace NewCM.CongViecs.Dto
{
    [AutoMap(typeof(CongViec))]
    public class CongViecDto: EntityDto<long>
    {
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string GhiChuKhachHang { get; set; }
        public string GhiChuNhanVien { get; set; }
        public string GhiChuQuanLy { get; set; }
        public string LinhKienThayThe { get; set; }
        public double? LinhKienThanhTien { get; set; }
        public int? DichVuId { get; set; }
        public double? PhuPhi { get; set; }
        public double ThanhTien { get; set; }
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

        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string ImageHoanThanh1 { get; set; }
        public string ImageHoanThanh2 { get; set; }
        public string ImageHoanThanh3 { get; set; }
        public string ImageKhachHang1 { get; set; }
        public string ImageKhachHang2 { get; set; }
        public string ImageKhachHang3 { get; set; }

        public string SanPhamName { get; set; }
        public string SanPhamModel { get; set; }
        public string SanPhamSerial { get; set; }

        public string SoGiaoNhan { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public DateTime? NgayKhamBenh { get; set; }
        public DateTime? NgayGiaoLinhKien { get; set; }
        public DateTime? NgayHoanThanh { get; set; }
        public DateTime? NgayDanhGia { get; set; }
        public DateTime? NgayDongCa { get; set; }

        public int? DanhGiaDiem { get; set; }
        public string DanhGiaText { get; set; }

        // Thêm các trường tên
        public int? NhomDichVuId { get; set; }
        public string NhomDichVuName { get; set; }
        public string DichVuName { get; set; }
        public string TramDichVuName { get; set; }
        public string NhanVienName { get; set; }
        public string NhanVienPhone { get; set; }
        public string TrangThaiName { get; set; }

        public List<CongViecHangMucDto> DanhSachHangMuc { get; set; }
    }
}
