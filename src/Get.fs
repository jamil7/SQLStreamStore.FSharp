namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

module Get =
    /// Get jsonData from a StreamMessage or an exception wrapped in a Result.
    let messagePayload (message: StreamMessage): Async<Result<string, exn>> =
        GetRaw.messagePayload message
        |> ExceptionsHandler.asyncExceptionHandler

    /// Get a list of jsonData from a list of StreamMessages of a list of exceptions wrapped in a Result.
    let messagesPayload (messages: StreamMessage list) =
        messages
        |> List.map messagePayload
        |> Async.parallelCombine
        |> Async.map
            (List.fold (fun (payloads, exceptions) result ->
                match result with
                | Ok payload -> (payloads @ [ payload ], exceptions)
                | Error exception' -> (payloads, exceptions @ [ exception' ])) ([], []))
