using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriGuard.Infrastructure.Email
{
    public class EmailTemplates
    {
        public static string PasswordResetOtp(string otp)
        {
            return $$"""
        <h2>NutriGuard</h2>

        <p>Your OTP is:</p>

        <h1>{{otp}}</h1>

        <p>This code expires in 5 minutes.</p>
        """;
        }

    }
}
