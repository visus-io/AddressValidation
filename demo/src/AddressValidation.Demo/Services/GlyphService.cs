namespace AddressValidation.Demo.Services;

using Abstractions;

public sealed class GlyphService : IGlyphService
{
	public string RenderFontAwesomeGlyph(params string[] classNames)
	{
		return $"<i class=\"{string.Join(" ", classNames)}\"></i>";
	}
}
