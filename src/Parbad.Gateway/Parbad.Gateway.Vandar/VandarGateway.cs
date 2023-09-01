using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Parbad.Abstraction;
using Parbad.Gateway.Vandar.Internal;
using Parbad.GatewayBuilders;
using Parbad.Internal;
using Parbad.Net;

namespace Parbad.Gateway.Vandar;

public class VandarGateway : GatewayBase<VandarGatewayAccount>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;
    private readonly VandarGatewayOptions _gatewayOptions;
    public const string Name = "Vandar";

    public VandarGateway(IGatewayAccountProvider<VandarGatewayAccount> accountProvider,
        IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory,
        IOptions<VandarGatewayOptions> gatewayOptions) : base(accountProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _gatewayOptions = gatewayOptions.Value;
        _httpClient = httpClientFactory.CreateClient(nameof(VandarGateway));
    }

    /// <inheritdoc />
    public override async Task<IPaymentRequestResult> RequestAsync(Invoice invoice,
        CancellationToken cancellationToken = default)
    {
        if (invoice == null) throw new ArgumentNullException(nameof(invoice));

        var account = await GetAccountAsync(invoice).ConfigureAwaitFalse();

        var jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = {new StringEnumConverter()}
        };
        var data = VandarHelper.CreateRequestModel(invoice, account);
        var responseMessage = await _httpClient.PostJsonAsync(_gatewayOptions.ApiTokenGenerationUrl, data, jsonSettings,
            cancellationToken);

        return await VandarHelper.CreateRequestResult(responseMessage, _httpContextAccessor.HttpContext, account,
            _gatewayOptions);
    }

    /// <inheritdoc />
    public override async Task<IPaymentFetchResult> FetchAsync(InvoiceContext context,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override async Task<IPaymentVerifyResult> VerifyAsync(InvoiceContext context,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override async Task<IPaymentRefundResult> RefundAsync(InvoiceContext context, Money amount,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}