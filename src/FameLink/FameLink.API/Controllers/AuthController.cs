using FameLink.Api.Models.Auth;
using FameLink.Domain.Commands.Auth;
using FameLink.Infrastructure.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FameLink.API.Controllers
{
    public class AuthController : ApiController<AuthController>
    {
        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn(SignIn model)
        {
            return await HandleRequest<SignInCommand>(model);
        }

        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp(SignUp model)
        {
            return await HandleRequest<SignUpCommand>(model);
        }
    }
}