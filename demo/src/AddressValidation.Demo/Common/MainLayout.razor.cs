namespace AddressValidation.Demo.Common;

using Services.Abstractions;

public partial class MainLayout : LayoutComponentBase
{
	private bool _isExpanded = true;

	[Inject]
	protected IGlyphService GlyphService { get; set; } = default!;
}
