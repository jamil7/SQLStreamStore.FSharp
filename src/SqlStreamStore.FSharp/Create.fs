namespace SqlStreamStore.FSharp

open SqlStreamStore

module Create =

    /// Represents an in-memory implementation of a stream store. Use for testing or high/speed + volatile scenarios.
    let inMemoryStore : unit -> InMemoryStreamStore = fun _ -> new InMemoryStreamStore()