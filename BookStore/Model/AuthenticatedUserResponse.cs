namespace BookStore.Model
{
    public class AuthenticatedUserResponse
    {
        public string? Token { get; set; }

        public User User { get; set; }
    }
}
