namespace SqlStreamStore.FSharp

open System.Collections.Generic
open FSharp.Prelude
open SqlStreamStore.FSharp.Types
open SqlStreamStore.Streams

exception IllegalArgumentException

module StreamControllerBuilder =
    let private streamReadMatcher direction prefetch store stream readVersion messageCount =
        match direction, prefetch with
        | (ReadDirection.Forward, false) -> ReadStream.forwards store stream readVersion messageCount
        | (ReadDirection.Forward, true) -> ReadStream.forwardsPrefetch store stream readVersion messageCount
        | (ReadDirection.Backward, false) -> ReadStream.backwards store stream readVersion messageCount
        | (ReadDirection.Backward, true) -> ReadStream.backwardsPrefetch store stream readVersion messageCount
        | _ -> Async.singleton (Error IllegalArgumentException)

    type StreamController(store: SqlStreamStore.IStreamStore,
                          stream: string,
                          readDirection: ReadDirection,
                          ?prefetch: bool) =
        inherit AbstractStreamController<ReadVersion>()
        let prefetch' = defaultArg prefetch false

        override this.append(messages: MessageDetails list, ?appendVersion: AppendVersion)
                             : AsyncResult<AppendResult, exn> =
            let appendVersion' =
                defaultArg appendVersion AppendVersion.Any

            Append.messages store stream appendVersion' messages

        override this.filteredMessagesJsonData(predicate: StreamMessage -> bool,
                                               ?messageCount: int,
                                               ?readVersion: ReadVersion)
                                               : AsyncResult<string list, exn> =
            let messageCount' = defaultArg messageCount 1000
            let readVersion' = defaultArg readVersion ReadVersion.Any

            asyncResult {
                let! readPage = streamReadMatcher readDirection prefetch' store stream readVersion' messageCount'

                return! readPage.Messages
                        |> List.ofArray
                        |> List.filter predicate
                        |> List.map (fun msg -> msg.GetJsonData)
                        |> List.map AsyncResult.ofTask
                        |> AsyncResult.sequence
            }

        override this.length(?messageCount: int, ?readVersion: ReadVersion): AsyncResult<int64, exn> =
            let messageCount' = defaultArg messageCount 100
            let readVersion' = defaultArg readVersion ReadVersion.Any

            asyncResult {
                let! readPage = streamReadMatcher readDirection prefetch' store stream readVersion' messageCount'
                return readPage.Messages.LongLength
            }

        override this.messagesJsonData(?messageCount: int, ?readVersion: ReadVersion): AsyncResult<string list, exn> =
            let messageCount' = defaultArg messageCount 1000
            let readVersion' = defaultArg readVersion ReadVersion.Any

            asyncResult {
                let! readPage = streamReadMatcher readDirection prefetch' store stream readVersion' messageCount'

                return! readPage.Messages
                        |> Array.toList
                        |> List.map (fun msg -> msg.GetJsonData)
                        |> List.map AsyncResult.ofTask
                        |> AsyncResult.sequence
            }
