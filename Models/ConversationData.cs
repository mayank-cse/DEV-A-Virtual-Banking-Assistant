using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevVirtualBankingAssistant.Models
{
    public class ConversationData
    {
        // Track whether we have already asked the user's name
        public bool PromptedUserForOTP { get; set; } = false;
    }
}
