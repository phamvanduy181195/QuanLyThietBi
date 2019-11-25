using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewCM.CongViecs.Dto
{
    public class ThongTinDinhKemKetQuaCongViecDto : EntityDto<int?>
    {
        public long FileSize { get; set; }
        public string FileURL { get; set; }
        public string FileName { get; set; }
    }
}
