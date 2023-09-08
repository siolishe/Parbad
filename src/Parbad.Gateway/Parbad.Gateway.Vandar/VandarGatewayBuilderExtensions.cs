// Copyright (c) Parbad. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC License, Version 3.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Parbad.GatewayBuilders;

namespace Parbad.Gateway.Vandar
{
    public static class VandarGatewayBuilderExtensions
    {
        /// <summary>
        /// Adds the Vandar.ir gateway to Parbad services.
        /// </summary>
        /// <param name="builder"></param>
        public static IGatewayConfigurationBuilder<VandarGateway> AddVandar(this IGatewayBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder
                .AddGateway<VandarGateway>()
                .WithHttpClient(_ => { })
                .WithOptions(_ => { });
        }

        /// <summary>
        /// Configures the accounts for Vandar.io.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureAccounts">Configures the accounts.</param>
        public static IGatewayConfigurationBuilder<VandarGateway> WithAccounts(
            this IGatewayConfigurationBuilder<VandarGateway> builder,
            Action<IGatewayAccountBuilder<VandarGatewayAccount>> configureAccounts)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.WithAccounts(configureAccounts);
        }

        /// <summary>
        /// Configures the options for Vandar Gateway.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">Configuration</param>
        public static IGatewayConfigurationBuilder<VandarGateway> WithOptions(
            this IGatewayConfigurationBuilder<VandarGateway> builder,
            Action<VandarGatewayOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);

            return builder;
        }
    }
}