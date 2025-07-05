namespace AddressValidation.Demo.Common.Components;

public sealed partial class ResponseRequestJsonRenderer : ComponentBase
{
    [Parameter]
    public MarkupString? RequestJson { get; set; }

    [Parameter]
    public MarkupString? ResponseJson { get; set; }
}
