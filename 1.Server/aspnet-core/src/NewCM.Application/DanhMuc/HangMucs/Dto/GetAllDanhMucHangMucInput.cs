using Abp.Application.Services.Dto;

namespace NewCM.DanhMuc.HangMucs.Dto
{
    public class GetAllDanhMucHangMucInput : PagedResultRequestDto
    {
        public string Keyword { get; set; }

        public int? NhomDichVuId { get; set; }

        public int? DichVuId { get; set; }

        public bool? IsActive { get; set; }
    }
}
