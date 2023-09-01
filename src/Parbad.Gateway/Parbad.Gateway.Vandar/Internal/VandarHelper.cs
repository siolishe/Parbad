using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Parbad.Abstraction;
using Parbad.Internal;
using Parbad.Utilities;

namespace Parbad.Gateway.Vandar.Internal;

internal static class VandarHelper
{
    public static VandarRequestModel CreateRequestModel(Invoice invoice, VandarGatewayAccount account)
    {
        return new VandarRequestModel
        {
            amount = (int) invoice.Amount,
            Description = "",
            Port = "",
            api_key= account.ApiKey,
            callback_url = invoice.CallbackUrl,
            FactorNumber = invoice.TrackingNumber.ToString(),
            Mobile_Number = "",
            National_Code = "",
            Valid_Card_Number = new List<string>()
        };
    }

    public static async Task<IPaymentRequestResult> CreateRequestResult(HttpResponseMessage responseMessage,
        HttpContext httpContext, VandarGatewayAccount account, VandarGatewayOptions gatewayOptions)
    {
        var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwaitFalse();
        if (!responseMessage.IsSuccessStatusCode)
        {
            return PaymentRequestResult.Failed(response.ToString(), account.Name);
        }

        var result = JsonConvert.DeserializeObject<VandarRequestResultModel>(response);

        var paymentPageUrl = QueryHelper.AddQueryString(gatewayOptions.PaymentPageUrl, new Dictionary<string, string>
        {
            {"token", result.Token}
        });

        return PaymentRequestResult.SucceedWithRedirect(account.Name, httpContext, paymentPageUrl);
    }
}