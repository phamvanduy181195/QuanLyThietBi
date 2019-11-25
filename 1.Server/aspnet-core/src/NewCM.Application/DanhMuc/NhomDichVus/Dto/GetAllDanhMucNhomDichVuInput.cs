using Abp.Application.Services.Dto;

namespace NewCM.DanhMuc.SoBaoHanhs.Dto
{
    public class GetAllSoBaoHanhInput : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
