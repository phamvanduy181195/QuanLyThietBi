using Abp.Application.Services.Dto;

namespace NewCM.DanhMuc.SoBaoHanhs.Dto
{
    public class QuanTamInput: EntityDto<int>
    {
        public bool QuanTam { get; set; }
    }
}
