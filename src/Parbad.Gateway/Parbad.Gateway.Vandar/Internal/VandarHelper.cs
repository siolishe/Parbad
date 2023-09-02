using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Parbad.Abstraction;
using Parbad.Http;
using Parbad.Internal;
using Parbad.Options;
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
            api_key = account.ApiKey,
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
            return PaymentRequestResult.Failed(response, account.Name);
        }

        var result = JsonConvert.DeserializeObject<VandarCallbackResultModel>(response);

        var paymentPageUrl = string.Concat(gatewayOptions.PaymentPageUrl, result?.Token);

        return PaymentRequestResult.SucceedWithRedirect(account.Name, httpContext, paymentPageUrl);
    }

    public static async Task<VandarCallbackResultModel> CreateCallbackResultAsync(
        InvoiceContext context,
        HttpRequest request,
        VandarGatewayAccount account,
        MessagesOptions optionsMessages,
        CancellationToken cancellationToken)
    {
        var paymentToken = await request.TryGetParamAsAsync<string>("paymentToken", cancellationToken);

        if (paymentToken.Value != context.Payment.Token.ToString())
        {
            return new VandarCallbackResultModel
            {
                IsSucceed = false,
                Message = optionsMessages.InvalidDataReceivedFromGateway
            };
        }

        var status = await request.TryGetParamAsAsync<bool>("payment_status", cancellationToken);
        var token = request.TryGetParamAsAsync<string>("token", cancellationToken).Result;
        return new VandarCallbackResultModel
        {
            IsSucceed = status.Value,
            Token = token.Value
        };
    }

    private static string VandarResultCodeDescriptor(int code)
    {
        return code switch
        {
            0 => "اطلاعات تراکنش اشتباه است",
            1 => "تراکنش منتظر تایید است",
            2 => "تراکنش از قبل تایید شده است",
            3 => "تراکنش منقضی شده است(مدت زمان انقضای تراکنش از زمان انتقال به صفحه پرداخت تا تایید 20 دقیقه است)",
            4 => "تراکنش ناموفق بوده و نتیجه از قبل اعلام شده است",
            _ => string.Empty
        };
    }

    public static VandarFetchDataModel CreateVerifyData(VandarCallbackResultModel callbackResult,
        VandarGatewayAccount account)
    {
        return new VandarFetchDataModel
        {
            api_key = account.ApiKey,
            token = callbackResult.Token
        };
    }

    public static IPaymentVerifyResult CreateVerifyResult(VandarVerifyResultModel response, InvoiceContext context,
        VandarCallbackResultModel callbackResult, MessagesOptions optionsMessages)
    {
        return new PaymentVerifyResult
        {
            IsSucceed = response.Status,
            Message = response.Message,
            Amount = response.Amount,
            Status = response.Status ? PaymentVerifyResultStatus.Succeed : PaymentVerifyResultStatus.Failed,
            TrackingNumber = long.Parse(response.FactorNumber),
            AdditionalData = { },
            GatewayName = "Vandar"
        };
    }

    public static VandarFetchDataModel CreateFetchData(VandarCallbackResultModel callbackResult,
        VandarGatewayAccount account)
    {
        return new VandarFetchDataModel
        {
            api_key = account.ApiKey,
            token = callbackResult.Token
        };
    }

    public static bool FetchDataDescriptor(InvoiceContext context, VandarFetchDataResultModel responseMessage)
    {
        return !context.Payment.IsPaid && responseMessage.Status;
    }
}