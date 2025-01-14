﻿using ConsoleMan;

namespace PgUtil;

internal partial class Program
{
    private sealed partial class LicensesCommand : IConsoleCommandContainer
    {
        public static string Name => "licenses";
        public static string Description => "Manage license definitions and audit package licenses";

        public static void Configure(ICommandBuilder builder)
        {
            builder.WithOption<SourceOption>()
                .WithOption<ApiKeyOption>()
                .WithOption<UserNameOption>()
                .WithOption<PasswordOption>()
                .WithCommand<ListCommand>()
                .WithCommand<InfoCommand>()
                .WithCommand<CreateCommand>()
                .WithCommand<DeleteCommand>()
                .WithCommand<DetectionCommand>()
                .WithCommand<FilesCommand>();
        }

        private sealed class CodeOption : IConsoleOption
        {
            public static bool Required => true;
            public static string Name => "--code";
            public static string Description => "Unique ID of the license";
        }
    }
}
