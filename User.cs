namespace LibraryManagementSystem2
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public User() { }

        public User(int userID, string username, string passwordHash, string email, string role)
        {
            UserID = userID;
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
            Role = role;
        }
    }
}
