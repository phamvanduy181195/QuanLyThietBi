using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("KhachHang")]
    public class KhachHang : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string Name { get; set; }
        public virtual DateTime? Birthday { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string Address { get; set; }
        public virtual int? DistrictId { get; set; }
        public virtual int? ProvinceId { get; set; }
        public virtual string Location { get; set; }
        public virtual string Description { get; set; }
        public virtual long? UserId { get; set; }
    }
}
