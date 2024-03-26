using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;

namespace DotNetAuth.Utils
{
    public static class TwoFactorUtils
    {
        private static string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public static string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }


        public static string GenerateQrCodeUri(string encodedEmail, string serviceName, string unformattedKey)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
                serviceName,
                encodedEmail,
                unformattedKey);
        }
    }
}
