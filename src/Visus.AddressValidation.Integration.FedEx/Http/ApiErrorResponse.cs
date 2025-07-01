namespace Visus.AddressValidation.Integration.FedEx.Http;

internal sealed class ApiErrorResponse
{
    public string? CustomerTransactionId { get; set; }

    public Error[] Errors { get; set; } = [];

    public Guid TransactionId { get; set; }

    internal sealed class Error
    {
        public string Code { get; set; } = null!;

        public string Message { get; set; } = null!;

        public Parameter[] ParameterList { get; set; } = [];
    }

    internal sealed class Parameter
    {
        public string Key { get; set; } = null!;

        public string Value { get; set; } = null!;
    }
}
