namespace Visus.AddressValidation.Integration.Google.Validation;

using System.Collections.Frozen;
using System.Diagnostics;
using Abstractions;
using AddressValidation.Validation;
using Http;

internal sealed class ApiResponseValidator : AbstractValidator<ApiResponse>
{
    private readonly FrozenSet<Granularity> _confirmedGranularity =
    [
        Granularity.PREMISE,
        Granularity.SUB_PREMISE,
    ];

    private readonly FrozenSet<ConfirmationLevel> _tenuousConfirmations =
    [
        ConfirmationLevel.UNCONFIRMED_BUT_PLAUSIBLE,
        ConfirmationLevel.UNCONFIRMED_AND_SUSPICIOUS,
    ];

    private readonly FrozenSet<Granularity> _tenuousGranularity =
    [
        Granularity.BLOCK,
        Granularity.GRANULARITY_UNSPECIFIED,
        Granularity.PREMISE_PROXIMITY,
        Granularity.ROUTE,
    ];

    protected override ValueTask<bool> PreValidateAsync(ApiResponse instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        // terminate validation early if there is an error response object
        if ( instance.ErrorResponse is not null )
        {
            results.Add(string.IsNullOrWhiteSpace(instance.ErrorResponse.Error.Message)
                            ? ValidationState.CreateError(instance.ErrorResponse.Error.Code.ToString())
                            : ValidationState.CreateError("{0}: {1}", instance.ErrorResponse.Error.Code, instance.ErrorResponse.Error.Message));

            return ValueTask.FromResult(false);
        }

        // terminate validation early if there is no response object
        if ( instance.Result is null )
        {
            return ValueTask.FromResult(false);
        }

        // if the response coming back has a validationGranularity of PREMISE or SUB_PREMISE and addressComplete = true
        // terminate validation early.
        if ( _confirmedGranularity.Contains(instance.Result.Verdict.ValidationGranularity) && instance.Result.Verdict.AddressComplete )
        {
            return ValueTask.FromResult(false);
        }

        if ( instance.Result.Verdict is not { ValidationGranularity: Granularity.OTHER, AddressComplete: false, } )
        {
            return ValueTask.FromResult(true);
        }

        // if the response coming back has a validationGranularity = OTHER and addressComplete = false
        // terminate the validation early.
        results.Add(ValidationState.CreateError(VerificationResult.UNVERIFIED.ToString()));
        return ValueTask.FromResult(false);
    }

    protected override ValueTask ValidateAsync(ApiResponse instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        Debug.Assert(instance.Result != null);

        // provide a validation warning if the validationGranularity is BLOCK or GRANULARITY_UNSPECIFIED or PREMISE_PROXIMITY
        // or ROUTE along with having either hasInferredComponents = true or hasReplacedComponents = true
        if ( _tenuousGranularity.Contains(instance.Result.Verdict.ValidationGranularity)
          && ( instance.Result.Verdict.HasInferredComponents || instance.Result.Verdict.HasReplacedComponents ) )
        {
            results.Add(ValidationState.CreateWarning(VerificationResult.PARTIALLY_VERIFIED.ToString()));
        }

        foreach ( ApiResponse.AddressComponent component in instance.Result.Address.AddressComponents.Where(w => w.ComponentName is not null) )
        {
            if ( !_tenuousConfirmations.Contains(component.ConfirmationLevel) )
            {
                continue;
            }

            Debug.Assert(component.ComponentName != null);
            results.Add(ValidationState.CreateWarning("{0}: {1}", component.ComponentName.Text!, component.ConfirmationLevel));
        }

        return ValueTask.CompletedTask;
    }
}
