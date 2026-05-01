namespace Visus.AddressValidation.Integration.Google.Validation;

using System.Collections.Frozen;
using System.Diagnostics;
using Abstractions;
using AddressValidation.Validation;
using Contracts;

internal sealed class ApiResponseValidator : AbstractValidator<ApiResponse>
{
    private static readonly FrozenSet<Granularity> ConfirmedGranularity =
    [
        Granularity.PREMISE,
        Granularity.SUB_PREMISE,
    ];

    private static readonly FrozenSet<ConfirmationLevel> TenuousConfirmations =
    [
        ConfirmationLevel.UNCONFIRMED_BUT_PLAUSIBLE,
        ConfirmationLevel.UNCONFIRMED_AND_SUSPICIOUS,
    ];

    protected override ValueTask<bool> PreValidateAsync(ApiResponse instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        // terminate validation early if there is an error response object
        if ( instance.ErrorResponse is not null )
        {
            if ( instance.ErrorResponse.Error is not null )
            {
                results.Add(string.IsNullOrWhiteSpace(instance.ErrorResponse.Error.Message)
                                ? ValidationState.CreateError(instance.ErrorResponse.Error.Code.ToString())
                                : ValidationState.CreateError($"{instance.ErrorResponse.Error.Code}: {instance.ErrorResponse.Error.Message}"));
            }

            return ValueTask.FromResult(false);
        }

        // terminate validation early if there is no response object
        if ( instance.Result is null )
        {
            return ValueTask.FromResult(false);
        }

        ApiResponse.Verdict verdict = instance.Result.Verdict;

        // only exit early as cleanly confirmed when granularity is strong, address is complete,
        // and the API made no modifications (inferred, replaced, or spell-corrected components
        // are independent CONFIRM signals regardless of granularity)
        if ( ConfirmedGranularity.Contains(verdict.ValidationGranularity)
          && verdict is
             {
                 AddressComplete: true,
                 HasInferredComponents: false,
                 HasReplacedComponents: false,
                 HasSpellCorrectedComponents: false,
             } )
        {
            return ValueTask.FromResult(false);
        }

        if ( verdict is not { ValidationGranularity: Granularity.OTHER, AddressComplete: false, } )
        {
            return ValueTask.FromResult(true);
        }

        // if the response coming back has a validationGranularity = OTHER and addressComplete = false
        // terminate the validation early.
        results.Add(ValidationState.CreateError(nameof(VerificationResult.UNVERIFIED)));
        return ValueTask.FromResult(false);
    }

    protected override ValueTask ValidateAsync(ApiResponse instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        Debug.Assert(instance.Result is not null);

        ApiResponse.Verdict verdict = instance.Result.Verdict;

        // any API modification (inferred, replaced, or spell-corrected components) is a CONFIRM signal
        // per Google's documentation, regardless of granularity
        if ( verdict.HasInferredComponents || verdict.HasReplacedComponents || verdict.HasSpellCorrectedComponents )
        {
            results.Add(ValidationState.CreateWarning(nameof(VerificationResult.PARTIALLY_VERIFIED)));
        }

        // for US addresses, USPS DPV confirmation is authoritative:
        // "S" means primary matched but secondary number is missing (CONFIRM)
        // "N" or absent means the address could not be confirmed for delivery (FIX)
        if ( instance.Result.UspsData is { DpvConfirmation: { } dpv, } )
        {
            switch ( dpv )
            {
                case "S":
                    results.Add(ValidationState.CreateWarning(nameof(VerificationResult.PARTIALLY_VERIFIED)));
                    break;
                case not "Y":
                    results.Add(ValidationState.CreateError(nameof(VerificationResult.UNVERIFIED)));
                    break;
            }
        }

        foreach ( ApiResponse.AddressComponent component in instance.Result.Address.AddressComponents )
        {
            if ( component.ComponentName is null || !TenuousConfirmations.Contains(component.ConfirmationLevel) )
            {
                continue;
            }

            results.Add(ValidationState.CreateWarning($"{component.ComponentName.Text!}: {component.ConfirmationLevel}"));
        }

        return ValueTask.CompletedTask;
    }
}
