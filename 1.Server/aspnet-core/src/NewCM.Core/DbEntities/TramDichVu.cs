using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using NewCM.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("TramDichVu")]
    public class TramDichVu : FullAuditedEntity, IMayHaveTenant
    {
        public const int MaxCodeLength = 50;
        public const int MaxNameLength = 255;
        public const int MaxDescriptionLength = 64 * 1024; //64KB

        public int? TenantId { get; set; }

        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual long TramTruongId { get; set; }
        public virtual string DiaChi { get; set; }
        public virtual int? DiaChiQuanHuyenId { get; set; }
        public virtual int? DiaChiTinhThanhId { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime? NgayHoatDong { get; set; }

        public virtual ICollection<User> NhanVien { get; set; }
    }
}
