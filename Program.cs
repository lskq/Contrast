// Calculates the contrast ratio between two colours.
// For details, see https://www.w3.org/TR/UNDERSTANDING-WCAG20/visual-audio-contrast-contrast.html#contrast-ratiodef
using System.CommandLine;

class Program
{

    static async Task Main(string[] args)
    {
        var firstArgument = new Argument<string>
            (name: "L1",
            description: "A comma-separated RGB color code.\nE.g.: 255,0,128");

        var secondArgument = new Argument<string>
            (name: "L2",
            description: "See L1.");

        var rootCommand = new RootCommand("A CLI app for calculating the contrast ratio between two colours. For details, see: " +
                                          "https://www.w3.org/TR/UNDERSTANDING-WCAG20/visual-audio-contrast-contrast.html#contrast-ratiodef");
        rootCommand.Add(firstArgument);
        rootCommand.Add(secondArgument);

        rootCommand.SetHandler((firstArgumentValue, secondArgumentValue) =>
        {
            Console.WriteLine(ContrastRatio(firstArgumentValue, secondArgumentValue));
        },
        firstArgument, secondArgument);

        await rootCommand.InvokeAsync(args);
    }

    static double ContrastRatio(string l1, string l2)
    {
        int[] l1Array = parseRGBString(l1);
        double l1Luminance = RelativeLuminance(l1Array[0], l1Array[1], l1Array[2]);

        int[] l2Array = parseRGBString(l2);
        double l2Luminance = RelativeLuminance(l2Array[0], l2Array[1], l2Array[2]);

        return ContrastRatio(l1Luminance, l2Luminance);

    }

    static double ContrastRatio(double l1, double l2)
    {
        l1 += 0.05;
        l2 += 0.05;

        return l1 > l2 ? l1 / l2 : l2 / l1;
    }

    static double RelativeLuminance(int r, int g, int b)
    {
        double rr = PartialLuminance(r / 255.0);
        double gg = PartialLuminance(g / 255.0);
        double bb = PartialLuminance(b / 255.0);

        return 0.2126 * rr + 0.7152 * gg + 0.0722 * bb;
    }

    static double PartialLuminance(double color)
    {
        if (color <= 0.03928)
        {
            return color / 12.92;
        }
        else
        {
            return Math.Pow((color + 0.055) / 1.055, 2.4);
        }
    }

    static int[] parseRGBString(string rgb)
    {
        string[] rgbStringArray = rgb.Split(",");
        int[] rgbIntArray = [0, 0, 0];

        for (int i = 0; i < 3; i++)
        {
            rgbIntArray[i] = int.Parse(rgbStringArray[i]);
        }

        return rgbIntArray;
    }
}