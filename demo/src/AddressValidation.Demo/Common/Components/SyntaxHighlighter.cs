namespace AddressValidation.Demo.Common.Components;

using Microsoft.AspNetCore.Components.Rendering;

public sealed class SyntaxHighlighter : ComponentBase
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public string Language { get; set; } = default!;

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		builder.OpenElement(0, "pre");
		builder.OpenElement(1, "code");

		builder.AddAttribute(2, "class", $"language-{Language}");
		builder.AddContent(3, ChildContent);

		builder.CloseElement();
		builder.CloseElement();
	}
}
