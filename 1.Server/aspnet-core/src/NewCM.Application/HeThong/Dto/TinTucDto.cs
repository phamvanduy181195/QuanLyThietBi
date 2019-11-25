using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using NewCM.DbEntities;

namespace NewCM.HeThong.Dto
{
    [AutoMap(typeof(TinTuc))]
    public class TinTucDto: EntityDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public byte[] Image { get; set; }

        public string ImageBase64 { get; set; }

        public int Type { get; set; }

        public bool IsActive { get; set; }
    }
}
