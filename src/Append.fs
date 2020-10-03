namespace SqlStreamStore.FSharp

open System
open SqlStreamStore
open SqlStreamStore.Streams

type NewMessage = NewStreamMessage
type Connection = IStreamStore
type Position = int

type StreamName = string

type MessageDetails =
    { id: Id
      event: string
      body: string
      metadata: string }

and Id =
    | Custom of Guid
    | Auto

module append =
    let appendMessage: Connection-> StreamName -> Position-> MessageDetails -> Async<AppendResult> =
        fun conn stream pos msg->
            let id: Id -> Guid =
                function
                | Custom guid -> guid
                | Auto -> Guid.NewGuid()

            let createMessage: MessageDetails -> NewMessage =
                fun msg ->
                    match msg.metadata with
                    | "" -> NewMessage(id msg.id, msg.event, msg.body)
                    | metadata -> NewMessage(id msg.id, msg.event, msg.body, metadata)

            let append : Connection-> StreamName -> Position-> MessageDetails-> Async<AppendResult> =
                fun conn stream pos msg ->
                    conn.AppendToStream (stream, pos, createMessage msg)
                    |> Async.AwaitTask
            
            append conn stream pos msg