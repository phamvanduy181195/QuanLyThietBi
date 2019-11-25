using Abp.AutoMapper;
using NewCM.DbEntities;

namespace NewCM.DanhMuc.NhomDichVus.Dto
{
    [AutoMap(typeof(DanhMucNhomDichVu))]
    public class CreateDanhMucNhomDichVuDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
