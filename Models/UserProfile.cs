using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevVirtualBankingAssistant.Models
{
    public class UserProfile
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool UserAuthenticated { get; set; } = false;
        public bool start { get; set; } = true;
        public int OTP { get; set; }
        public bool attendance { get; set; }
        public string storeName { get; set; }
        public string Message { get; set; }
        public string location { get; set; }
        public bool EmailVerified { get; set; } = false;
        public string ValueFinder { get; set; }
        public string Time { get; set; }
        public string Reason { get; set; }
    }
}
