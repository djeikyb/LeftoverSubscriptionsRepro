## How did I get here

I've been using [Cysharp/R3][0] with [AvaloniaUI][1]. 
I suspected I'm leaking subscriptions all over the place.
So I wrote a stripped down copy of a view model and turned on the ObservableTracker.
I promptly found a ton of undisposed subscriptions.

I'm thinking I can shove all this into a unit test
and assert that after diposing the main view model,
there are no subscriptions left open.
I have not been able to get that test to pass.

So I started hacking away at my view model
until I got what I think is the smallest not-passing example.

I don't understand why it doesn't pass.
It passes _sometimes_.
So there's a race condition.
Most likely I'm misusing something, just not sure what?


[0]: https://github.com/Cysharp/R3/issues?q=is%3Aissue+dispose
[1]: https://github.com/AvaloniaUI/Avalonia

## Reproduce it (or not) on your machine

First build the project that will reproduce the bug:

```
dotnet publish ConsoleAppTestSubject -c release
```

Try running it once. It often succeeds the first time, but sometimes doesn't.

```
./artifacts/publish/ConsoleAppTestSubject/release/ConsoleAppTestSubject ./readme.md
```

Run it repeatedly until it fails:

```
dotnet run --project ConsoleAppRunner -- ./artifacts/publish/ConsoleAppTestSubject/release/ConsoleAppTestSubject ./readme.md
```

I've never seen it fail running like `dotnet run`.
I'm most interested in the native aot release build.
But it's worth noting I've never seen it fail in this loop:

```
while [ $? -eq 0 ]; do dotnet run --project ConsoleAppTestSubject -- ./readme.md; done
```

## Sample output

Here's what I usually see.
This time, it ran twelve times, then failed.

```
---
Path: ./readme.md
Hash: 9dd7676b5f4742f3f5c8e109a7ee31b0869aee19
$ ./artifacts/publish/ConsoleAppTestSubject/release/ConsoleAppTestSubject ./readme.md
---
Path: ./readme.md
Hash: 9dd7676b5f4742f3f5c8e109a7ee31b0869aee19
$ ./artifacts/publish/ConsoleAppTestSubject/release/ConsoleAppTestSubject ./readme.md
---
Path: ./readme.md
Hash: 9dd7676b5f4742f3f5c8e109a7ee31b0869aee19
TrackingState { TrackingId = 1, FormattedType = ReactiveProperty`1.ObserverNode<String>, AddTime = 10/23/2024 16:38:14, StackTrace =    at R3.Observable`1.Subscribe(Observer`1) + 0x1c
   at ConsoleAppTestSubject.Container..ctor() + 0x14c
   at ConsoleAppTestSubject.Program.<Main>d__0.MoveNext() + 0x70
   at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine&) + 0x40
   at ConsoleAppTestSubject.Program.Main(String[]) + 0x2c
   at ConsoleAppTestSubject.Program.<Main>(String[] args) + 0x10
   at ConsoleAppTestSubject!<BaseAddress>+0x16dfc8
 }
TrackingState { TrackingId = 2, FormattedType = SelectAwait`2.SelectAwaitSwitch<String, String>, AddTime = 10/23/2024 16:38:14, StackTrace =    at ConsoleAppTestSubject.Container..ctor() + 0x14c
   at ConsoleAppTestSubject.Program.<Main>d__0.MoveNext() + 0x70
   at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine&) + 0x40
   at ConsoleAppTestSubject.Program.Main(String[]) + 0x2c
   at ConsoleAppTestSubject.Program.<Main>(String[] args) + 0x10
   at ConsoleAppTestSubject!<BaseAddress>+0x16dfc8
 }
TrackingState { TrackingId = 3, FormattedType = ReactiveProperty`1.ObserverNode<String>, AddTime = 10/23/2024 16:38:14, StackTrace =    at R3.Observable`1.Subscribe(Observer`1) + 0x1c
   at R3.Merge`1.SubscribeCore(Observer`1) + 0xfc
   at R3.Observable`1.Subscribe(Observer`1) + 0x1c
   at R3.Observable`1.Subscribe(Observer`1) + 0x1c
   at ConsoleAppTestSubject.Container..ctor() + 0x220
   at ConsoleAppTestSubject.Program.<Main>d__0.MoveNext() + 0x70
   at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine&) + 0x40
   at ConsoleAppTestSubject.Program.Main(String[]) + 0x2c
   at ConsoleAppTestSubject.Program.<Main>(String[] args) + 0x10
   at ConsoleAppTestSubject!<BaseAddress>+0x16dfc8
 }
