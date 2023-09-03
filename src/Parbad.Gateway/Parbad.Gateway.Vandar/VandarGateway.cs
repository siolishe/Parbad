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
using Parbad.Options;

namespace Parbad.Gateway.Vandar;

public class VandarGateway : GatewayBase<VandarGatewayAccount>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;
    private readonly VandarGatewayOptions _gatewayOptions;
    private readonly ParbadOptions _options;

    public const string Name = "Vandar";

    public VandarGateway(IGatewayAccountProvider<VandarGatewayAccount> accountProvider,
        IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory,
        IOptions<VandarGatewayOptions> gatewayOptions, IOptions<ParbadOptions> options) : base(accountProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
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
        if (context == null) throw new ArgumentNullException(nameof(context));

        var account = await GetAccountAsync(context.Payment).ConfigureAwaitFalse();

        var callbackResult = await VandarHelper.CreateCallbackResultAsync(
            context,
            _httpContextAccessor.HttpContext?.Request,
            account,
            _options.Messages,
            cancellationToken);

        var data = VandarHelper.CreateFetchData(callbackResult, account);
        var responseMessage =
            await _httpClient.PostJsonAsync<VandarFetchDataResultModel>(_gatewayOptions.ApiCheckPaymentUrl, data,
                cancellationToken);
        var isValid = VandarHelper.FetchDataDescriptor(context, responseMessage);

        return isValid
            ? PaymentFetchResult.ReadyForVerifying()
            : PaymentFetchResult.Failed(callbackResult.Message);
    }

    /// <inheritdoc />
    public override async Task<IPaymentVerifyResult> VerifyAsync(InvoiceContext context,
        CancellationToken cancellationToken = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var callbackResult = await VandarHelper.CreateCallbackResultAsync(
                context,
                _httpContextAccessor.HttpContext?.Request,
                null,
                _options.Messages,
                cancellationToken)
            .ConfigureAwaitFalse();

        if (!callbackResult.IsSucceed)
        {
            return PaymentVerifyResult.Failed(callbackResult.Message);
        }

        var account = await GetAccountAsync(context.Payment).ConfigureAwaitFalse();

        var data = VandarHelper.CreateVerifyData(callbackResult, account);
        var jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = {new StringEnumConverter()}
        };

        var response = await _httpClient.PostJsonAsync<VandarVerifyResultModel>(
            _gatewayOptions.ApiVerificationUrl, data, cancellationToken);
        

        return VandarHelper.CreateVerifyResult(response, context, callbackResult, _options.Messages);
    }

    /// <inheritdoc />
    public override async Task<IPaymentRefundResult> RefundAsync(InvoiceContext context, Money amount,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}