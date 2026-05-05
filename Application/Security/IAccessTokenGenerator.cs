using Domain.Users;

namespace Application.Security;

public interface IAccessTokenGenerator
{
    AccessTokenResult Generate(User user);
}
