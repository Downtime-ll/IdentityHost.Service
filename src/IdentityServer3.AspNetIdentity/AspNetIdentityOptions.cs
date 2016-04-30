namespace IdentityServer3.AspNetIdentity
{
    public class AspNetIdentityOptions
    {
        public string DisplayNameClaimType { get; set; }

        public bool EnableSecurityStamp { get; set; } = true;

        public bool DisableExternalAccountCreation { get; set; }
    }
}
