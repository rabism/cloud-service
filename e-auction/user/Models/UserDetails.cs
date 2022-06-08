using System.ComponentModel.DataAnnotations;

namespace user.Models
{
    public class UserDetails
    {
        [Key]
        [Required(ErrorMessage ="Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
        ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage ="FirstName is required")]
        [StringLength(30,MinimumLength =5,ErrorMessage ="The {0} value must be between {2} to {1} characters.")]
        public string FirstName {get;set;}

        [Required(ErrorMessage ="LastName is required")]
        [StringLength(25,MinimumLength =3,ErrorMessage ="The {0} value must be between {2} to {1} characters.")]
        public string LastName {get;set;}

        public string Address {get;set;}

        public string City {get;set;}

        public string State {get;set;}

        public string Pin {get;set;}

        [Required(ErrorMessage ="Phone is required")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10,MinimumLength =10,ErrorMessage ="The {0} must be {1} characters length")]
        public string Phone {get;set;}

        [Required(ErrorMessage ="user type is required")]
        public string UserType {get;set;}

        [Required(ErrorMessage ="password is required")]
        public string Password { get; set; }
        
    }
}
