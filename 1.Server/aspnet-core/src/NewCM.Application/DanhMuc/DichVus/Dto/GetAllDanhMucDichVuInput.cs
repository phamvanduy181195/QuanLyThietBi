using Abp.Application.Services.Dto;

namespace NewCM.DanhMuc.DichVus.Dto
{
    public class GetAllDanhMucDichVuInput : PagedResultRequestDto
    {
        public string Keyword { get; set; }

        public int? NhomDichVuId { get; set; }

        public bool? IsActive { get; set; }
    }
}
