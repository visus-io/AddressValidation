namespace Visus.AddressValidation.Integration.Ups.Validation;

using AddressValidation.Validation;
using Contracts;

internal sealed class ApiResponseValidator : AbstractValidator<ApiResponse>
{
    protected override ValueTask<bool> PreValidateAsync(ApiResponse instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        if ( instance.ErrorResponse is null )
        {
            return ValueTask.FromResult(instance.Result is not null);
        }

        foreach ( ApiErrorResponse.Error error in instance.ErrorResponse.Response.Errors )
        {
            if ( string.IsNullOrWhiteSpace(error.Message) )
            {
                continue;
            }

            results.Add(string.IsNullOrWhiteSpace(error.Code)
                            ? ValidationState.CreateError(error.Message)
                            : ValidationState.CreateError($"{error.Code}: {error.Message}"));
        }

        return ValueTask.FromResult(false);
    }
}
