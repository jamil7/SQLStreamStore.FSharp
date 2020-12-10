namespace SqlStreamStore.FSharp

open SqlStreamStore
open SqlStreamStore.Streams
open SqlStreamStore.FSharp.Types
open FSharp.Prelude

module ReadStream =
    let private fromReadVersionForwards (readVersion: ReadVersion): int =
        match readVersion with
        | ReadVersion.Any -> int (Position.Start)
        | ReadVersion.SpecificVersion version -> int (version)

    let private fromReadVersionBackwards (readVersion: ReadVersion): int =
        match readVersion with
        | ReadVersion.Any -> int (Position.End)
        | ReadVersion.SpecificVersion version -> int (version)

    let forwards (store: IStreamStore) (stream: string) (readVersion: ReadVersion) (msgCount: int) =
        asyncResult {
            return! store.ReadStreamForwards(StreamId(stream), fromReadVersionForwards readVersion, msgCount) }

    let backwards (store: IStreamStore) (stream: string) (readVersion: ReadVersion) (msgCount: int) =
        asyncResult {
            return! store.ReadStreamBackwards(StreamId(stream), fromReadVersionForwards readVersion, msgCount) }

    let forwardsPrefetch (store: IStreamStore) (stream: string) (readVersion: ReadVersion) (msgCount: int) =
        asyncResult {
            return! store.ReadStreamForwards(StreamId(stream), fromReadVersionForwards readVersion, msgCount, true) }

    let backwardsPrefetch (store: IStreamStore) (stream: string) (readVersion: ReadVersion) (msgCount: int) =
        asyncResult {
            return! store.ReadStreamBackwards(StreamId(stream), fromReadVersionForwards readVersion, msgCount, true) }

module ReadAll =
    let private fromReadVersionForwards: ReadVersion -> int =
        function
        | ReadVersion.Any -> int (Position.Start)
        | ReadVersion.SpecificVersion version -> int (version)

    let private fromReadVersionBackwards: ReadVersion -> int =
        function
        | ReadVersion.Any -> int (Position.End)
        | ReadVersion.SpecificVersion version -> int (version)

    let private fromStartPositionInclusiveForwards: StartPosition -> int64 =
        function
        | StartPosition.Any -> 0L
        | StartPosition.SpecificPosition position -> position

    let private fromStartPositionInclusiveBackwards: StartPosition -> int64 =
        function
        | StartPosition.Any -> -1L
        | StartPosition.SpecificPosition position -> position

    let forwards (store: IStreamStore) (startPositionInclusive: StartPosition) (msgCount: int) =
        asyncResult {
            return! store.ReadAllForwards(fromStartPositionInclusiveForwards startPositionInclusive, msgCount) }

    let backwards (store: IStreamStore) (startPositionInclusive: StartPosition) (msgCount: int) =
        asyncResult {
            return! store.ReadAllBackwards(fromStartPositionInclusiveBackwards startPositionInclusive, msgCount) }

    let forwardsPrefetch (store: IStreamStore) (startPositionInclusive: StartPosition) (msgCount: int) =
        asyncResult {
            return! store.ReadAllForwards(fromStartPositionInclusiveForwards startPositionInclusive, msgCount, true) }

    let backwardsPrefetch (store: IStreamStore) (startPositionInclusive: StartPosition) (msgCount: int) =
        asyncResult {
            return! store.ReadAllBackwards(fromStartPositionInclusiveBackwards startPositionInclusive, msgCount, true) }
