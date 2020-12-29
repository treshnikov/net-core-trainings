using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BusinessLogic.Common;
using BusinessLogic.DAL;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using Domain;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Managers
{
    public class UserManager : IUserManager
    {
        private readonly IRepository<User> _repository;
        private readonly IReadOnlyRepository<UserRole> _userRoleRepository;
        private readonly IUserPrincipal _userPrincipal;
        private readonly IServerSettingsManager _serverSettingsManager;

        public UserManager(
            IRepository<User> repository,
            IReadOnlyRepository<UserRole> userRoleRepository,
            IUserPrincipal userPrincipal,
            IServerSettingsManager serverSettingsManager)
        {
            _repository = repository;
            _userRoleRepository = userRoleRepository;
            _userPrincipal = userPrincipal;
            _serverSettingsManager = serverSettingsManager;
        }
        
        public User Get(long id)
        {
            return _repository.Query()
                       .Include(u => u.Role.Permissions)
                       .ThenInclude(p => p.Permission)
                       .FirstOrDefault(d => d.Id == id) ??
                   throw new NotFoundException("User not found");
        }

        public Paging<User> Get(string filter, int take, int? skip)
        {
            IQueryable<User> query = _repository.Query()
                .OrderBy(u => u.Id);
            
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var filterParser = new FilterParser.FilterParser<User>(filter);
                query = filterParser.Filter(query);
            }

            var totalCount = query.Count();
            
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            query = query.Take(take);

            return new Paging<User>(totalCount, skip ?? 0, query.AsEnumerable());
        }

        public void Add(User user)
        {
            ValidatePassword(user.Password);
            ValidateEmail(user.Email);
            ValidateUserExists(user.Email);

            HashPassword(user);
            
            var roleId = Convert.ToInt32(_serverSettingsManager.GetServerSettings().DefaultUserRole);
            var role = _userRoleRepository.Query().FirstOrDefault(r => r.Id == roleId);
            user.Role = role;
            _repository.Add(user);
        }
        
        public void Update(User user)
        {
            var old = _repository.Query().Include(u => u.Role).FirstOrDefault(u => u.Id == user.Id);
            if (old == null)
            {
                throw new NotFoundException("User not found");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                user.Email = old.Email;
            }
            else
            {
                ValidateEmail(user.Email);
                ValidateUserExists(user.Email);
            }

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                user.UserName = old.UserName;
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = old.Password;
                user.Salt = old.Salt;
            }
            else
            {
                ValidatePassword(user.Password);
                HashPassword(user);
            }
            
            user.Role = old.Role;
            _repository.Update(user.Id, user);
        }

        public void Delete(long id)
        {
            if (!_repository.Query().Any(u => u.Id == id))
            {
                throw new NotFoundException("User not found");
            }
            
            _repository.Delete(id);
        }

        public User FindUser(string email)
        {
            return _repository.Query()
                .Include(u => u.Role.Permissions)
                .ThenInclude(p => p.Permission)
                .FirstOrDefault(u => u.Email == email);
        }

        public bool ComparePassword(User user, string password)
        {
            var hash = MakePasswordHash(password, user.Salt);
            return user.Password == hash;
        }
        
        public void UpdateRole(long userId, int roleId)
        {
            if (_userPrincipal.UserId == userId)
            {
                throw new BadRequestException("You are not allowed to change your own role");
            }
            
            var currentUser = Get(_userPrincipal.UserId); 
            var user = Get(userId);
            if (user.Role.IsSuperUser && !currentUser.Role.IsSuperUser)
            {
                throw new BadRequestException("You are not allowed to manage roles for this user");                
            }
            
            var role = _userRoleRepository.Query().FirstOrDefault(r => r.Id == roleId);
            if (role == null)
            {
                throw new NotFoundException("Role not found");
            }

            if (role.IsSuperUser && !currentUser.Role.IsSuperUser)
            {
                throw new BadRequestException("You are not allowed to assign this role");                
            }

            user.Role = role;
            _repository.Update(user.Id, user);
        }

        private void HashPassword(User user)
        {
            using (var generator = RandomNumberGenerator.Create())  
            {
                var randomBytes = new byte[16];
                generator.GetBytes(randomBytes);  
                user.Salt = Convert.ToBase64String(randomBytes);  
            }
            
            user.Password = MakePasswordHash(user.Password, user.Salt);
        }

        private static string MakePasswordHash(string password, string salt)
        {
            var value = KeyDerivation.Pbkdf2(
                password,
                Encoding.UTF8.GetBytes(salt),
                KeyDerivationPrf.HMACSHA512,
                10000,
                32);
            
            return Convert.ToBase64String(value);
        }
        
        private void ValidatePassword(string password)
        {
            const int minPasswordLength = 8;
            if (password.Length < minPasswordLength)
            {
                throw new BadRequestException($"Password length should be not less than {minPasswordLength}");
            }
        }

        private void ValidateEmail(string email)
        {
            if (!email.Contains("@"))
            {
                throw new BadRequestException("Incorrect email");
            }
        }

        private void ValidateUserExists(string email)
        {
            if (_repository.Query().Any(u => u.Email == email))
            {
                throw new BadRequestException("User with the same email already exists");
            }
        }
    }
}