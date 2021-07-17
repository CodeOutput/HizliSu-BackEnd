using System.Globalization;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Configuration;
using Abp.UI;
using Abp.Zero.Configuration;
using HizliSu.Authorization.Accounts.Dto;
using HizliSu.Authorization.Users;
using HizliSu.Users.Dto;

namespace HizliSu.Authorization.Accounts
{
    public class AccountAppService : HizliSuAppServiceBase, IAccountAppService
    {
        // from: http://regexlib.com/REDetails.aspx?regexp_id=1923
        public const string PasswordRegex = "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s)[0-9a-zA-Z!@#$%^&*()]*$";

        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly UserManager _userManager;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager, UserManager userManager)
        {
            _userRegistrationManager = userRegistrationManager;
            _userManager = userManager;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            input.EmailAddress = input.EmailAddress.ToLower(new CultureInfo("en-US", false));
            input.UserName = input.UserName.ToLower(new CultureInfo("en-US", false));
            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                true, // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
                input.PhoneNumber
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }

        [AbpAuthorize()]
        public async Task<UserDto> GetAuthUserInfo()
        {
            if (!AbpSession.UserId.HasValue)
            {
                throw new UserFriendlyException("Kullanýcý bulunamadý!");
            } 
            var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
            return ObjectMapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateUser(UserDto input)
        {
            //input.UserName = input.EmailAddress;
            if (!AbpSession.UserId.HasValue)
            {
                throw new UserFriendlyException("Kullanýcý bulunamadý!");
            }
            var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);

            MapToEntity(input, user);

            CheckErrors(await _userManager.UpdateAsync(user));


            return input;
        }

        protected void MapToEntity(UserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }
    }
}
