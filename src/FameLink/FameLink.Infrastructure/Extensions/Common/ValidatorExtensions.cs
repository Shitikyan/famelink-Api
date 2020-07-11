using FameLink.Data.Models.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace FameLink.Infrastructure.Extensions.Common
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> UniqueEmail<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            UserManager<FameLinkUser> userManager)
        {
            return ruleBuilder.MustAsync(async (email, cancellationToken) =>
                 !await EmailExists(userManager, email)
            ).WithMessage("The email already exists.");
        }

        public static IRuleBuilderOptions<T, string> ExistingEmail<T>(
           this IRuleBuilder<T, string> ruleBuilder,
           UserManager<FameLinkUser> userManager)
        {
            return ruleBuilder.MustAsync(async (email, cancellationToken) =>
                 await EmailExists(userManager, email)
            ).WithMessage("The email does not exist.");
        }

        public static IRuleBuilderOptions<T, Guid> ExistingUserId<T>(
            this IRuleBuilder<T, Guid> ruleBuilder,
            UserManager<FameLinkUser> userManager)
        {
            return ruleBuilder.MustAsync(async (id, cancellationToken) =>
                 await userManager.FindByIdAsync(id.ToString()) != null
            ).WithMessage("Not existing userId.");
        }

        private static async Task<bool> EmailExists(
            UserManager<FameLinkUser> userManager,
            string email) =>
            await userManager.FindByEmailAsync(email) != null;
    }
}
