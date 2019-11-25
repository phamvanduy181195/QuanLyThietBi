using Abp.Application.Services.Dto;

namespace NewCM.CongViecs.Dto
{
    public class GetAllCongViecInput : PagedResultRequestDto
    {
        public string Keyword { get; set; }

        public int? TramDichVuId { get; set; }

        public int? TrangThaiId { get; set; }
    }
}
