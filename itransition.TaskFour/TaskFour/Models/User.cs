namespace TaskFour.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? LastActivityTime { get; set; }

        public User()
        {
            Designation = "N/A";
        }
    }
}
