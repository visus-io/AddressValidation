namespace Visus.AddressValidation.Http.Authentication;

using System.Collections.Specialized;
using System.Web;
using Microsoft.Extensions.Configuration;

public sealed class QueryStringDelegatingHandler : DelegatingHandler
{
	private readonly IConfiguration _configuration;

	private readonly string _configurationKey;

	private readonly string _queryKey;

	public QueryStringDelegatingHandler(IConfiguration configuration, string configurationKey, string queryKey)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(configurationKey);
		ArgumentException.ThrowIfNullOrWhiteSpace(queryKey);

		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		_configurationKey = configurationKey;
		_queryKey = queryKey;
	}

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		return SendInternalAsync(request, cancellationToken);
	}

	private async Task<HttpResponseMessage> SendInternalAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if ( request.RequestUri is null )
		{
			return await base.SendAsync(request, cancellationToken);
		}

		string? value = _configuration[_configurationKey];
		if ( string.IsNullOrWhiteSpace(value) )
		{
			throw new KeyNotFoundException(_configurationKey);
		}

		UriBuilder uriBuilder = new(request.RequestUri);
		NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

		query.Add(_queryKey, value);
		uriBuilder.Query = query.ToString();

		request.RequestUri = new Uri(uriBuilder.ToString());

		return await base.SendAsync(request, cancellationToken);
	}
}
