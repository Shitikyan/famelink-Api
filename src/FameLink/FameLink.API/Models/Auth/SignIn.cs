using FluentValidation;
using Microsoft.AspNetCore.Identity;
using FameLink.Data.Models.Identity;
using FameLink.Infrastructure.Extensions.Common;

namespace FameLink.Api.Models.Auth
{
    public class SignIn
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class SignInValidator : AbstractValidator<SignIn>
    {
        public SignInValidator(UserManager<FameLinkUser> userManager)
        {
            RuleFor(m => m.Email).NotNull().EmailAddress().ExistingEmail(userManager);
            RuleFor(m => m.Password).NotNull();
        }
    }
}
