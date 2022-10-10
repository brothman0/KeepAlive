using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using KeepAlive;
using KeepAlive.External;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

//TODO: create a way to parse an input math y = x... expression

var (xStart, yStart) = (0, 0);
var myWidth = 4;
//x^4 = a^2(x^2 - y^2)
//-(x sqrt(a^2 - x^2))/a
//(x^{2}+y^{2})^{2}=2a^{2}(x^{2}-y^{2}) y = -sqrt(-sqrt(a^2 (a^2 + 4 x^2)) - a^2 - x^2)

var myRadius = GetRadius(myWidth);
Console.WriteLine(myRadius);
DoSomething(myWidth, myRadius);


using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (_, services) =>
            services
                .AddHostedService<KeepAliveService>()
                .AddSingleton<ICommonAdapter, CommonAdapter>()
                .AddSingleton<ICommonAgent, CommonAgent>()
                .AddTransient<IExternalAdapter, ExternalAdapter>()
                .AddTransient<IExternalAgent, ExternalAgent>())
    .Build();
await host.RunAsync();

double GetRadius(double width)
{
    return Math.Pow(width, 3D / 2D) / (2D * Math.Sqrt(2));
}

void DoSomething(double width, double radius)
{
    var x = 0D;
    foreach (var quadrant in Enum.GetValues<Quadrant>())
    {
        var (continueCondition, increment, yModifier) = GetQuadrantComponents(quadrant, width);
        for (; continueCondition(x); x += increment)
        {
            Console.WriteLine($"X:{x} | Y:{GetY(x, radius, yModifier)}");
        }
    }
}

void DrawQuadrant(double width, double radius)
{

}

decimal GetPoint(int inputX, int inputRadius)
{
    var x = (double)inputX;
    var radius = (double)inputRadius;
    return default;
}

double GetY(double x, double radius, double modifier)
{
    //y = ± sqrt(-a^2 + sqrt(a^4 + 4 a^2 x^2) - x^2)
    var y = Math.Sqrt(-Math.Pow(radius, 2) + Math.Sqrt(Math.Pow(radius, 4) + 4 * Math.Pow(radius, 2) * Math.Pow(x, 2)) -
                      Math.Pow(x, 2));
    return double.IsNaN(y) ? 0 : y * modifier;

}

(Func<double, bool> continueCondition, double increment, double yModifier) GetQuadrantComponents(
    Quadrant quadrant,
    double width)
{
    return quadrant switch
    {
        Quadrant.TopRight => (x => x < width, 1, 1),
        Quadrant.BottomRight => (x => x > 0, -1, -1),
        Quadrant.TopLeft => (x => x > -width,-1, 1),
        Quadrant.BottomLeft => (x => x < 0 ,1, -1)
    };
}

[ExcludeFromCodeCoverage(Justification = "Main does not require code coverage in this case.")]
public partial class Program
{
}



public enum Quadrant
{
    TopRight = 0,
    BottomRight = 1,
    TopLeft = 2,
    BottomLeft = 3
}

public enum VerticalOrientation
{
    Top = Quadrant.TopRight | Quadrant.TopLeft,
    Bottom = Quadrant.BottomLeft | Quadrant.BottomLeft
}

public enum HorizontalOrientation
{
    Right = Quadrant.TopRight | Quadrant.BottomRight,
    Left = Quadrant.TopLeft | Quadrant.BottomLeft
}