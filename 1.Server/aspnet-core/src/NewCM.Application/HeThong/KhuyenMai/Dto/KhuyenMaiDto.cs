using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using NewCM.DbEntities;

namespace NewCM.HeThong.KhuyenMai.Dto
{
    [AutoMap(typeof(TinTuc))]
    public class KhuyenMaiDto: EntityDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string ImageBase64 { get; set; }
    }
}
