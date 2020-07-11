using FameLink.Common.Models;
using FameLink.Domain.Responses.Auth;
using MediatR;

namespace FameLink.Domain.Commands.Auth
{
    public class SignUpCommand : IRequest<ResponseModel<AccessToken>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
