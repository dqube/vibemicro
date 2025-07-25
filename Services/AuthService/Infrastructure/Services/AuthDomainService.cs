using BuildingBlocks.Domain.Services;
using BuildingBlocks.Domain.BusinessRules;
using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Repositories;
using AuthService.Domain.BusinessRules;
using AuthService.Domain.Exceptions;
using AuthService.Domain.Services;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Domain.Repository;

namespace AuthService.Infrastructure.Services;

/// <summary>
/// Domain service implementation for authentication operations
/// </summary>
public class AuthDomainService : DomainServiceBase, IAuthDomainService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IRegistrationTokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthDomainService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IRegistrationTokenRepository tokenRepository,
        IUnitOfWork unitOfWork,
        ILogger<AuthDomainService> logger) : base(logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(Username username, string password, CancellationToken cancellationToken = default)
    {
        LogOperationStart(nameof(AuthenticateAsync), new { Username = username.Value });

        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (user == null)
        {
            Logger.LogWarning("Authentication failed: User '{Username}' not found", username.Value);
            return AuthenticationResult.Failure("Invalid username or password.", AuthenticationFailureReason.UserNotFound);
        }

        return await ValidateAndAuthenticateUser(user, password);
    }

    public async Task<AuthenticationResult> AuthenticateByEmailAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        LogOperationStart(nameof(AuthenticateByEmailAsync), new { Email = email });

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            Logger.LogWarning("Authentication failed: User with email '{Email}' not found", email);
            return AuthenticationResult.Failure("Invalid email or password.", AuthenticationFailureReason.UserNotFound);
        }

        return await ValidateAndAuthenticateUser(user, password);
    }

    public async Task<RegistrationResult> RegisterUserAsync(Username username, string email, string password, CancellationToken cancellationToken = default)
    {
        LogOperationStart(nameof(RegisterUserAsync), new { Username = username.Value, Email = email });

        // Validate business rules
        var usernameUniqueRule = new UsernameMustBeUniqueRule(username, async u => 
            await _userRepository.IsUsernameUniqueAsync(u, cancellationToken: cancellationToken));
        
        var emailUniqueRule = new EmailMustBeUniqueRule(email, async (e, excludeId) => 
            await _userRepository.IsEmailUniqueAsync(e, excludeId, cancellationToken));

        var passwordStrengthRule = new PasswordMustMeetStrengthRequirementsRule(password);

        if (await usernameUniqueRule.IsBrokenAsync())
        {
            Logger.LogWarning("Registration failed: {Message}", usernameUniqueRule.Message);
            return RegistrationResult.Failure(usernameUniqueRule.Message);
        }

        if (await emailUniqueRule.IsBrokenAsync())
        {
            Logger.LogWarning("Registration failed: {Message}", emailUniqueRule.Message);
            return RegistrationResult.Failure(emailUniqueRule.Message);
        }

        if (passwordStrengthRule.IsBroken())
        {
            Logger.LogWarning("Registration failed: {Message}", passwordStrengthRule.Message);
            return RegistrationResult.Failure(passwordStrengthRule.Message);
        }

        // Create user
        var user = User.Create(username, email, password);
        await _userRepository.AddAsync(user, cancellationToken);

        // Create email verification token
        var verificationToken = RegistrationToken.CreateEmailVerification(user.Id);
        await _tokenRepository.AddAsync(verificationToken, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("User '{Username}' registered successfully with ID '{UserId}'", username.Value, user.Id);
        return RegistrationResult.Success(user, verificationToken);
    }

    public async Task<PasswordResetResult> InitiatePasswordResetAsync(string email, CancellationToken cancellationToken = default)
    {
        LogOperationStart(nameof(InitiatePasswordResetAsync), new { Email = email });

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            // Don't reveal that the email doesn't exist for security
            Logger.LogInformation("Password reset requested for unknown email '{Email}'", email);
            return PasswordResetResult.Success();
        }

        if (!user.IsActive)
        {
            Logger.LogWarning("Password reset failed: User '{UserId}' is inactive", user.Id);
            return PasswordResetResult.Failure("Account is inactive.");
        }

        // Invalidate existing password reset tokens
        await _tokenRepository.InvalidateTokensForUserAsync(user.Id, TokenType.PasswordReset, cancellationToken);

        // Create new password reset token
        var resetToken = RegistrationToken.CreatePasswordReset(user.Id);
        await _tokenRepository.AddAsync(resetToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Password reset token created for user '{UserId}'", user.Id);
        return PasswordResetResult.Success(resetToken);
    }

    public async Task<PasswordResetResult> CompletePasswordResetAsync(TokenId tokenId, string newPassword, CancellationToken cancellationToken = default)
    {
        LogOperationStart(nameof(CompletePasswordResetAsync), new { TokenId = tokenId });

        var token = await _tokenRepository.GetByIdAsync(tokenId, cancellationToken);
        if (token == null)
        {
            Logger.LogWarning("Password reset failed: Token '{TokenId}' not found", tokenId);
            return PasswordResetResult.Failure("Invalid or expired reset token.");
        }

        if (!token.IsValid() || !token.TokenType.IsPasswordReset)
        {
            Logger.LogWarning("Password reset failed: Token '{TokenId}' is invalid or wrong type", tokenId);
            return PasswordResetResult.Failure("Invalid or expired reset token.");
        }

        var user = await _userRepository.GetByIdAsync(token.UserId, cancellationToken);
        if (user == null)
        {
            Logger.LogWarning("Password reset failed: User '{UserId}' not found for token '{TokenId}'", token.UserId, tokenId);
            return PasswordResetResult.Failure("User not found.");
        }

        var passwordStrengthRule = new PasswordMustMeetStrengthRequirementsRule(newPassword);
        if (passwordStrengthRule.IsBroken())
        {
            Logger.LogWarning("Password reset failed: {Message}", passwordStrengthRule.Message);
            return PasswordResetResult.Failure(passwordStrengthRule.Message);
        }

        // Reset password and use token
        user.ResetPassword(newPassword, null); // System operation
        token.Use();

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _tokenRepository.UpdateAsync(token, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Password reset completed for user '{UserId}'", user.Id);
        return PasswordResetResult.Success();
    }

    public async Task<EmailVerificationResult> InitiateEmailVerificationAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        LogOperationStart(nameof(InitiateEmailVerificationAsync), new { UserId = userId });

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            Logger.LogWarning("Email verification failed: User '{UserId}' not found", userId);
            return EmailVerificationResult.Failure("User not found.");
        }

        // Invalidate existing email verification tokens
        await _tokenRepository.InvalidateTokensForUserAsync(userId, TokenType.EmailVerification, cancellationToken);

        // Create new email verification token
        var verificationToken = RegistrationToken.CreateEmailVerification(userId);
        await _tokenRepository.AddAsync(verificationToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Email verification token created for user '{UserId}'", userId);
        return EmailVerificationResult.Success(verificationToken);
    }

    public async Task<EmailVerificationResult> VerifyEmailAsync(TokenId tokenId, CancellationToken cancellationToken = default)
    {
        LogOperationStart(nameof(VerifyEmailAsync), new { TokenId = tokenId });

        var token = await _tokenRepository.GetByIdAsync(tokenId, cancellationToken);
        if (token == null)
        {
            Logger.LogWarning("Email verification failed: Token '{TokenId}' not found", tokenId);
            return EmailVerificationResult.Failure("Invalid or expired verification token.");
        }

        if (!token.IsValid() || !token.TokenType.IsEmailVerification)
        {
            Logger.LogWarning("Email verification failed: Token '{TokenId}' is invalid or wrong type", tokenId);
            return EmailVerificationResult.Failure("Invalid or expired verification token.");
        }

        var user = await _userRepository.GetByIdAsync(token.UserId, cancellationToken);
        if (user == null)
        {
            Logger.LogWarning("Email verification failed: User '{UserId}' not found for token '{TokenId}'", token.UserId, tokenId);
            return EmailVerificationResult.Failure("User not found.");
        }

        // Activate user if not already active and use token
        if (!user.IsActive)
        {
            user.Activate();
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        token.Use();
        await _tokenRepository.UpdateAsync(token, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Email verification completed for user '{UserId}'", user.Id);
        return EmailVerificationResult.Success();
    }

    public async Task<PasswordChangeResult> ChangePasswordAsync(UserId userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        LogOperationStart(nameof(ChangePasswordAsync), new { UserId = userId });

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            Logger.LogWarning("Password change failed: User '{UserId}' not found", userId);
            return PasswordChangeResult.Failure("User not found.");
        }

        var passwordStrengthRule = new PasswordMustMeetStrengthRequirementsRule(newPassword);
        if (passwordStrengthRule.IsBroken())
        {
            Logger.LogWarning("Password change failed: {Message}", passwordStrengthRule.Message);
            return PasswordChangeResult.Failure(passwordStrengthRule.Message);
        }

        try
        {
            user.ChangePassword(currentPassword, newPassword, userId);
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("Password changed successfully for user '{UserId}'", userId);
            return PasswordChangeResult.Success();
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogWarning("Password change failed for user '{UserId}': {Error}", userId, ex.Message);
            return PasswordChangeResult.Failure(ex.Message);
        }
    }

    public async Task<bool> HasRoleAsync(UserId userId, RoleId requiredRole, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user?.HasRole(requiredRole) ?? false;
    }

    public async Task<bool> HasAnyRoleAsync(UserId userId, IEnumerable<RoleId> requiredRoles, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user?.HasAnyRole(requiredRoles.ToArray()) ?? false;
    }

    private async Task<AuthenticationResult> ValidateAndAuthenticateUser(User user, string password)
    {
        // Check if user is active
        if (!user.IsActive)
        {
            Logger.LogWarning("Authentication failed: User '{UserId}' is inactive", user.Id);
            return AuthenticationResult.Failure("Account is inactive.", AuthenticationFailureReason.UserInactive);
        }

        // Check if user is locked out
        if (user.IsLockedOut())
        {
            Logger.LogWarning("Authentication failed: User '{UserId}' is locked out until {LockoutEnd}", user.Id, user.LockoutEnd);
            return AuthenticationResult.Failure("Account is locked out.", AuthenticationFailureReason.UserLockedOut);
        }

        // Verify password
        if (!user.VerifyPassword(password))
        {
            user.RecordFailedLoginAttempt();
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            Logger.LogWarning("Authentication failed: Invalid password for user '{UserId}'. Failed attempts: {FailedAttempts}", 
                user.Id, user.FailedLoginAttempts);
            return AuthenticationResult.Failure("Invalid username or password.", AuthenticationFailureReason.InvalidPassword);
        }

        // Reset failed login attempts on successful authentication
        if (user.FailedLoginAttempts > 0)
        {
            user.ResetFailedLoginAttempts();
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        Logger.LogInformation("User '{UserId}' authenticated successfully", user.Id);
        return AuthenticationResult.Success(user);
    }
} 