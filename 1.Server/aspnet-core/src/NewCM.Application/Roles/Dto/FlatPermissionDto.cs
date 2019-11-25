using Abp.Authorization;
using Abp.AutoMapper;

namespace NewCM.Roles.Dto
{
    [AutoMapFrom(typeof(Permission))]
    public class FlatPermissionDto
    {
        public string Name { get; set; }
        
        public string DisplayName { get; set; }
        
        public string Description { get; set; }
    }
}