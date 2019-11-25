using Abp.Domain.Entities;
using NewCM.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("NhanVienQuanHuyen")]
    public class NhanVienQuanHuyen: Entity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual long NhanVienId { get; set; }

        public virtual int QuanHuyenId { get; set; }

        public virtual User NhanVien { get; set; }
    }
}
