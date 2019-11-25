using System.ComponentModel.DataAnnotations;

namespace NewCM.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}