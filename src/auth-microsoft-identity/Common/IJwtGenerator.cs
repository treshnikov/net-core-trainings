namespace auth_microsoft_identity
{
    public interface IJwtGenerator
 {
     string CreateToken(AppUser user);
 }
}
