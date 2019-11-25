using Abp.Application.Services.Dto;

namespace NewCM.CongViecs.Dto
{
    public class PhanLoaiCongViecInput : EntityDto<long>
    {
        public int LoaiCongViecId { get; set; }

        public int DichVuId { get; set; }

        public string GhiChuNhanVien { get; set; }
    }
}
