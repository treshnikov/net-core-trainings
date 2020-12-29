namespace BusinessLogic.Common
{
    public interface IUserPrincipal
    {
        long UserId { get; }
    }
    
    public class UserPrincipal : IUserPrincipal
    {
        public long UserId { get; }

        public UserPrincipal(long userId)
        {
            UserId = userId;
        }
    }
}