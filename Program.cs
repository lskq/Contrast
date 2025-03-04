using System.CommandLine;

namespace Contrast;

class Program
{
    /// <summary>
    /// A CLI interface for calculating the contrast ratio between two colors.
    /// For details, see https://www.w3.org/TR/UNDERSTANDING-WCAG20/visual-audio-contrast-contrast.html#contrast-ratiodef
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static async Task Main(string[] args)
    {
        var firstArgument = new Argument<string>
            (name: "L1",
            description: "A hex or comma-separated RGB color code." +
                         "\nE.g.: FF0080\n      255,0,128");

        var secondArgument = new Argument<string>
            (name: "L2",
            description: "See L1.");

        var rootCommand = new RootCommand("A CLI app for calculating the contrast ratio between two colors. Supports hex and comma-separated RGB. For details, see: " +
                                          "https://www.w3.org/TR/UNDERSTANDING-WCAG20/visual-audio-contrast-contrast.html#contrast-ratiodef");
        rootCommand.Add(firstArgument);
        rootCommand.Add(secondArgument);

        rootCommand.SetHandler((firstArgumentValue, secondArgumentValue) =>
        {
            try
            {
                Console.WriteLine(ContrastRatio(firstArgumentValue, secondArgumentValue));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        },
        firstArgument, secondArgument);

        await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Calculates the contrast ratio between two colors in hex or RGB string form.
    /// </summary>
    /// <param name="l1"></param>
    /// <param name="l2"></param>
    /// <returns>A double between 1 (for the ratio of two colors with equal luminance) and 21 (for the ratio between pure black and pure white).</returns>
    static double ContrastRatio(string l1, string l2)
    {
        int[] l1Array = ParseColor(l1);
        double l1Luminance = RelativeLuminance(l1Array[0], l1Array[1], l1Array[2]);

        int[] l2Array = ParseColor(l2);
        double l2Luminance = RelativeLuminance(l2Array[0], l2Array[1], l2Array[2]);

        return ContrastRatio(l1Luminance, l2Luminance);
    }

    /// <summary>
    /// Calculates the contrast ratio between the relative luminances of two colors.
    /// </summary>
    /// <param name="l1"></param>
    /// <param name="l2"></param>
    /// <returns>A double between 1 (for two equal luminances) and 21 (for the luminances of pure black and pure white).</returns>
    static double ContrastRatio(double l1, double l2)
    {
        l1 += 0.05;
        l2 += 0.05;

        return l1 > l2 ? l1 / l2 : l2 / l1;
    }

    /// <summary>
    /// Calculates the relative luminance of an RGB color code.
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <returns>A double between 0 (for pure black) and 1 (for pure white).</returns>
    static double RelativeLuminance(int r, int g, int b)
    {
        double rr = PartialLuminance(r / 255.0);
        double gg = PartialLuminance(g / 255.0);
        double bb = PartialLuminance(b / 255.0);

        return 0.2126 * rr + 0.7152 * gg + 0.0722 * bb;
    }

    /// <summary>
    /// Calculates the relative luminance of a single color channel.
    /// </summary>
    /// <param name="color"></param>
    /// <returns>A double</returns>
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

    /// <summary>
    /// Converts the string representation of a hex or RGB color code to an int array containing its equivalent RGB values.              
    /// </summary>
    /// <param name="color"></param>
    /// <returns>An int array containing three values, each between 0 and 255.</returns>
    /// <exception cref="FormatException"></exception>
    static int[] ParseColor(string color)
    {
        string originalColor = color;

        if (color.StartsWith('#')) // Might never trigger?
        {
            color = color[1..];
        }
        else if (color.StartsWith("0x"))
        {
            color = color[2..];
        }

        if (color.Split(",").Length == 3)
        {
            return ParseRGBString(color);
        }
        else if (color.Length == 6)
        {
            return ParseHexString(color);
        }
        else
        {
            throw new FormatException($"FormatException: {originalColor}. Not a valid hex code or comma-separated RGB value.");
        }
    }

    /// <summary>
    /// Converts the comma-separated string representation of an RGB color code to an equivalent int array.
    /// </summary>
    /// <param name="rgb"></param>
    /// <returns>An int array containing three values, each between 0 and 255.</returns>
    /// <exception cref="FormatException"></exception>
    static int[] ParseRGBString(string rgb)
    {
        string[] rgbStringArray = rgb.Split(",");
        int[] rgbIntArray = [0, 0, 0];

        for (int i = 0; i < 3; i++)
        {
            if (!int.TryParse(rgbStringArray[i], out rgbIntArray[i]) || 0 > rgbIntArray[i] || rgbIntArray[i] > 255)
                throw new FormatException($"FormatException: {rgb}. Not a valid RGB value.");
        }

        return rgbIntArray;
    }

    /// <summary>
    /// Converts the string representation of a hex color code to an int array containing its RGB equivalent.
    /// </summary>
    /// <param name="hex"></param>
    /// <returns>An int array containing three values, each between 0 and 255.</returns>
    /// <exception cref="FormatException"></exception>
    static int[] ParseHexString(string hex)
    {
        int[] hexIntArray = [0, 0, 0];

        for (int i = 0; i < 5; i += 2)
        {
            string hexString = hex.Substring(i, 2);

            try
            {
                hexIntArray[i / 2] = Convert.ToInt32(hexString, 16);
            }
            catch (FormatException)
            {
                throw new FormatException($"FormatException: {hex}. Not a valid hex code.");
            }
        }

        return hexIntArray;
    }
}