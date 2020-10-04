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
            let id: Id -> System.Guid =
                function
                | Custom guid -> guid
                | Auto -> System.Guid.NewGuid()

            let createMessage: MessageDetails -> NewStreamMessage =
                fun msg ->
                    match msg.jsonMetadata with
                    | "" -> NewStreamMessage(id msg.id, msg.type_, msg.jsonData)
                    | metadata -> NewStreamMessage(id msg.id, msg.type_, msg.jsonData, metadata)

            let toVersion: Version -> int =
                function
                | Version.None
                | Version.Any -> ExpectedVersion.Any
                | Version.EmptyStream -> ExpectedVersion.EmptyStream
                | Version.NoStream -> ExpectedVersion.NoStream
                | Version.SpecificVersion version -> version

            let append: IStreamStore -> StreamDetails -> MessageDetails -> Async<AppendResult> =
                fun store stream msg ->
                    store.AppendToStream(stream.streamName, toVersion stream.version, createMessage msg)
                    |> Async.AwaitTask

            append store streamDetails messageDetails
