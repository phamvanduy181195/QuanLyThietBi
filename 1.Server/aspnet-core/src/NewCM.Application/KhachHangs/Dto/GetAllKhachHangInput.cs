using Abp.Application.Services.Dto;

namespace NewCM.KhachHangs.Dto
{
    public class GetAllKhachHangInput: PagedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
