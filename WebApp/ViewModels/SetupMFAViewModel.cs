using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class SetupMFAViewModel
    {
        // string will be considered as a required field
        public string? Key { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Code")]
        public string SecurityCode { get; set; } = string.Empty;

        public byte[]? QRCodeBytes { get; set; }
    }
}
