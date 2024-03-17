namespace DotNetAuth.Models.DTO
{
    public class UserInfoDTO
    {
        public string Username { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
