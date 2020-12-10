namespace SqlStreamStore.FSharp

open SqlStreamStore
open SqlStreamStore.Streams
open FSharp.Prelude

module Read =
    let private fromReadVersionForwards: ReadVersion -> int =
        function
        | ReadVersion.Any -> int (Position.Start)
        | ReadVersion.SpecificVersion version -> int (version)

    let private fromReadVersionBackwards: ReadVersion -> int =
        function
        | ReadVersion.Any -> int (Position.End)
        | ReadVersion.SpecificVersion version -> int (version)

    let readForwards (store: IStreamStore) (stream: string) (readVersion: ReadVersion) (msgCount: int) =
        asyncResult {
            return! store.ReadStreamForwards(StreamId(stream), fromReadVersionForwards readVersion, msgCount) }

    let readBackwards (store: IStreamStore) (stream: string) (readVersion: ReadVersion) (msgCount: int) =
        asyncResult {
            return! store.ReadStreamBackwards(StreamId(stream), fromReadVersionForwards readVersion, msgCount) }

    let readForwardsPrefetch (store: IStreamStore) (stream: string) (readVersion: ReadVersion) (msgCount: int) =
        asyncResult {
            return! store.ReadStreamForwards(StreamId(stream), fromReadVersionForwards readVersion, msgCount, true) }

    let readBackwardsPrefetch (store: IStreamStore) (stream: string) (readVersion: ReadVersion) (msgCount: int) =
        asyncResult {
            return! store.ReadStreamBackwards(StreamId(stream), fromReadVersionForwards readVersion, msgCount, true) }
