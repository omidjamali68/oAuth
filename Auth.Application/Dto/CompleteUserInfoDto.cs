namespace Auth.Application.Dto
{
    public class CompleteUserInfoDto
    {
        public string UserId { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? NationalCode { get; set; }
        public string? Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
