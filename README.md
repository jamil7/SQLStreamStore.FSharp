[![NuGet Badge](https://buildstats.info/nuget/SqlStreamStore.FSharp?includePreReleases=true)](https://www.nuget.org/packages/SqlStreamStore.FSharp/0.0.1-alpha.12)
# SqlStreamStore.FSharp

A thin F# wrapper around [SQLStreamStore](https://www.nuget.org/packages/SqlStreamStore), and [SqlStreamStore.Postgres](https://www.nuget.org/packages/SqlStreamStore.Postgres).

This library isn't intended to be an implementation of an event store on top of SqlStreamStore. It only wraps some simple functions in SqlStreamStore in F#, mostly to turn `Tasks` to `Asyncs` and offers some nice-to-haves eg. a Postgres config record type instead of a string.

Wrapping only the basic functionality, and sticking to the naming conventions in the original library gives the flexibility to model events, aggregates, error types, etc.. however the user sees fit.
  
For more complete implementations check out other libraries such as [Equinox](https://github.com/jet/equinox). 


## Supported functionality
- Connecting to Postgres
- Reading a `StreamMessage`
- Appending a `StreamMessage`

## Not Supported functionality
- Subscribtions aren't going to be supported. Wrapping them is more awkward than using C# in F#.

## TODO

- ~~Postgres connector~~
- ~~Read a stream message~~
- ~~Write a stream message~~
- ~~Subscribe to a stream~~
- ~~Work on Read, and Append modules that wrap read and appends in AsyncResult type for error handeling~~
- Add additional tests when Append and Read fail, and throw exceptions.
