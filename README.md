# Rx.Unity

An attempt at building a lightweight layer on top of System.Reactive to add support for Unity in a similar fashion as one using UniRx is used to.

Right now it requires some changes to System.Reactive in order to work but I'll try and create a PR to integrate required changes to the official System.Reactive repo soon.

This is still work in progress.
A few tests still need fixing and alot of AoT issues need to be fixed / added hacks to enforce generic type generation for.

Part of the goal is to drop some legacy code from the UniRx code and start using a newer C# Syntax and framework versions.
This also means that older versions of unity won't be supported (Versions >= 2021 should work - testing is currently happening in the unity beta version only)

Other than the basic scheduling these are the features I moved over from UniRx:

Assembly: Rx.Data
- .AsUnitObservable()
- .AsSingleUnitObservable()
- .Pairwise() (without overloads)
- ReactiveProperty
- ReactiveCollection
- ReactiveDictionary
(all of those incl. readonly variants)

Assembly: Rx.Unity
- ObservableStateMachineTrigger
- Observable.ReturnUnit()
- Observable.EveryUpdate()
- Observable.EveryFixedUpdate()
- .UpdateAsObservable()
- .OnDestroyAsObservable()
- .ObserveOnMainThread() (without overloads)
- .SubscribeOnMainThread() (without overloads)
- .TakeUntilDestroy()
- .DelayFrame()
- .BatchFrame()

---
changes:
- removed serialization support ReactiveProperty
- removed serialization support ReactiveDictionary
- removed IOptimizedObservable stuff

---
Important: when using System.Reactive.Unity it is crucial that
`Rx.Unity.ReactiveUnity.SetupPatches()`
is executed before any other reactive code to ensure the environment is properly configured for unity.


