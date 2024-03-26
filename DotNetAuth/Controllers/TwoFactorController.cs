using System.Text.Encodings.Web;
using DotNetAuth.Models.DTO;
using DotNetAuth.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAuth.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TwoFactorController : ControllerBase
    {

        private UserManager<IdentityUser> _userManager;
        private UrlEncoder _urlEncoder;
        public TwoFactorController(UserManager<IdentityUser> userManager, UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _urlEncoder = urlEncoder;

        }

        [HttpGet("secret")]
        public async Task<IActionResult> Get2FaSecret()
        {
            var user = await _userManager.GetUserAsync(User);

            var key = await _userManager.GetAuthenticatorKeyAsync(user); // get the key
            if (string.IsNullOrEmpty(key))
            {
                // if no key exists, generate one and persist it
                await _userManager.ResetAuthenticatorKeyAsync(user);
                // get the key we just created
                key = await _userManager.GetAuthenticatorKeyAsync(user);

            }
            var email = await _userManager.GetEmailAsync(user);


            var SharedKey = TwoFactorUtils.FormatKey(key);
            var QrCodeUri = TwoFactorUtils.GenerateQrCodeUri(_urlEncoder.Encode(email), _urlEncoder.Encode("Microsoft.AspNetCore.Identity.UI"), key);


            var twoFactorSecretDTO = new TwoFactorSecretDTO()
            {
                Secret = SharedKey,
                QRCodeURI = QrCodeUri
            };
            return Ok(twoFactorSecretDTO);
        }


        [HttpPost("enable")]
        public async Task<IActionResult> Enable2FA(TwoFactorVerificationCodeDTO factorVerificationCodeDto)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Oeps er iets fout gegaan");
            }

            var verificationCode = factorVerificationCodeDto.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);


            if (!is2faTokenValid)
            {
                return BadRequest("Oeps er iets fout gegaan");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            var userId = await _userManager.GetUserIdAsync(user);

            return Ok();
        }

    }
}
