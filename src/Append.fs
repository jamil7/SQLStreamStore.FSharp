namespace SqlStreamStore.FSharp

open System
open SqlStreamStore
open SqlStreamStore.Streams

type StreamDetails = { streamName: string; position: int }

type MessageDetails =
    { id: Id
      type_: string
      jsonData: string
      jsonMetadata: string }

and Id =
    | Custom of Guid
    | Auto

module append =
    let appendNewMessage: IStreamStore -> StreamDetails -> MessageDetails -> Async<AppendResult> =
        fun store stream msg ->
            let id: Id -> Guid =
                function
                | Custom guid -> guid
                | Auto -> Guid.NewGuid()

            let createMessage: MessageDetails -> NewStreamMessage =
                fun msg ->
                    match msg.jsonMetadata with
                    | "" -> NewStreamMessage(id msg.id, msg.type_, msg.jsonData)
                    | metadata -> NewStreamMessage(id msg.id, msg.type_, msg.jsonData, metadata)

            let append: IStreamStore -> StreamDetails -> MessageDetails -> Async<AppendResult> =
                fun store stream msg ->
                    store.AppendToStream(stream.streamName, stream.position, createMessage msg)
                    |> Async.AwaitTask

            append store stream msg
