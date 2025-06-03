using System.ComponentModel.DataAnnotations;

namespace TaskFour.Models
{
    public class RegisterModel
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Designation { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }

}
