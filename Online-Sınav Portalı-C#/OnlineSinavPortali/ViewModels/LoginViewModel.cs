using System.ComponentModel.DataAnnotations;

namespace OnlineSinavPortali.ViewModels;

public sealed class LoginViewModel
{
    [Display(Name = "Kullanıcı adı")]
    [Required(ErrorMessage = "Kullanıcı adını girin.")]
    [StringLength(60, ErrorMessage = "Kullanıcı adı en fazla 60 karakter olabilir.")]
    public string UserName { get; set; } = "";

    [Display(Name = "Parola")]
    [Required(ErrorMessage = "Parolayı girin.")]
    [StringLength(128, ErrorMessage = "Parola en fazla 128 karakter olabilir.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    public string? ReturnUrl { get; set; }
}
