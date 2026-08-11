> [!NOTE]
> AddressValidation runs an internal validation engine under the covers to
> validate both the request and the response. These results appear in the `Warnings` and `Errors` collections on the [`IAddressValidationResponse`](xref:Visus.AddressValidation.Models.IAddressValidationResponse) object.
> 
> The internal validator **does not** process items in the `Suggestions` collection.
