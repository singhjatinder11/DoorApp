using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MordenDoors.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage = " Email is Required")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessage = " Email is Required")]
        [Display(Name = "Email")]

        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = " Email is Required")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = " Password is Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class EditEmp
    {
        public IEnumerable<SelectListItem> UserRoles { get; set; }
        public string Email { get; set; }

    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = " Email is Required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        [Remote("IsMailExist", "Account", HttpMethod = "POST", ErrorMessage = "Email Already Exists.")]
        public string Email { get; set; }

        //[Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$",ErrorMessage ="Password must be between 8 and 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = " First Name is Required")]
        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Print Check Name")]
        public string PrintOnCheckName { get; set; }

        public bool status { get; set; }

        [Required(ErrorMessage = " Employee Number is Required")]
        [StringLength(20, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Employee Id")]
        [Remote("IsEmployeeExist", "Account", HttpMethod = "POST", ErrorMessage = "Employee Number Already Exists.")]
        public string EmoployeeNumber { get; set; }

        //[StringLength(5, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 5)]
        [Display(Name = "Social Security Number")]
        public string SSN { get; set; }

        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Address 1")]
        public string AddrLine1 { get; set; }

        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Address 2")]
        public string AddrLine2 { get; set; }

        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please Select Country")]
        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Please Select Province")]
        [Display(Name = "Province")]
        public int? State { get; set; }
        public string stateName { get; set; }

        //[StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 3)]
        //[Display(Name = "Sub - Country")]
        //public string SubCountry { get; set; }

        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Postal Code")]
        public string Postalcode { get; set; }

        [Display(Name = "Date Of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "dd - MM - yyyy")]
        public Nullable<System.DateTime> DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "Hired Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> HiredDate { get; set; }

        [Display(Name = "Create Time")]
        public Nullable<System.DateTime> CreateTime { get; set; }

        [Display(Name = "Last UpdateTime")]
        public Nullable<System.DateTime> LastUpdateTime { get; set; }

        [Display(Name = "Roles")]
        public string UserRole { get; set; }

        public IEnumerable<SelectListItem> UserRoles { get; set; }

        [Display(Name = "Skills")]
        public int[] UserSkill { get; set; }

        public IEnumerable<SelectListItem> UserSkills { get; set; }
        public IEnumerable<SelectListItem> StateList { get; set; }
    }
    public enum Gender
    {
        Male,
        Female,
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = " Email is Required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = " Password is Required")]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
