namespace Visus.AddressValidation.Google.Validation;

using Abstractions;
using FluentValidation;
using Http;

internal sealed class ApiAddressValidationResponseValidator : AbstractValidator<ApiAddressValidationResponse>
{
	private readonly HashSet<Granularity> _unverifiableGranularity =
	[
		Granularity.BLOCK,
		Granularity.GRANULARITY_UNSPECIFIED,
		Granularity.OTHER,
		Granularity.ROUTE
	];

	public ApiAddressValidationResponseValidator()
	{
		When(w => w is not null,
			 () =>
			 {
				 RuleFor(r => r.Result.Verdict.ValidationGranularity)
					.Must(m => !_unverifiableGranularity.Contains(m))
					.WithMessage("2"); // TODO: Message about unverified 

				 When(w => !_unverifiableGranularity.Contains(w.Result.Verdict.ValidationGranularity),
					  () =>
					  {
						  RuleFor(r => r.Result.Verdict.ValidationGranularity)
							 .Must(m => m != Granularity.PREMISE_PROXIMITY)
							 .WithSeverity(Severity.Warning)
							 .WithMessage("1"); // TODO: Message about partially verified

						  RuleForEach(r => r.Result.Address.AddressComponents)
							 .SetValidator(new AddressComponentValidator());
					  });
			 });
	}
}
