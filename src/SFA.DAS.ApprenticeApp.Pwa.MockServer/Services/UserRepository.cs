using SFA.DAS.ApprenticeApp.Pwa.MockServer.Interfaces;
using SFA.DAS.ApprenticeApp.Pwa.MockServer.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.MockServer.Services
{
    public class UserRepository : IUserRepository
    {
        private List<User>? _users;

        public UserRepository()
        {
            InitializeData();
        }

        public IEnumerable<User> All => _users;

        public bool DoesItemExist(long id)
        {
            return _users.Any(item => item.Id == id);
        }

        public User Find(long id)
        {
            return _users.FirstOrDefault(item => item.Id == id);
        }
        private void InitializeData()
        {
            _users = new List<User>();

            var user = new User
            {
                Id = 1,
                UserName = "bob@jones.com"
            };

            _users.Add(user);
        }
    }
}