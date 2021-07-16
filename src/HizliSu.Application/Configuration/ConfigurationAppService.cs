using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using HizliSu.Configuration.Dto;

namespace HizliSu.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : HizliSuAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
