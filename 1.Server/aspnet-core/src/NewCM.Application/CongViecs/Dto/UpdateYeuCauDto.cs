using Abp.Application.Services.Dto;

namespace NewCM.CongViecs.Dto
{
    public class UpdateYeuCauDto: EntityDto<long>
    {
        public double PhuPhi { get; set; }
        public string GhiChuNhanVien { get; set; }
        public int TrangThaiId { get; set; }
        public int? LoaiCongViecId { get; set; }

        public DanhSachHangMucDto[] DanhSachHangMuc { get; set; }
    }
}
