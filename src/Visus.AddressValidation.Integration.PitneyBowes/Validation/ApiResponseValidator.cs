namespace Visus.AddressValidation.Integration.PitneyBowes.Validation;

using System.Diagnostics;
using AddressValidation.Validation;
using Http;

internal sealed class ApiResponseValidator : AbstractApiResponseValidator<ApiResponse>
{
	protected override ValueTask<bool> PreValidateAsync(ApiResponse instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
	{
		if ( instance.ErrorResponse is null )
		{
			return ValueTask.FromResult(instance.Result is not null);
		}

		foreach ( ApiErrorResponse.Error error in instance.ErrorResponse.Errors )
		{
			if ( !string.IsNullOrWhiteSpace(error.ErrorDescription) )
			{
				results.Add(string.IsNullOrWhiteSpace(error.ErrorCode)
								? ValidationState.CreateError(error.ErrorDescription)
								: ValidationState.CreateError("{0}: {1}", error.ErrorCode, error.ErrorDescription));
			}

			if ( !string.IsNullOrWhiteSpace(error.AdditionalInfo) )
			{
				results.Add(ValidationState.CreateError(error.AdditionalInfo));
			}
		}

		return ValueTask.FromResult(false);
	}

	protected override ValueTask ValidateAsync(ApiResponse instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
	{
		Debug.Assert(instance.Result != null, "instance.Result != null");

		if ( string.IsNullOrWhiteSpace(instance.Result.PostalCode) )
		{
			results.Add(ValidationState.CreateWarning("Value is null or empty.", nameof(instance.Result.PostalCode)));
		}

		return ValueTask.CompletedTask;
	}
}
