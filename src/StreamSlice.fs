namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.Streams

type StreamSlice(readStreamPage: AsyncResult<ReadStreamPage, exn>) =
    let apply f =
        asyncResult {
            let! page = readStreamPage
            return f page
        }

    member this.messages() =
        apply (fun page -> page.Messages |> Array.toList)

    member this.status() = apply (fun page -> page.Status)

    member this.isEnd() = apply (fun page -> page.IsEnd)

    member this.readDirection() = apply (fun page -> page.ReadDirection)

    member this.readNextSnapshot() =
        let nextPage =
            asyncResult {
                let! page = readStreamPage
                return! page.ReadNext()
            }

        StreamSlice nextPage

    member this.streamId() = apply (fun page -> page.StreamId)

    member this.fromStreamVersion() =
        apply (fun page -> page.FromStreamVersion)

    member this.lastStreamPosition() =
        apply (fun page -> page.LastStreamPosition)

    member this.lastStreamVersion() =
        apply (fun page -> page.LastStreamVersion)

    member this.nextStreamVersion() =
        apply (fun page -> page.NextStreamVersion)

    member this.ReadStreamPage = readStreamPage
