using System.ComponentModel.DataAnnotations;

namespace Auth.Application.Dto
{
    public class SendVerificationCodeRequestDto
    {
        [Required]
        public string PhoneNumber { get; set; }
    }
}
