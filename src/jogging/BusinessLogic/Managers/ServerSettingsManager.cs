using System.Linq;
using BusinessLogic.DAL;
using Domain;

namespace BusinessLogic.Managers
{
    public interface IServerSettingsManager
    {
        MainServerSettings GetServerSettings();
    }
    
    public class ServerSettingsManager : IServerSettingsManager
    {
        private readonly IReadOnlyRepository<ServerSettings> _repository;

        public ServerSettingsManager(IReadOnlyRepository<ServerSettings> repository)
        {
            _repository = repository;
        }

        public MainServerSettings GetServerSettings()
        {
            var settings = _repository.Query().ToDictionary(s => s.Name, s => s.Value);
            
            var result = new MainServerSettings();

            foreach (var property in typeof(MainServerSettings).GetProperties())
            {
                if (settings.TryGetValue(property.Name, out var value))
                {
                    property.SetValue(result, value);
                }
            }
            
            return result;
        }
    }
}