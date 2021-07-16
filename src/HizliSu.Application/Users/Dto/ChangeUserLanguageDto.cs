using System.ComponentModel.DataAnnotations;

namespace HizliSu.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}