TrackingState { TrackingId = 4, FormattedType = AsUnitObservable`1._AsUnitObservable<String>, AddTime = 10/23/2024 16:38:14, StackTrace =    at R3.Merge`1.SubscribeCore(Observer`1) + 0xfc
   at R3.Observable`1.Subscribe(Observer`1) + 0x1c
   at R3.Observable`1.Subscribe(Observer`1) + 0x1c
   at ConsoleAppTestSubject.Container..ctor() + 0x220
   at ConsoleAppTestSubject.Program.<Main>d__0.MoveNext() + 0x70
   at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine&) + 0x40
   at ConsoleAppTestSubject.Program.Main(String[]) + 0x2c
   at ConsoleAppTestSubject.Program.<Main>(String[] args) + 0x10
   at ConsoleAppTestSubject!<BaseAddress>+0x16dfc8
 }
TrackingState { TrackingId = 5, FormattedType = ReactiveProperty`1.ObserverNode<String>, AddTime = 10/23/2024 16:38:14, StackTrace =    at R3.Observable`1.Subscribe(Observer`1) + 0x1c
   at R3.Merge`1.SubscribeCore(Observer`1) + 0xfc
   at R3.Observable`1.Subscribe(Observer`1) + 0x1c
   at R3.Observable`1.Subscribe(Observer`1) + 0x1c
   at ConsoleAppTestSubject.Container..ctor() + 0x220
   at ConsoleAppTestSubject.Program.<Main>d__0.MoveNext() + 0x70
   at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine&) + 0x40
   at ConsoleAppTestSubject.Program.Main(String[]) + 0x2c
   at ConsoleAppTestSubject.Program.<Main>(String[] args) + 0x10
   at ConsoleAppTestSubject!<BaseAddress>+0x16dfc8
 }
TrackingState { TrackingId = 6, FormattedType = AsUnitObservable`1._AsUnitObservable<String>, AddTime = 10/23/2024 16:38:14, StackTrace =    at R3.Merge`1.SubscribeCore(Observer`1) + 0xfc
   at R3.Observable`1.Subscribe(Observer`1) + 0x1c
   at R3.Observable`1.Subscribe(Observer`1) + 0x1c
   at ConsoleAppTestSubject.Container..ctor() + 0x220
   at ConsoleAppTestSubject.Program.<Main>d__0.MoveNext() + 0x70
   at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine&) + 0x40
   at ConsoleAppTestSubject.Program.Main(String[]) + 0x2c
   at ConsoleAppTestSubject.Program.<Main>(String[] args) + 0x10
   at ConsoleAppTestSubject!<BaseAddress>+0x16dfc8
 }
🚨 fail
Ran 12 times.
Unhandled exception. Chell.ProcessTaskException: Process '/opt/homebrew/bin/bash' (85582) has exited with exit code 255. (Executed command: /opt/homebrew/bin/bash -c "set -euo pipefail;./artifacts/publish/ConsoleAppTestSubject/release/ConsoleAppTestSubject ./readme.md")
   at Chell.ProcessTask.AsTaskCore()
   at Chell.ProcessTask.AsTask()
   at Program.<Main>$(String[] args) in /Users/jacob/dev/me/active/ava/LeftoverSubscriptionsRepro/ConsoleAppRunner/Program.cs:line 21
   at Program.<Main>(String[] args)
```
