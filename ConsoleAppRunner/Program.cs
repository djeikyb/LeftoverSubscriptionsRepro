using System.Runtime.InteropServices;
using static Chell.Exports;

var cts = new CancellationTokenSource();
var ct = cts.Token;

Console.WriteLine("adding sigint");
using var sigint = PosixSignalRegistration.Create(PosixSignal.SIGINT, context =>
{
    Console.WriteLine($"caught {context.Signal}");
    context.Cancel = true;
    cts.Cancel();
    Console.WriteLine("Requested cancel.");
});

int i = 1;
try
{
    for (; !ct.IsCancellationRequested; i++)
    {
        await Run($"{Arguments[0]} {Arguments[1]}");
    }
}
finally
{
    Console.WriteLine($"Ran {i} times.");
}
