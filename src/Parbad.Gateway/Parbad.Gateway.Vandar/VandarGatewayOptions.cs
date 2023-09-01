// Copyright (c) Parbad. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC License, Version 3.0. See License.txt in the project root for license information.

namespace Parbad.Gateway.Vandar
{
    public class VandarGatewayOptions
    {
        public string ApiTokenGenerationUrl { get; set; } = "https://ipg.vandar.io/api/v3/send";

        public string PaymentPageUrl { get; set; } = "https://ipg.vandar.io/v3/";

        public string ApiCheckPaymentUrl { get; set; } =
            "https://fcp.shaparak.ir/ref-payment/RestServices/mts/inquiryMerchantToken";

        public string ApiVerificationUrl { get; set; } =
            "https://fcp.shaparak.ir/ref-payment/RestServices/mts/verifyMerchantTrans";

        public string ApiRefundUrl { get; set; } =
            "https://fcp.shaparak.ir/ref-payment/RestServices/mts/reverseMerchantTrans";

        public string ApiKey { get; set; }
    }
}