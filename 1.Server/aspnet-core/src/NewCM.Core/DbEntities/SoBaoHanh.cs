using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("SoBaoHanh")]
    public class SoBaoHanh : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string Name { get; set; }
        public virtual string SoSerial { get; set; }
        public virtual string Hang { get; set; }
        public virtual bool QuanTam { get; set; }
        public virtual DateTime? NgayMua { get; set; }
        public virtual int SoThangBaoHanh { get; set; }
        public virtual byte[] Image { get; set; }
    }
}
