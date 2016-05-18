using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IdentityManager.AspNetCore.Identity
{
    public static class Ex
    {
        public static string IdentityErrorToString(this IdentityError error)
        {
            return error.Description;
        }
    }
}
