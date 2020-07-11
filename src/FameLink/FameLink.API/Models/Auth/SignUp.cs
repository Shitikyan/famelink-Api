using FameLink.Data.Models.Identity;
using FameLink.Infrastructure.Extensions.Common;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace FameLink.Api.Models.Auth
{
    public class SignUp
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class SignUpValidator : AbstractValidator<SignUp>
    {
        public SignUpValidator(UserManager<FameLinkUser> userManager)
        {
            RuleFor(m => m.Email).NotNull().EmailAddress().UniqueEmail(userManager);
            RuleFor(m => m.Password).NotNull();
            RuleFor(m => m.ConfirmPassword).Equal(u => u.Password);
        }
    }
}
