using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TakeawayOrder.Models
{
    public class UserEditViewModel
    {
        [Required, MinLength(4, ErrorMessage = "Min length is 4")]
        [Display(Name = "Name")]
        public string FullName { get; set; }
        public string Id { get; set; }
        [Required, MinLength(2, ErrorMessage = "Min length is 2")]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required, EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        [DataType(DataType.Password), MinLength(4, ErrorMessage = "Min length is 4")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Role")]
        public Staffs StaffRole { get; set; }


        public UserEditViewModel() { }

        public UserEditViewModel(ApplicationUser user)
        {
            Id = user.Id;
            FullName = user.FullName;
            UserName = user.UserName;
            Email = user.Email;
            Password = user.PasswordHash;
        }
    }
}
