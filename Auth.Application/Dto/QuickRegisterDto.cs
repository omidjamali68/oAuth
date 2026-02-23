using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Auth.Application.Dto
{
    public record QuickRegisterDto
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        public uint VerificationCode { get; set; }

        [JsonIgnore]
        public string? UserIp { get; set; }
    }
}