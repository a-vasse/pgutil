﻿using ConsoleMan;

namespace PgUtil;

internal partial class Program
{
    private sealed partial class BuildsCommand
    {
        private sealed partial class IssuesCommand
        {
            private sealed class ResolveCommand : IConsoleCommand
            {
                public static string Name => "resolve";
                public static string Description => "Resolves an open issue";
                public static string Examples => """
                      >$ pgutil builds issues resolve --project=newApplication --build=1.0.1 --number=2

                    For more information, see: https://docs.inedo.com/docs/proget/reference-api/proget-api-sca/issues/proget-api-sca-issues-resolve
                    """;

                public static void Configure(ICommandBuilder builder)
                {
                    builder.WithOption<NumberOption>();
                }

                public static async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
                {
                    var client = context.GetProGetClient();
                    await client.ResolveIssueAsync(context.GetOption<ProjectOption>(), context.GetOption<BuildOption>(), context.GetOption<NumberOption, int>(), cancellationToken);
                    Console.WriteLine("Issue resolved.");

                    return 0;
                }
            }
        }
    }
}
