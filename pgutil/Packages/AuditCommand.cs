﻿using ConsoleMan;

namespace PgUtil;

internal partial class Program
{
    private sealed partial class PackagesCommand
    {
        private sealed class AuditCommand : IConsoleCommand
        {
            public static string Name => "audit";
            public static string Description => "Analyze a package for compliance issues";

            public static void Configure(ICommandBuilder builder)
            {
                builder.WithOption<PackageNameOption>()
                    .WithOption<PackageVersionOption>();
            }

            public static async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
            {
                var client = context.GetProGetClient();
                var (p, fullName) = GetPackageIdentifier(context);

                CM.WriteLine("Checking ", new TextSpan($"{fullName} {p.Version}", ConsoleColor.White), "...");
                var result = await client.AuditPackageAsync(p, cancellationToken);
                if (result.ResultCode is not null)
                {
                    CM.WriteError(result.StatusText);
                    return -1;
                }

                CM.WriteLine("Analzyed: ", new TextSpan(result.AnalysisDate!.Value.ToLocalTime().ToString()));
                CM.WriteLine(
                    "Status: ",
                    new TextSpan(
                        result.StatusText,
                        result.ResultCode switch
                        {
                            "C" => ConsoleColor.Green,
                            "W" => ConsoleColor.DarkYellow,
                            "I" => ConsoleColor.Yellow,
                            _ => ConsoleColor.Red
                        }
                    )
                );

                if (!string.IsNullOrWhiteSpace(result.Detail))
                    Console.WriteLine(result.Detail);

                return result.ResultCode is "C" or "W" ? 0 : -1;
            }
        }
    }
}