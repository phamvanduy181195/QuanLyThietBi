using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("DanhMucQuanHuyen")]
    public class DanhMucQuanHuyen : Entity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual int TinhThanhId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}
