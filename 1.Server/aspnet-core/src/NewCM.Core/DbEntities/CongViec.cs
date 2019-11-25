using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("CongViec")]
    public class CongViec : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string TieuDe { get; set; }
        public virtual string NoiDung { get; set; }
        public virtual string GhiChuKhachHang { get; set; }
        public virtual string GhiChuNhanVien { get; set; }
        public virtual string GhiChuQuanLy { get; set; }
        public virtual string LinhKienThayThe { get; set; }
        public virtual double? LinhKienThanhTien { get; set; }
        public virtual int? DichVuId { get; set; }
        public virtual double? PhuPhi { get; set; }
        public virtual double ThanhTien { get; set; }
        public virtual int? TramDichVuId { get; set; }
        public virtual long? NhanVienId { get; set; }
        public virtual long? KhachHangId { get; set; }
        public virtual string KhachHangName { get; set; }
        public virtual DateTime? NgayGioHen { get; set; }
        public virtual string SoDienThoai { get; set; }
        public virtual string DiaChi { get; set; }
        public virtual int? DiaChiQuanHuyenId { get; set; }
        public virtual int? DiaChiTinhThanhId { get; set; }
        public virtual string Location { get; set; }
        public virtual string LyDoHuy { get; set; }
        public virtual string LyDoKhongNhan { get; set; }
        public virtual string LyDoTraVe { get; set; }
        public virtual int TrangThaiId { get; set; }
        public virtual int? LoaiCongViecId { get; set; }
        public virtual bool DaCapNhatLocation { get; set; }
        public virtual int? DanhGiaDiem { get; set; }
        public virtual string DanhGiaText { get; set; }

        public virtual string Image1 { get; set; }
        public virtual string Image2 { get; set; }
        public virtual string Image3 { get; set; }
        public virtual string ImageKhachHang1 { get; set; }
        public virtual string ImageKhachHang2 { get; set; }
        public virtual string ImageKhachHang3 { get; set; }
        public virtual string ImageHoanThanh1 { get; set; }
        public virtual string ImageHoanThanh2 { get; set; }
        public virtual string ImageHoanThanh3 { get; set; }
        public virtual string TimeId { get; set; }

        public virtual string SanPhamName { get; set; }
        public virtual string SanPhamModel { get; set; }
        public virtual string SanPhamSerial { get; set; }

        public virtual string SoGiaoNhan { get; set; }
        public virtual DateTime? NgayYeuCau { get; set; }
        public virtual DateTime? NgayKhamBenh { get; set; }
        public virtual DateTime? NgayGiaoLinhKien { get; set; }
        public virtual DateTime? NgayHoanThanh { get; set; }
        public virtual DateTime? NgayDanhGia { get; set; }
        public virtual DateTime? NgayDongCa { get; set; }

        public virtual ICollection<CongViecHangMuc> CongViecHangMucs { get; set; }
    }
}
