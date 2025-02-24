// Calculates the contrast ratio between two colours.
// For details, see https://www.w3.org/TR/UNDERSTANDING-WCAG20/visual-audio-contrast-contrast.html#contrast-ratiodef

double ContrastRatio(double l1, double l2)
{
    l1 += 0.05;
    l2 += 0.05;

    return l1 > l2 ? l1 / l2 : l2 / l1;
}

double RelativeLuminance(int r, int g, int b)
{
    double rr = PartialLuminance(r / 255.0);
    double gg = PartialLuminance(g / 255.0);
    double bb = PartialLuminance(b / 255.0);

    return 0.2126 * rr + 0.7152 * gg + 0.0722 * bb;
}

double PartialLuminance(double color)
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