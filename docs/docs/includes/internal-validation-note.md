> [!NOTE]
> AddressValidation runs an internal validation engine under the covers to
> validate both the request and the response. You will find these results in the `Warnings` and `Errors` collections from the [`IAddressValidationResponse`](xref:Visus.AddressValidation.Model.IAddressValidationResponse) object.
> 
> Items within the `Suggestion` collection **are not** processed by the internal validator.
