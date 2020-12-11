namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.FSharp.Types
open SqlStreamStore.Streams
open SqlStreamStore.Streams

exception IllegalArgumentException

module AllStreamController =
    let private allStreamReadMatcher direction prefetch store startPosition messageCount =
        match direction, prefetch with
        | (ReadDirection.Forward, false) -> ReadAll.forwards store startPosition messageCount
        | (ReadDirection.Backward, false) -> ReadAll.backwards store startPosition messageCount
        | (ReadDirection.Forward, true) -> ReadAll.forwardsPrefetch store startPosition messageCount
        | (ReadDirection.Backward, true) -> ReadAll.backwardsPrefetch store startPosition messageCount
        | _ -> Async.singleton (Error IllegalArgumentException)

    type AllStreamController(store: SqlStreamStore.IStreamStore) =
        inherit AbstractAllStreamController()

        override this.direction(?readDirection: ReadDirection,
                                ?startPosition: StartPosition,
                                ?messageCount: int,
                                ?prefetch: bool) =
            let readDirection' =
                defaultArg readDirection ReadDirection.Forward

            let startPosition' =
                defaultArg startPosition StartPosition.Any

            let messageCount' = defaultArg messageCount 1000

            let prefetch' = defaultArg prefetch false
            asyncResult {
                let! readPage = allStreamReadMatcher readDirection' prefetch' store startPosition' messageCount'
                return readPage.Direction
            }

        override this.fromPosition(?readDirection: ReadDirection,
                                   ?startPosition: StartPosition,
                                   ?messageCount: int,
                                   ?prefetch: bool) =
            let readDirection' =
                defaultArg readDirection ReadDirection.Forward

            let startPosition' =
                defaultArg startPosition StartPosition.Any

            let messageCount' = defaultArg messageCount 1000

            let prefetch' = defaultArg prefetch false
            asyncResult {
                let! readPage = allStreamReadMatcher readDirection' prefetch' store startPosition' messageCount'
                return readPage.FromPosition
            }

        override this.isEnd(?readDirection: ReadDirection,
                            ?startPosition: StartPosition,
                            ?messageCount: int,
                            ?prefetch: bool) =
            let readDirection' =
                defaultArg readDirection ReadDirection.Forward

            let startPosition' =
                defaultArg startPosition StartPosition.Any

            let messageCount' = defaultArg messageCount 1000

            let prefetch' = defaultArg prefetch false
            asyncResult {
                let! readPage = allStreamReadMatcher readDirection' prefetch' store startPosition' messageCount'
                return readPage.IsEnd
            }

        override this.nextPosition(?readDirection: ReadDirection,
                                   ?startPosition: StartPosition,
                                   ?messageCount: int,
                                   ?prefetch: bool) =
            let readDirection' =
                defaultArg readDirection ReadDirection.Forward

            let startPosition' =
                defaultArg startPosition StartPosition.Any

            let messageCount' = defaultArg messageCount 1000

            let prefetch' = defaultArg prefetch false
            asyncResult {
                let! readPage = allStreamReadMatcher readDirection' prefetch' store startPosition' messageCount'
                return readPage.NextPosition
            }

        // TODO: think about ReadNext

        override this.messages(?readDirection: ReadDirection,
                               ?startPosition: StartPosition,
                               ?messageCount: int,
                               ?prefetch: bool) =
            let readDirection' =
                defaultArg readDirection ReadDirection.Forward

            let startPosition' =
                defaultArg startPosition StartPosition.Any

            let messageCount' = defaultArg messageCount 1000

            let prefetch' = defaultArg prefetch false


            let messages =
                asyncResult {
                    let! readPage = allStreamReadMatcher readDirection' prefetch' store startPosition' messageCount'
                    return List.ofArray readPage.Messages
                }

            StreamMessages(messages)
