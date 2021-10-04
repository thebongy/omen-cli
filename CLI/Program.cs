using CommandLine;

namespace LightingControl
{
    [Verb("set4", HelpText = "Set colors for four zones of the keyboard")]
    class SetFourOptions {
        [Value(0, HelpText = "First Color")] public string color1 { get; set; }
        [Value(1, HelpText = "Second Color")] public string color2 { get; set; }
        [Value(2, HelpText = "Third Color")] public string color3 { get; set; }
        [Value(3, HelpText = "Fourth Color")] public string color4 { get; set; }
        [Option(HelpText = "Specifies the color arg format (either name (default) or rgb)", Default = "name")] public string type { get; set;  }
    }

    [Verb("set1", HelpText = "Set the same color to all 4 zones of the keyboard")]
    class SetOneOptions
    {
        [Value(0, HelpText = "The Color")] public string color { get; set; }
        [Option(HelpText = "type", Default = "Specifies the color arg format (either name (default) or rgb)")] public string type { get; set;  }

    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<SetFourOptions, SetOneOptions>(args)
                .WithParsed<SetFourOptions>(options =>
                    CommandHandlers.SetFourCommand(
                        Utils.ParseColor(options.type, options.color1),
                        Utils.ParseColor(options.type, options.color2),
                        Utils.ParseColor(options.type, options.color3),
                        Utils.ParseColor(options.type, options.color4)
                    ))
                .WithParsed<SetOneOptions>(options =>
                    CommandHandlers.SetOneCommand(
                        Utils.ParseColor(options.type, options.color)
                        ));
        }
    }
}