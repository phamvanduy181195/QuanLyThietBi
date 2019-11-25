using System;
using System.Collections.Generic;
using System.Text;

namespace NewCM.Global.Dto
{
    public class LookupTableDto : LookupTableDto<int>
    {
    }

    public class LookupTableDto<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }

        public string DisplayName { get; set; }
    }
}
