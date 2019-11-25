using Abp.Application.Services.Dto;

namespace NewCM.HeThong.Dto
{
    public class GetAllTinTucInput : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
