# SqlStreamStore.FSharp

A thin F# wrapper around [SQLStreamStore](https://www.nuget.org/packages/SqlStreamStore), and [SqlStreamStore.Postgres](https://www.nuget.org/packages/SqlStreamStore.Postgres).

This library isn't intended to be an implementation of an event store on top of SqlStreamStore. It only wraps some simple functions in SqlStreamStore in F#, mostly to turn `Tasks` to `Asyncs` and offers some nice-to-haves eg. a Postgres config record type instead of a string.

Wrapping only the basic functionality, and sticking to the naming conventions in the original library gives the flexibility to model events, aggregates, error types, ..etc however the user sees fit.
  
For more complete implementations check out other libraries such as [Equinox](https://github.com/jet/equinox). 


## Supported functionality
- Connecting to Postgres
- Reading a `StreamMessage`
- Appending a `StreamMessage`

## TODO

- ~~Postgres connector~~
- ~~Read a stream message~~
- ~~Write a stream message~~
- Subscribe to a stream
