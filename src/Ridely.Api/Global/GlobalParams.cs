namespace Ridely.Api.Global
{
    public static class GlobalParams
    {
        // providers
        public static readonly string APPPROVIDER = "APPPROVIDER";
        public static readonly string GOOGLEPROVIDER = "GOOGLEPROVIDER";
        public static readonly string FACEBOOKPROVIDER = "FACEBOOKPROVIDER";
        public static readonly List<string> ROLES = new List<string> {
            // Admin roles
            AdminRoles.Admin,
            AdminRoles.Staff,
            AdminRoles.SuperAdmin,
            //App Roles
            AppRoles.User,
            AppRoles.Driver
        };

        public static class AdminRoles
        {
            public static readonly string Admin = "ADMIN";
            public static readonly string Staff = "STAFF";
            public static readonly string SuperAdmin = "SUPER-ADMIN";
        }
        public static class AppRoles
        {
            public static readonly string User = "USER";
            public static readonly string Driver = "DRIVER";
        }

    }

}
