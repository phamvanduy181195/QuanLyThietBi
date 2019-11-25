using Abp.Application.Services.Dto;

namespace NewCM.TramDichVus.Dto
{
    public class GetAllTramDichVuInput : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
