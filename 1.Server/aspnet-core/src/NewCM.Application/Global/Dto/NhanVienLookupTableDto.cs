using Abp.Application.Services.Dto;

namespace NewCM.Global.Dto
{
    public class NhanVienLookupTableDto : EntityDto<long>
    {
        public string DisplayName { get; set; }

        public string UserName { get; set; }
    }
}
