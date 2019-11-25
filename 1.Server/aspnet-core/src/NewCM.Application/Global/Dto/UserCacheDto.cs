namespace NewCM.Global.Dto
{
    public class UserCacheDto
    {
        // AbpUser -> Id
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public bool IsCustomer { get; set; }

        // KhachHang -> Id
        public long CustomerId { get; set; }
    }
}
