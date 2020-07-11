using FameLink.Common.Models;
using FameLink.Data.Models.Identity;
using FameLink.Domain.Commands.Auth;
using FameLink.Domain.Repositories.Auth;
using FameLink.Domain.Responses.Auth;
using FameLink.Domain.UnitOfWork;
using FameLink.Infrastructure.Abstractions;
using FameLink.Infrastructure.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FameLink.Domain.CommandHandlers
{
    public class AuthCommandsHandler : DomainHandlerBase<AuthCommandsHandler>,
                                       IRequestHandler<SignInCommand, ResponseModel<AccessToken>>,
                                       IRequestHandler<SignUpCommand, ResponseModel<AccessToken>>
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<FameLinkUser> _userManager;
        private readonly SignInManager<FameLinkUser> _signInManager;
        private readonly JwtTokenHandler _jwtTokenHandler;

        public AuthCommandsHandler(
            ILogger<AuthCommandsHandler> logger,
            IUnitOfWork unitOfWork,
            UserManager<FameLinkUser> userManager,
            SignInManager<FameLinkUser> signInManager,
            JwtTokenHandler jwtTokenHandler,
            IIdentityService identityService) : base(logger, unitOfWork, identityService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenHandler = jwtTokenHandler;
            _userRepository = unitOfWork.Users;
        }

        public async Task<ResponseModel<AccessToken>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Sign in attempt for email '{request.Email}'.");
            var user = await _userManager.FindByEmailAsync(request.Email);
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
            {
                _logger.LogInformation($"Sign in failed for email '{request.Email}'. Wrong credentials.");
                return ResponseModel<AccessToken>.SetError("Sign in failed");
            }

            _logger.LogInformation($"Sign in success for email '{request.Email}'.");

            var token = await _userRepository.UpdateAndReturnUserToken(user);
            return new ResponseModel<AccessToken>(token);
        }

        public async Task<ResponseModel<AccessToken>> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Requested user registartion for {request.Email}.");

            var user = request.Adapt<FameLinkUser>();
            var identityResult = await _userRepository.Add(user, request.Password, "");
            if (!identityResult.Succeeded)
            {
                return ResponseModel<AccessToken>.SetError("Unable to create a user.");
            }

            _logger.LogInformation($"Successfuly created user for {user.Email}. Sending confirmation e-mail.");

            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var resetLink = $"{_settings.ConfirmEmailUrl}/{user.Id}/{HttpUtility.UrlEncode(token)}";
            //await _emailService.SendEmailAsync(new EmailData
            //{
            //    EmailAddress = user.Email,
            //    TemplateId = _emailOptions.ResetPasswordTemplate,
            //    FullName = user.FullName,
            //    ResetLink = resetLink,
            //});

            _logger.LogInformation($"Sent confirmation e-mail [{user.Email}].");

            var accessToken = await _userRepository.UpdateAndReturnUserToken(user);
            return new ResponseModel<AccessToken>
            {
                Item = accessToken,
                StatusCode = HttpStatusCode.Created,
            };
        }
    }
}
