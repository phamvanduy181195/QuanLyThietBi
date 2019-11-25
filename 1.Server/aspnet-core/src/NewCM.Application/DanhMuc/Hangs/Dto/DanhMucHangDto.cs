using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using NewCM.DbEntities;

namespace NewCM.DanhMuc.Hangs.Dto
{
    [AutoMap(typeof(DanhMucHang))]
    public class DanhMucHangDto: EntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
