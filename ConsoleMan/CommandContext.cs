﻿using System.Diagnostics.CodeAnalysis;

namespace ConsoleMan;

public sealed class CommandContext
{
    private readonly Dictionary<Type, ParsedOption> options;
    private readonly List<Command> commands;

    internal CommandContext(Dictionary<Type, ParsedOption> options, List<Command> commands)
    {
        this.options = options;
        this.commands = commands;
    }

    public Command Command => this.commands[^1];
    public IReadOnlyList<Command> Commands => this.commands;

    public bool TryGetOption<TOption>([MaybeNullWhen(false)] out string value) where TOption : IConsoleOption
    {
        value = null;
        if (!this.options.TryGetValue(typeof(TOption), out var o) || string.IsNullOrEmpty(o.Value))
            return false;

        value = o.Value;
        return true;
    }
    public bool TryGetOption<TOption, TValue>([MaybeNullWhen(false)] out TValue value) where TOption : IConsoleOption where TValue : IParsable<TValue>
    {
        value = default;

        if (!this.TryGetOption<TOption>(out var s))
            return false;

        if (!TValue.TryParse(s, null, out value))
            throw new Exception();

        return true;
    }
    public string? GetOptionOrDefault<TOption>() where TOption : IConsoleOption
    {
        if (this.TryGetOption<TOption>(out var value))
            return value;
        else
            return null;
    }

    public string GetOption<TOption>() where TOption : IConsoleOption
    {
        if (!this.TryGetOption<TOption>(out var value))
            throw new Exception();

        return value;
    }
    public TValue GetOption<TOption, TValue>() where TOption : IConsoleOption where TValue : IParsable<TValue>
    {
        if (!this.TryGetOption<TOption, TValue>(out var value))
            throw new Exception();

        return value;
    }

    public bool TryGetEnumValue<TOption, TEnum>(out TEnum value)
        where TOption : IConsoleEnumOption<TEnum>
        where TEnum : struct, Enum
    {
        value = default;
        if (this.TryGetOption<TOption>(out var s))
        {
            value = Enum.Parse<TEnum>(s, true);
            return true;
        }

        return false;
    }
    public TEnum GetEnumValue<TOption, TEnum>()
        where TOption : IConsoleEnumOption<TEnum>
        where TEnum : struct, Enum
    {
        if (!this.TryGetEnumValue<TOption, TEnum>(out var value))
            throw new Exception();

        return value;
    }
    public bool HasFlag<TOption>() where TOption : IConsoleFlagOption => this.options.ContainsKey(typeof(TOption));

    public void TrySetOption<TOption>(string value) where TOption : IConsoleOption
    {
        this.options.TryAdd(typeof(TOption), new ParsedOption(new Option<TOption>(), value));
    }

    public void WriteUsage()
    {
        Console.WriteLine("Description:");
        Console.Write("  ");
        WordWrapper.WriteOutput(this.Command.Description, 2);
        Console.WriteLine();

        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine($"  {string.Join(' ', this.Commands.Select(c => c.Name))} {(this.Command.Subcommands.Count > 0 ? "[command] " : string.Empty)}[options]");
        Console.WriteLine();

        Console.WriteLine("Options:");
        WriteTwoColumnList(
            [.. this.Command.GetOptionsInScope().OrderByDescending(o => o.Required).ThenBy(o => o.Name).Select(o => new KeyValuePair<string, string>(formatName(o), formatDescription(o))),
            new KeyValuePair<string, string>("  -?, --help", "Show help and usage information")]
        );

        if (this.Command.Subcommands.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Commands:");
            WriteTwoColumnList([.. this.Command.Subcommands.Select(c => new KeyValuePair<string, string>($"  {c.Name}", c.Description))]);
        }

        static string formatName(Option o)
        {
            var value = $"  {o.Name}";
            if (o.HasValue)
                value += $"=<{o.Name.AsSpan().TrimStart("-/")}>";
            if (o.Required)
                value += " (REQUIRED)";

            return value;
        }

        static string formatDescription(Option o)
        {
            var desc = o.Description;
            if (o.ValidValues != null && o.ValidValues.Length > 0)
                desc += $"{Environment.NewLine}Valid values: {string.Join(", ", o.ValidValues)}";

            return desc;
        }
    }

    private static void WriteTwoColumnList(List<KeyValuePair<string, string>> items)
    {
        int margin = items.Select(i => i.Key.Length).Max() + 2;
        foreach (var item in items)
        {
            Console.Write(item.Key);
            for (int i = item.Key.Length; i < margin; i++)
                Console.Write(' ');

            WordWrapper.WriteOutput(item.Value, margin);
            Console.WriteLine();
        }
    }
}