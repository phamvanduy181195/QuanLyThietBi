using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using NewCM.DbEntities;

namespace NewCM.Global.Dto
{
    [AutoMap(typeof(DanhMucHangMuc))]
    public class HangMucLookupTableDto : EntityDto
    {
        public string DisplayName { get; set; }

        public string Name { get; set; }

        public string DonViTinh { get; set; }

        public double DonGia { get; set; }
    }
}
