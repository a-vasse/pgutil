﻿using ConsoleMan;
using Inedo.ProGet;

namespace PgUtil;

internal partial class Program
{
    internal sealed partial class ApiKeysCommand
    {
        private sealed partial class CreateCommand : IConsoleCommandContainer
        {
            public static string Name => "create";
            public static string Description => "Creates an API key in ProGet";

            public static void Configure(ICommandBuilder builder)
            {
                builder.WithCommand<SystemCommand>()
                    .WithCommand<PersonalCommand>();
            }

            private sealed class KeyOption : IConsoleOption
            {
                public static bool Required => false;
                public static string Name => "--key";
                public static string Description => "API key to create. This will be generated by ProGet when not specified. Note that ProGet security policies may disallow use of this option.";
            }

            private sealed class NameOption : IConsoleOption
            {
                public static bool Required => false;
                public static string Name => "--name";
                public static string Description => "Friendly display name of the API key";
            }

            private sealed class DescriptionOption : IConsoleOption
            {
                public static bool Required => false;
                public static string Name => "--description";
                public static string Description => "Description of the API key";
            }

            private sealed class ExpirationOption : IConsoleOption
            {
                public static bool Required => false;
                public static string Name => "--expiration";
                public static string Description => "Expiration date of the API key";
            }

            private sealed class ShowKeyOption : IConsoleFlagOption
            {
                public static string Name => "--show-key";
                public static string Description => "Writes the newly created API key to stdout";
            }

            private sealed class LoggingOption : IConsoleEnumOption<ApiKeyBodyLogging>
            {
                public static bool Required => false;
                public static string Name => "--logging";
                public static string Description => "Request/response logging for the API key";
                public static string DefaultValue => "none";
            }
        }
    }
}
