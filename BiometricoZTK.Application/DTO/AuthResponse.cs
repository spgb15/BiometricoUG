using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometricoZTK.Application.DTO
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }

    }

    public class AuthRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
