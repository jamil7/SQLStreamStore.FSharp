namespace SqlStreamStore.FSharp

open System.Threading
open SqlStreamStore
open SqlStreamStore.Streams

type ReadingDirection =
    | Forward
    | Backward

module Read =
    let readFromStreamAsync: IStreamStore -> ReadingDirection -> StreamDetails -> int -> Async<ReadStreamPage> =
        fun conn direction stream msgCount ->
            match direction with
            | Forward -> conn.ReadStreamForwards(stream.streamName, stream.position, msgCount)
            | Backward -> conn.ReadStreamBackwards(stream.streamName, stream.position, msgCount)
            |> Async.AwaitTask

    let readFromStreamAsync': IStreamStore -> ReadingDirection -> StreamDetails -> int -> CancellationToken -> Async<ReadStreamPage> =
        fun conn direction stream msgCount cancellationToken ->
            match direction with
            | Forward -> conn.ReadStreamForwards(stream.streamName, stream.position, msgCount, cancellationToken)
            | Backward -> conn.ReadStreamBackwards(stream.streamName, stream.position, msgCount, cancellationToken)
            |> Async.AwaitTask
