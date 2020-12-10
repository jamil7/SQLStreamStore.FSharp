namespace SqlStreamStore.FSharp

open SqlStreamStore.FSharp.Types
open FSharp.Prelude
open SqlStreamStore.Streams

exception IllegalArgumentException
exception IllegalAppendToAllStream

module StreamControllerBuilder =

    let private streamReadMatcher direction prefetch store stream readVersion messageCount =
        match direction, prefetch with
        | (ReadDirection.Forward, false) -> ReadStream.forwards store stream readVersion messageCount
        | (ReadDirection.Forward, true) -> ReadStream.forwardsPrefetch store stream readVersion messageCount
        | (ReadDirection.Backward, false) -> ReadStream.backwards store stream readVersion messageCount
        | (ReadDirection.Backward, true) -> ReadStream.backwardsPrefetch store stream readVersion messageCount
        | _ -> Async.singleton (Error IllegalArgumentException)

    let private allStreamReadMatcher direction prefetch store startPosition messageCount =
        match direction, prefetch with
        | (ReadDirection.Forward, false) -> ReadAll.forwards store startPosition messageCount
        | (ReadDirection.Forward, true) -> ReadAll.forwardsPrefetch store startPosition messageCount
        | (ReadDirection.Backward, false) -> ReadAll.backwards store startPosition messageCount
        | (ReadDirection.Backward, true) -> ReadAll.backwardsPrefetch store startPosition messageCount
        | _ -> Async.singleton (Error IllegalArgumentException)

    type StreamController(store: SqlStreamStore.IStreamStore, stream: string, readDirection: ReadDirection) =
        member this.append(messages: MessageDetails list, ?appendVersion: AppendVersion)
                           : AsyncResult<AppendResult, exn> =
            let appendVersion' =
                defaultArg appendVersion AppendVersion.Any

            Append.messages store stream appendVersion' messages

        member this.length(?messageCount: int, ?readVersion: ReadVersion, ?prefetch: bool): AsyncResult<int64, exn> =
            let messageCount' = defaultArg messageCount 100
            let readVersion' = defaultArg readVersion ReadVersion.Any
            let prefetch' = defaultArg prefetch false

            asyncResult {
                let! readPage = streamReadMatcher readDirection prefetch' store stream readVersion' messageCount'
                return readPage.Messages.LongLength
            }

        member this.messagesJsonData(?messageCount: int, ?readVersion: ReadVersion, ?prefetch: bool)
                                     : AsyncResult<string list, exn> =
            let messageCount' = defaultArg messageCount 100
            let readVersion' = defaultArg readVersion ReadVersion.Any
            let prefetch' = defaultArg prefetch false

            asyncResult {
                let! readPage = streamReadMatcher readDirection prefetch' store stream readVersion' messageCount'

                return! readPage.Messages
                        |> Array.toList
                        |> List.map (fun msg -> msg.GetJsonData)
                        |> List.map AsyncResult.ofTask
                        |> AsyncResult.sequence
            }
