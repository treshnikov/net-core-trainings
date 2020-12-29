using System.Collections.Generic;
using BusinessLogic.Models;
using Domain;

namespace BusinessLogic.Managers
{
    public interface IUserManager
    {
        User Get(long id);
        Paging<User> Get(string filter, int take, int? skip);
        void Add(User user);
        void Update(User user);
        void Delete(long id);
        User FindUser(string email);
        bool ComparePassword(User user, string password);
        void UpdateRole(long userId, int roleId);
    }
}