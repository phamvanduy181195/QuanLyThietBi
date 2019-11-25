using System;
using System.Collections.Generic;
using System.Text;

namespace NewCM.Authorization.Accounts.Dto
{
    public class ActiveAccountInput
    {
        public string UserName { get; set; }

        public string ActiveCode { get; set; }
    }
}
