namespace BiometricoZTK.Application.DTO
{
    public class ChangePasswordRequest
    {
        public string PasswordActual { get; set; } = string.Empty;
        public string NuevaPassword { get; set; } = string.Empty;
    }
}
