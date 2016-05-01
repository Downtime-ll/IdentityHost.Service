namespace IdentityServer3.AspNetIdentity
{
    public class AspNetCoreIdentityOptions
    {
        public string DisplayNameClaimType { get; set; }

        public bool EnableSecurityStamp { get; set; } = true;

        public bool DisableExternalAccountCreation { get; set; }
    }
}
