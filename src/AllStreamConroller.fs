namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.FSharp.Types
open SqlStreamStore.Streams

exception IllegalArgumentException

module AllStreamController =
    let private allStreamReadMatcher direction prefetch store startPosition messageCount =
        match direction, prefetch with
        | (ReadDirection.Forward, false) -> ReadAll.forwards store startPosition messageCount
        | (ReadDirection.Forward, true) -> ReadAll.forwardsPrefetch store startPosition messageCount
        | (ReadDirection.Backward, false) -> ReadAll.backwards store startPosition messageCount
        | (ReadDirection.Backward, true) -> ReadAll.backwardsPrefetch store startPosition messageCount
        | _ -> Async.singleton (Error IllegalArgumentException)

    type StreamController(store: SqlStreamStore.IStreamStore, readDirection: ReadDirection, ?prefetch: bool) =
        let prefetch' = defaultArg prefetch false

        interface IAllStreamController<StartPosition> with
            member this.length(?messageCount: int, ?startPosition: StartPosition): AsyncResult<int64, exn> =
                let messageCount' = defaultArg messageCount 100

                let startPosition' =
                    defaultArg startPosition StartPosition.Any

                asyncResult {
                    let! readPage = allStreamReadMatcher readDirection prefetch' store startPosition' messageCount'
                    return readPage.Messages.LongLength
                }

            member this.messagesJsonData(?messageCount: int, ?startPosition: StartPosition)
                                         : AsyncResult<string list, exn> =
                let messageCount' = defaultArg messageCount 1000

                let startPosition' =
                    defaultArg startPosition StartPosition.Any

                asyncResult {
                    let! readPage = allStreamReadMatcher readDirection prefetch' store startPosition' messageCount'

                    return! readPage.Messages
                            |> Array.toList
                            |> List.map (fun msg -> msg.GetJsonData)
                            |> List.map AsyncResult.ofTask
                            |> AsyncResult.sequence
                }
