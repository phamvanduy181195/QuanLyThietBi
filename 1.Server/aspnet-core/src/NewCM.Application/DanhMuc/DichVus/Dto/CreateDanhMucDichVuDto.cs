using Abp.AutoMapper;
using NewCM.DbEntities;

namespace NewCM.DanhMuc.DichVus.Dto
{
    [AutoMap(typeof(DanhMucDichVu))]
    public class CreateDanhMucDichVuDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int NhomDichVuId { get; set; }

        public bool IsActive { get; set; }
    }
}
