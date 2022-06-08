using System.ComponentModel.DataAnnotations;

namespace user.Models
{
    public class UserBasicDetails
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pin { get; set; }
        public string Phone { get; set; }

        public string UserType {get;set;}

    }
}
