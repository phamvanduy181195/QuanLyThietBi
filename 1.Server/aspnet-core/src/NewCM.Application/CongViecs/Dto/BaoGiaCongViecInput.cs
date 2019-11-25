using Abp.Application.Services.Dto;

namespace NewCM.CongViecs.Dto
{
    public class BaoGiaCongViecInput : EntityDto<long>
    {
        public double PhuPhi { get; set; }

        public string LinhKienThayThe { get; set; }

        public double? LinhKienThanhTien { get; set; }

        public int YeuCauLinhKien { get; set; }

        public DanhSachHangMucDto[] DanhSachHangMuc { get; set; }
    }
}
