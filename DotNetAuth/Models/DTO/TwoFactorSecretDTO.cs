namespace DotNetAuth.Models.DTO
{
    public class TwoFactorSecretDTO
    {
        public string Secret { get; set; }
        public string QRCodeURI { get; set; }
    }
}
