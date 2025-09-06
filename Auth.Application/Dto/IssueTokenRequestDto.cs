using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Auth.Application.Dto
{
    public record IssueTokenRequestDto
    {
        [Required]
        public string PhoneNumber { get; set; }        
        [JsonIgnore]
        public string? UserAgent { get; set; }
        [JsonIgnore]
        public string? UserIp { get; set; }
    }
}
