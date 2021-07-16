using System.Threading.Tasks;
using HizliSu.Configuration.Dto;

namespace HizliSu.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
