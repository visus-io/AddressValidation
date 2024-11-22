namespace Visus.AddressValidation.Validation;

using Extensions;
using Http;

/// <summary>
///     Base Validator for <see cref="AbstractAddressValidationRequest" /> instances.
/// </summary>
public abstract class AbstractAddressValidationRequestValidator<T> : IValidator<T>, IDisposable
	where T : AbstractAddressValidationRequest
{
	private readonly SemaphoreSlim _semaphore = new(1, 1);
	private bool _isDisposed;

	/// <inheritdoc />
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <inheritdoc />
	public ValueTask<IValidationResult> ExecuteAsync(T instance, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(instance);
		return ExecuteInternalAsync(new ValidationContext<T>(instance), cancellationToken);
	}

	/// <summary>
	///     Performs application-defined tasks associated with freeing, release or resetting managed and unmanaged resources.
	/// </summary>
	/// <param name="disposing"><c>true</c> if managed resources should be disposed; otherwise, <c>false</c>.</param>
	protected virtual void Dispose(bool disposing)
	{
		if ( _isDisposed )
		{
			return;
		}

		if ( disposing )
		{
			_semaphore.Dispose();
		}

		_isDisposed = true;
	}

	/// <summary>
	///     Determines if validation should continue as well as providing a means to modify the instance and validation state
	///     prior to execution.
	/// </summary>
	/// <param name="instance">The object to perform validation against.</param>
	/// <param name="results">The set (collection) of <see cref="ValidationState" /> objects for the current instance.</param>
	/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
	/// <returns><c>true</c> to continue with validation; otherwise, <c>false</c>.</returns>
	protected virtual ValueTask<bool> PreValidateAsync(T instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
	{
		return ValueTask.FromResult(true);
	}

	/// <summary>
	///     Validates the specified instance.
	/// </summary>
	/// <param name="instance">The object to perform validation against.</param>
	/// <param name="results">The set (collection) of <see cref="ValidationState" /> objects for the current instance.</param>
	/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
	/// <returns>A task that represents the asynchronous validation operation.</returns>
	protected virtual ValueTask ValidateAsync(T instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
	{
		return ValueTask.CompletedTask;
	}

	private static ValueTask<bool> PreValidateInternalAsync(T instance, HashSet<ValidationState> results)
	{
		if ( instance.Country is not null )
		{
			return ValueTask.FromResult(true);
		}

		results.Add(ValidationState.CreateError("Value cannot be null.", nameof(instance.Country)));

		return ValueTask.FromResult(false);
	}

	private static ValueTask ValidateInternalAsync(T instance, HashSet<ValidationState> results)
	{
		switch ( instance.AddressLines.Count )
		{
			case 0:
			case > 0 when instance.AddressLines.All(string.IsNullOrWhiteSpace):
				results.Add(ValidationState.CreateError("Value cannot be null or empty.", nameof(instance.AddressLines)));
				break;
		}

		if ( !Constants.CityStates.Contains(instance.Country!.Value) )
		{
			if ( string.IsNullOrWhiteSpace(instance.CityOrTown) )
			{
				results.Add(ValidationState.CreateError("Value cannot be null or empty.", nameof(instance.CityOrTown)));
			}

			if ( string.IsNullOrWhiteSpace(instance.StateOrProvince) )
			{
				results.Add(ValidationState.CreateError("Value cannot be null or empty.", nameof(instance.StateOrProvince)));
			}
		}

		if ( !Constants.NoPostalCode.Contains(instance.Country!.Value) && string.IsNullOrWhiteSpace(instance.PostalCode) )
		{
			results.Add(ValidationState.CreateError("Value cannot be null or empty.", nameof(instance.PostalCode)));
		}

		if ( Constants.NoPostalCode.Contains(instance.Country!.Value) )
		{
			results.Add(ValidationState.CreateError("{0}: {1} is currently not supported for address validation.", nameof(instance.Country), instance.Country));
		}

		return ValueTask.CompletedTask;
	}

	private async ValueTask<IValidationResult> ExecuteInternalAsync(ValidationContext<T> context, CancellationToken cancellationToken)
	{
		using ( await _semaphore.LockAsync(cancellationToken).ConfigureAwait(false) )
		{
			if ( !await PreValidateInternalAsync(context.Instance, context.ValidationResults).ConfigureAwait(false) )
			{
				return new ValidationResult(context.ValidationResults);
			}

			if ( !await PreValidateAsync(context.Instance, context.ValidationResults, cancellationToken).ConfigureAwait(false) )
			{
				return new ValidationResult(context.ValidationResults);
			}

			await ValidateInternalAsync(context.Instance, context.ValidationResults).ConfigureAwait(false);
			await ValidateAsync(context.Instance, context.ValidationResults, cancellationToken).ConfigureAwait(false);
		}

		return new ValidationResult(context.ValidationResults);
	}
}
