namespace OpenBots.Core.Server.User
{
    public class RegistryKeys
    {
        public string SubKey { get; } = @"SOFTWARE\OpenBots\Agent\Credentials";
        public string UsernameKey { get; } = "Username";
        public string PasswordKey { get; } = "Password";
    }
}
