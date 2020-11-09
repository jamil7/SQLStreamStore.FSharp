namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

module Get =
    let messagePayload (message: StreamMessage): Async<Result<string, exn>> =
        GetRaw.messagePayload message
        |> ExceptionsHandler.asyncExceptionHandler

    let messagesPayload (messages: StreamMessage list) =
        messages
        |> List.map messagePayload
        |> Async.parallelCombine
        |> Async.map
            (List.fold (fun (payloads, exceptions) result ->
                match result with
                | Ok payload -> (payloads @ [ payload ], exceptions)
                | Error exception' -> (payloads, exceptions @ [ exception' ])) ([], []))
