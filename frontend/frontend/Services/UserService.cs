using frontend.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace frontend.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Get users by searching username
        /// </summary>
        /// <param name="searchUserName"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        [Description("Get user list by searching username")]
        public Task<List<UserObjectDTO>> GetUsers(string? searchUserName, int top = 10);
        /// <summary>
        /// Get single user by username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserObjectDTO? GetSingleUser(string userName);
        /// <summary>
        /// Get total user count
        /// </summary>
        /// <returns></returns>
        public int GetTotalUserCount();
    }
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<UserObjectDTO>> GetUsers(string? searchUserName, int top = 10)
        {
            if (string.IsNullOrEmpty(searchUserName))
            {
                return await _context.Users
                    .Select(a => new UserObjectDTO { UserName = a.UserName, Email = a.Email, PhoneNumber = a.PhoneNumber })
                    .Take(top).ToListAsync();
            }
            return await _context.Users
                .Select(a => new UserObjectDTO { UserName = a.UserName, Email = a.Email, PhoneNumber = a.PhoneNumber })
                .Take(top)
                .ToListAsync();
        }
        public UserObjectDTO? GetSingleUser(string userName)
        {
            return _context.Users
                .Where(u => u.UserName == userName)
                .Select(u => new UserObjectDTO { UserName = u.UserName, Email = u.Email, PhoneNumber = u.PhoneNumber })
                .FirstOrDefault();
        }
        public int GetTotalUserCount()
        {
            return _context.Users.Count();
        }
    }
    public class UserObjectDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
