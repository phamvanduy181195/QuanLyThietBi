using Abp.Application.Services.Dto;

namespace NewCM.CongViecs.Dto
{
    public class HoanThanhCongViecInput : EntityDto<long>
    {
        public string GhiChuNhanVien { get; set; }
    }
}
