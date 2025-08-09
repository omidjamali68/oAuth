using Microsoft.AspNetCore.Identity;

namespace Auth.Infrastructure.Services
{
    public class PersianIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"رمز عبور باید حداقل {length} کاراکتر باشد."
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "رمز عبور باید حداقل شامل یک کاراکتر غیر الفبایی (مثل @ یا #) باشد."
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"نام کاربری '{userName}' قبلاً ثبت شده است."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = $"رمز عبور باید حداقل شامل یک کاراکتر عددی (0-9) باشد."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = $"رمز عبور باید حداقل شامل یک کاراکتر بزرگ (A-Z) باشد."
            };
        }
    }

}
