using Abp.Application.Services.Dto;

namespace NewCM.DanhMuc.Hangs.Dto
{
    public class GetAllDanhMucHangInput : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
