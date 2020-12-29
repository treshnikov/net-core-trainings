using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.Models;
using Domain;
using JoggingWebApp.Models;

namespace BusinessLogic.Managers
{
    public interface IJoggingDataManager
    {
        JoggingData Get(long id, long userId);
        Paging<JoggingData> Get(long userId, string filter, int take, int? skip);
        Task Add(JoggingData data, long userId);
        void Update(long id, JoggingDataUpdateModel model, long userId);
        void Delete(long id, long userId);
    }
}