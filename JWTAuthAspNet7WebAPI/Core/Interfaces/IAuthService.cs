using JWTAuthAspNet7WebAPI.Core.Dtos;

namespace JWTAuthAspNet7WebAPI.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthServiceResponseDto> SeedRoleAsync();
        Task<AuthServiceResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthServiceResponseDto> MakeAdminAsync(UpdatePermissionDto updatePermissionDto);
        Task<AuthServiceResponseDto> MakeCreatorAsync(UpdatePermissionDto updatePermissionDto);
    }
}
