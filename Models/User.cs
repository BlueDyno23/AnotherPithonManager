namespace AnotherPithonManager.Models
{
    public class User
    {
        public List<Volunteer> Volunteers { get; set; } = new List<Volunteer>();
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }

        public User()
        {

        }
    }
}
