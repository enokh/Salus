using Salus.Models;

namespace Salus.Services
{
    public class SessionContext
    {
        public Profile? ActiveProfile { get; set; }
        public bool HasShownPromptToday { get; set; }
    }
}
