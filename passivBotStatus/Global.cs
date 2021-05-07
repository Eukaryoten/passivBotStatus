using Microsoft.Extensions.Configuration;
using System;

namespace passivBotCheck {
    public static class Global {

        private static string statusFile = "passiv_bot_status.txt";
        private static string hostTimeoutFile = "host_timeout.txt";

        public static string HostTimeoutFile(this Config config) {
            return $"{config.ExternalSharedFolder}{hostTimeoutFile}";
        }

        public static string HostStatusFile(this Config config) {
            return $"{config.ExternalSharedFolder}{statusFile}";
        }

        public static string LocalStatusFile(this Config config) {
            return $"{config.LocalSharedFolder}{statusFile}";
        }

        public static string LocalTimeoutFile(this Config config) {
            return $"{config.LocalSharedFolder}{hostTimeoutFile}";
        }


        public static T InitOptions<T>() where T : new() {
            var config = InitConfig();
            return config.Get<T>();
        }

        public static IConfigurationRoot InitConfig() {

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"config.json", true, true)
                .AddJsonFile($"config.{env}.json", true, true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

    }
}
