[![NuGet Badge](https://buildstats.info/nuget/SqlStreamStore.FSharp?includePreReleases=true)](https://www.nuget.org/packages/SqlStreamStore.FSharp/0.0.1-alpha.12)

# SqlStreamStore.FSharp

A thin F# wrapper around [SQLStreamStore](https://www.nuget.org/packages/SqlStreamStore),
and [SqlStreamStore.Postgres](https://www.nuget.org/packages/SqlStreamStore.Postgres).

## What does it do?

- Provides a nice API to interact with SqlStreamStore from F#
- Wraps all functions that can throw in `Async<Result<'a, exn>>`

## Usage
This quick usage guide presumes familiarity with SQLStreamStore. A more in-depth guide and documentation are coming soon :) 

### Creating a store

Use the `Create` module.

```f#
    open SqlStreamStore
    open SqlStreamStore.FSharp
    open System
    
    // A new in memory store
    let inMemStore : IStreamStore = Create.inMemoryStore()
    
    
    let config : PostgresConfig = 
        {
            host = Environment.GetEnvironmentVariable "HOST"
            port = Environment.GetEnvironmentVariable "PORT"
            username = Environment.GetEnvironmentVariable "USERNAME"
            password = Environment.GetEnvironmentVariable "PASSWORD"
            database = Environment.GetEnvironmentVariable "DATABASE"
        } 
        
    // A new PostgresStore with a custom schema (use None if you want the tables to be created in public)
    let postgresStore : PostgresStreamStore = Create.postgresStore config (Some "my-cool-schema")    
    
    // Cast to IStreamStore
    let store : IStreamStore = postgresStore :> IStreamStore       
    
    // Or create schema if this is the first run 
    store |> Create.schemaIfNotExists       // : Async<Result<IStreamStore,exn>>
```

### Appending to a stream

Use the `Connect` module to connect to the stream you want to append to, and then use the `Append` module to append
messages or events.

```f#
    open ...
    
    type MyEvent = 
        | Hi
        | Greeting of Greeting
        
    and Greeting = {data: string}
    
    let eventToAppend : NewStreamEvent<MyEvent> = 
        Greeting {data = "hello there!"}
        |> NewStreamEvent.create "me"
    
    // Given an IStreamStore called store
    store                                               // IStreamStore
    |> Connect.toStream "my-cool-stream"                // Stream
    |> Append.streamEvents [eventToAppend]              // AsyncResult<AppendResult, exn>
```

### Reading and getting some data from a stream

Use the `Read` module to read the stream and then the `Get` module to get some data without having to bind the result of
the read operation.

```f#
    open ...
    
    // Using the MyType declared before and an IStreamStore called store
    store                                               // IStreamStore
    |> Connect.toStream "my-cool-stream"                // Stream 
    |> Read.entire                                      // Async<Result<ReadStreamPage,exn>>
    |> Get.messagesData                                 // Async<Result<string,exn>>   
```

#### What if I want to read the stream backwards and specify if I want to prefetch the data, and all the cool stuff that already exists in SQLStreamStore?

Well each of the above functions has an alternative version call that has a `'` at the end for the function's name. The
alternative function takes a list of options as a parameter.

#### Why do this?

Because 90% of the time these options aren't used, and this is an elegant way to hide them without having to use
builder-pattern.

#### Example

```f#
    open ...
    
    // Using the MyType declared before and an IStreamStore called store
    store                                               // IStreamStore
    |> Connect.toStream "my-cool-stream"                // Stream 
    |> Read.entire' [
                     ReadEntireOption.FromVersionInclusive 50
                     ReadEntireOption.NoPrefetch
                     ReadEntireOption.ReadBackwards
                     ]                                  // Async<Result<ReadStreamPage,exn>>
    |> Get.messagesData                                 // Async<Result<string,exn>>   
```
