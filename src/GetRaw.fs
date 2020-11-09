namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

module GetRaw =
    /// Get jsonData from a StreamMessage.
    /// Can throw exceptions.
    /// Not recommended to use. Refer to Get module's functions.
    let messagePayload (message: StreamMessage): Async<string> =
        async {
            return! message.GetJsonData()
                    |> Async.awaitTaskWithInnerException
        }

    /// Get a list of jsonData from a list of StreamMessages.
    /// Can throw exceptions for each message.
    /// Not recommended to use. Refer to Get module's functions.
    let messagesPayload: StreamMessage list -> Async<string list> =
        List.map messagePayload >> Async.parallelCombine
