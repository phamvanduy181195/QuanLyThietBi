using System;
using System.Collections.Generic;

namespace NewCM.Sessions.Dto
{
    public class ApplicationInfoDto
    {
        public string Version { get; set; }

        public string AndroidVersion { get; set; }

        public string IosVersion { get; set; }

        public DateTime ReleaseDate { get; set; }

        public Dictionary<string, bool> Features { get; set; }
    }
}
