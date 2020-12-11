namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.FSharp.Types
open SqlStreamStore.Streams

exception IllegalArgumentException

module StreamControllerBuilder =
    let private streamReadMatcher direction prefetch store stream readVersion messageCount =
        match direction, prefetch with
        | (ReadDirection.Forward, false) -> ReadStream.forwards store stream readVersion messageCount
        | (ReadDirection.Backward, false) -> ReadStream.backwards store stream readVersion messageCount
        | (ReadDirection.Forward, true) -> ReadStream.forwardsPrefetch store stream readVersion messageCount
        | (ReadDirection.Backward, true) -> ReadStream.backwardsPrefetch store stream readVersion messageCount
        | _ -> Async.singleton (Error IllegalArgumentException)

    type StreamController(store: SqlStreamStore.IStreamStore, stream: string) =
        member this.append(messages: MessageDetails list, ?appendVersion: AppendVersion)
                           : AsyncResult<AppendResult, exn> =
            let appendVersion' =
                defaultArg appendVersion AppendVersion.Any

            Append.messages store stream appendVersion' messages

        member this.messages(?readDirection: ReadDirection,
                             ?readVersion: ReadVersion,
                             ?messageCount: int,
                             ?prefetch: bool) =
            let readDirection' =
                defaultArg readDirection ReadDirection.Forward

            let readVersion' = defaultArg readVersion ReadVersion.Any
            let messageCount' = defaultArg messageCount 1000
            let prefetch' = defaultArg prefetch false

            let messages =
                asyncResult {
                    let! readPage = streamReadMatcher readDirection' prefetch' store stream readVersion' messageCount'

                    return List.ofArray readPage.Messages
                }

            StreamMessages(messages)
