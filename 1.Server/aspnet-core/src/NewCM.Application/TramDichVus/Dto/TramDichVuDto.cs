using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using NewCM.DbEntities;
using System;
using System.ComponentModel.DataAnnotations;

namespace NewCM.TramDichVus.Dto
{
    [AutoMap(typeof(TramDichVu))]
    public class TramDichVuDto : EntityDto
    {
        [Required]
        [StringLength(TramDichVu.MaxCodeLength)]
        public string Code { get; set; }

        [Required]
        [StringLength(TramDichVu.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        public long TramTruongId { get; set; }

        public string DiaChi { get; set; }

        public int? DiaChiQuanHuyenId { get; set; }

        public int? DiaChiTinhThanhId { get; set; }

        public DateTime? NgayHoatDong { get; set; }

        [StringLength(TramDichVu.MaxDescriptionLength)]
        public string Description { get; set; }

        public long[] UserIds { get; set; }
    }
}
