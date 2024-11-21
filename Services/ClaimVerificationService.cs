using CMCSapp_ST10311777.Models;
using Microsoft.AspNetCore.Mvc;

namespace CMCSapp_ST10311777.Services
{
    public class ClaimVerificationService : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ClaimVerificationService> _logger;

        // Policy configuration settings
        private decimal _maxHoursPerClaim;
        private decimal _maxHourlyRate;
        private decimal _maxTotalClaimAmount;

        public ClaimVerificationService(IConfiguration configuration, ILogger<ClaimVerificationService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Load verification policies from configuration
            LoadPolicySettings();
        }

        private void LoadPolicySettings()
        {
            // Load policy settings from appsettings.json or other configuration source
            _maxHoursPerClaim = _configuration.GetValue("ClaimPolicies:MaxHoursPerClaim", 45m);
            _maxHourlyRate = _configuration.GetValue("ClaimPolicies:MaxHourlyRate", 200m);
            _maxTotalClaimAmount = _configuration.GetValue("ClaimPolicies:MaxTotalClaimAmount", 10000m);
        }

        public (bool IsValid, List<string> ValidationErrors) VerifyClaim(ClaimTable claim)
        {
            var errors = new List<string>();

            // 1. Validate Hours Worked
            if (claim.hoursWorked <= 0)
                errors.Add("Hours worked must be greater than zero.");
            else if (claim.hoursWorked > _maxHoursPerClaim)
                errors.Add($"Hours worked cannot exceed {_maxHoursPerClaim} hours per claim.");

            // 2. Validate Hourly Rate
            if (claim.hourlyRate <= 0)
                errors.Add("Hourly rate must be greater than zero.");
            else if (claim.hourlyRate > _maxHourlyRate)
                errors.Add($"Hourly rate cannot exceed {_maxHourlyRate} per hour.");

            // 3. Calculate and Validate Total Claim Amount
            decimal calculatedTotalAmount = claim.hoursWorked * claim.hourlyRate;
            if (calculatedTotalAmount > _maxTotalClaimAmount)
                errors.Add($"Total claim amount cannot exceed {_maxTotalClaimAmount}.");

            // 4. Additional Complex Validation (example of a business rule)
            if (claim.claimDate > DateTime.Now)
                errors.Add("Claim date cannot be in the future.");

            // Log validation results
            if (errors.Count > 0)
            {
                _logger.LogWarning("Claim {ClaimId} failed validation: {Errors}",
                    claim.claimID, string.Join(", ", errors));
            }

            return (errors.Count == 0, errors);
        }

        public string DetermineAutoApprovalStatus(ClaimTable claim)
        {
            var (isValid, validationErrors) = VerifyClaim(claim);

            if (isValid)
            {
                // Automatically approve claims that meet all criteria
                if (claim.totalAmount <= _maxTotalClaimAmount / 2)
                {
                    _logger.LogInformation("Claim {ClaimId} auto-approved", claim.claimID);
                    return "Auto-Approved";
                }
                else
                {
                    _logger.LogInformation("Claim {ClaimId} requires manual review", claim.claimID);
                    return "Pending Review";
                }
            }
            else
            {
                _logger.LogWarning("Claim {ClaimId} rejected due to validation errors", claim.claimID);
                return "Rejected";
            }
        }
    }
}
