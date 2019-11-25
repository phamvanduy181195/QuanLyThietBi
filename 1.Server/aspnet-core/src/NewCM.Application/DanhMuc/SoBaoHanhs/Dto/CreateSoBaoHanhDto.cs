using Abp.AutoMapper;
using NewCM.DbEntities;
using System;

namespace NewCM.DanhMuc.SoBaoHanhs.Dto
{
    [AutoMap(typeof(SoBaoHanh))]
    public class CreateSoBaoHanhDto
    {
        public string Name { get; set; }
        public string SoSerial { get; set; }
        public string Hang { get; set; }
        public bool QuanTam { get; set; }
        public DateTime? NgayMua { get; set; }
        public int SoThangBaoHanh { get; set; }
        public string ImageBase64 { get; set; }
        public byte[] Image { get; set; }
    }
}
