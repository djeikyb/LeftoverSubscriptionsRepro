using System.Security.Cryptography;
using R3;

namespace ConsoleAppTestSubject;

class Program
{
    static async Task<int> Main(string[] args)
    {
        ObservableTracker.EnableTracking = true;
        ObservableTracker.EnableStackTrace = true;

        using (var container = new Container())
        {
            container.AbsolutePath.Value = args[0];

            // prove the gc isn't clearing out the tracker
            GC.Collect();
            ObservableTracker.ForEachActiveTask(x => Console.WriteLine($"{x.TrackingId}: {x.FormattedType}"));
            // [/prove]

            await Task.Delay(3_000);
        }

        GC.Collect();
        bool any = false;
        ObservableTracker.ForEachActiveTask(x =>
        {
            any = true;
            Console.WriteLine(x.ToString());
        });

        if (any)
        {
            Console.WriteLine("🚨 fail");
            return -1;
        }

        return 0;
    }
}

class Container : IDisposable
{
    private DisposableBag _disposable;

    public Container()
    {
        AbsolutePath = new ReactiveProperty<string?>().AddTo(ref _disposable);
        Hash = new ReactiveProperty<string?>().AddTo(ref _disposable);

        AbsolutePath.SelectAwait(async (p, ct) =>
            {
                if (p is null) return null;
                using var stream = File.Open(p, FileMode.Open, FileAccess.Read, FileShare.Read);
                var bytes = await SHA1.HashDataAsync(stream, ct);
                return Convert.ToHexString(bytes).ToLowerInvariant();
            }, AwaitOperation.Switch)
            .Subscribe(h => { Hash.Value = h; }).AddTo(ref _disposable);

        Observable.Merge(AbsolutePath.AsUnitObservable(), Hash.AsUnitObservable())
            .Debounce(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
                Console.WriteLine($"""
                                   ---
                                   Path: {AbsolutePath.Value}
                                   Hash: {Hash.Value}
                                   """);
            }).AddTo(ref _disposable);
    }

    public ReactiveProperty<string?> AbsolutePath { get; }
    public ReactiveProperty<string?> Hash { get; }

    public void Dispose()
    {
        _disposable.Dispose();
    }
}
