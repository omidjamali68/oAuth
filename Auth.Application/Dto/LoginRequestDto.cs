using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Auth.Application.Dto
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        [JsonIgnore]
        public string? UserIp { get; set; }
        [JsonIgnore]
        public string? UserAgent { get; set; }
    }
}
