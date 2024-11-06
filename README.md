# Zafiro
The new "GLASS"

Hello! 

This is a **utility toolkit** that contains abstractions to deal with common challenges in .NET programming. 

- Extensions to deal with Reactive and IEnumerable.
- Data and Data Analysis structures, like trees and graphs, intervals, clusters (and clustering algorithms).
- A super powerful abstraction for filesystems (Zafiro.FileSystem).
- A Data Model to mix Observables, Streams and other byte level stuff.
- A lot of extensions for CSharpFunctionalExtensions by @vkhorikov.

You can find interesting stuff here, but it would requiere an in depth look to the code. My advice to learn how can serve your own project is to look how the other Zafiro.* projects use it.

A compelling example of what it can do:

- Create a tree out of any hierarchical structure to a fully-fledged tree, from which you can extract a lot of interesting properties.
- Negate a observable of booleans with obs.Not()
- Clusterize a set of data
- Organize data in a Table
- Create a Graph
- Convert a Stream to an IObservable<byte[]> or IObservable<byte>.
- ...

# Credits and acknowledgements
- Contains code by [Pedro Lamas](https://twitter.com/pedrolamas) / [Cimbalino Toolkit](https://cimbalino.org/])
