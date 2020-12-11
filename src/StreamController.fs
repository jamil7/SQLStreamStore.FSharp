namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.FSharp.Types
open SqlStreamStore.Streams

exception IllegalArgumentException

type StreamController(store: SqlStreamStore.IStreamStore, stream: string) =
    inherit AbstractStreamController()

    let streamReadMatcher direction prefetch store stream readVersion messageCount =
        match direction, prefetch with
        | (ReadDirection.Forward, false) -> ReadStream.forwards store stream readVersion messageCount
        | (ReadDirection.Backward, false) -> ReadStream.backwards store stream readVersion messageCount
        | (ReadDirection.Forward, true) -> ReadStream.forwardsPrefetch store stream readVersion messageCount
        | (ReadDirection.Backward, true) -> ReadStream.backwardsPrefetch store stream readVersion messageCount
        | _ -> Async.singleton (Error IllegalArgumentException)

    override this.append(messages: MessageDetails list, ?appendVersion: AppendVersion): AsyncResult<AppendResult, exn> =
        let appendVersion' =
            defaultArg appendVersion AppendVersion.Any

        Append.messages store stream appendVersion' messages

    override this.isEnd(?readDirection: ReadDirection, ?readVersion: ReadVersion, ?messageCount: int, ?prefetch: bool) =
        let readDirection' =
            defaultArg readDirection ReadDirection.Forward

        let readVersion' = defaultArg readVersion ReadVersion.Any
        let messageCount' = defaultArg messageCount 1000
        let prefetch' = defaultArg prefetch false


        asyncResult {
            let! readPage = streamReadMatcher readDirection' prefetch' store stream readVersion' messageCount'
            return readPage.IsEnd
        }

    override this.readDirection(?readDirection: ReadDirection,
                                ?readVersion: ReadVersion,
                                ?messageCount: int,
                                ?prefetch: bool) =
        let readDirection' =
            defaultArg readDirection ReadDirection.Forward

        let readVersion' = defaultArg readVersion ReadVersion.Any
        let messageCount' = defaultArg messageCount 1000
        let prefetch' = defaultArg prefetch false


        asyncResult {
            let! readPage = streamReadMatcher readDirection' prefetch' store stream readVersion' messageCount'
            return readPage.ReadDirection
        }

    override this.streamId(?readDirection: ReadDirection, ?readVersion: ReadVersion, ?messageCount: int, ?prefetch: bool) =
        let readDirection' =
            defaultArg readDirection ReadDirection.Forward

        let readVersion' = defaultArg readVersion ReadVersion.Any
        let messageCount' = defaultArg messageCount 1000
        let prefetch' = defaultArg prefetch false


        asyncResult {
            let! readPage = streamReadMatcher readDirection' prefetch' store stream readVersion' messageCount'
            return readPage.StreamId
        }

    override this.fromStreamVersion(?readDirection: ReadDirection,
                                    ?readVersion: ReadVersion,
                                    ?messageCount: int,
                                    ?prefetch: bool) =
        let readDirection' =
            defaultArg readDirection ReadDirection.Forward

        let readVersion' = defaultArg readVersion ReadVersion.Any
        let messageCount' = defaultArg messageCount 1000
        let prefetch' = defaultArg prefetch false


        asyncResult {
            let! readPage = streamReadMatcher readDirection' prefetch' store stream readVersion' messageCount'
            return readPage.FromStreamVersion
        }

    override this.lastStreamPosition(?readDirection: ReadDirection,
                                     ?readVersion: ReadVersion,
                                     ?messageCount: int,
                                     ?prefetch: bool) =
        let readDirection' =
            defaultArg readDirection ReadDirection.Forward

        let readVersion' = defaultArg readVersion ReadVersion.Any
        let messageCount' = defaultArg messageCount 1000
        let prefetch' = defaultArg prefetch false


        asyncResult {
            let! readPage = streamReadMatcher readDirection' prefetch' store stream readVersion' messageCount'
            return readPage.LastStreamPosition
        }

    override this.lastStreamVersion(?readDirection: ReadDirection,
                                    ?readVersion: ReadVersion,
                                    ?messageCount: int,
                                    ?prefetch: bool) =
        let readDirection' =
            defaultArg readDirection ReadDirection.Forward

        let readVersion' = defaultArg readVersion ReadVersion.Any
        let messageCount' = defaultArg messageCount 1000
        let prefetch' = defaultArg prefetch false


        asyncResult {
            let! readPage = streamReadMatcher readDirection' prefetch' store stream readVersion' messageCount'
            return readPage.LastStreamVersion
        }

    override this.nextStreamVersion(?readDirection: ReadDirection,
                                    ?readVersion: ReadVersion,
                                    ?messageCount: int,
                                    ?prefetch: bool) =
        let readDirection' =
            defaultArg readDirection ReadDirection.Forward

        let readVersion' = defaultArg readVersion ReadVersion.Any
        let messageCount' = defaultArg messageCount 1000
        let prefetch' = defaultArg prefetch false


        asyncResult {
            let! readPage = streamReadMatcher readDirection' prefetch' store stream readVersion' messageCount'
            return readPage.NextStreamVersion
        }

    override this.messages(?readDirection: ReadDirection, ?readVersion: ReadVersion, ?messageCount: int, ?prefetch: bool) =
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
