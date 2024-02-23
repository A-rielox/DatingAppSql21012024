using DatingAppSql21012024.DTOs;
using DatingAppSql21012024.Entities;
using DatingAppSql21012024.Helpers;

namespace DatingAppSql21012024.Interfaces
{
    public interface IMessageRepository

    {
        Task<bool> AddMessage(Message message);
        Task<bool> DeleteMessage(Message message, string userNameDeleting);
        Task<Message> GetMessage(int id);


        //Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessagesForUser(MessageParams messageParams); // el lo hace paginado


        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName);
    }

}
