using Abp.Application.Services.Dto;

namespace NewCM.DanhMuc.NhomDichVus.Dto
{
    public class GetAllDanhMucNhomDichVuInput : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
