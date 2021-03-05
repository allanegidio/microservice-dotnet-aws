using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Account
{
    public class ForgotPasswordModel
    {
      [Required(ErrorMessage = "Email is required.")]
      [Display(Name = "Email")]
      [EmailAddress]
      public string Email { get; set; }
    }
}