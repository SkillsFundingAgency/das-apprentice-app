using SFA.DAS.ApprenticeApp.Pwa.MockServer.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.MockServer.Interfaces
{
    public interface IUserRepository
    {
        bool DoesItemExist(long id);
        IEnumerable<User> All { get; }
        User Find(long id);
    }
}
