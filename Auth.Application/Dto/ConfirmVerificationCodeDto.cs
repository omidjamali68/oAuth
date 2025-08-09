using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Auth.Application.Dto
{
    public class ConfirmVerificationCodeDto
    {
        [Required] 
        public string PhoneNumber { get; set; }
        [Required] 
        public uint VerificationCode { get; set; }

        [JsonIgnore]        
        public string? UserIp { get; set; }
    }
}
