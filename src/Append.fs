namespace SqlStreamStore.FSharp


open SqlStreamStore
open SqlStreamStore.Streams

type MessageDetails =
    { id: Id
      type_: string
      jsonData: string
      jsonMetadata: string }

and Id =
    | Custom of System.Guid
    | Auto

module append =
    let appendNewMessage: IStreamStore -> StreamDetails -> MessageDetails -> Async<AppendResult> =
        fun store streamDetails messageDetails ->
            let toId: Id -> System.Guid =
                function
                | Custom guid -> guid
                | Auto -> System.Guid.NewGuid()

            let createMessage: MessageDetails -> NewStreamMessage =
                fun msg ->
                    match msg.jsonMetadata with
                    | "" -> NewStreamMessage(toId msg.id, msg.type_, msg.jsonData)
                    | metadata -> NewStreamMessage(toId msg.id, msg.type_, msg.jsonData, metadata)

            let append: IStreamStore -> StreamDetails -> MessageDetails -> Async<AppendResult> =
                fun store streamDetails messageDetails ->
                    store.AppendToStream(streamDetails.streamName, Helpers.toVersion streamDetails.version, createMessage messageDetails)
                    |> Async.AwaitTask

            append store streamDetails messageDetails
