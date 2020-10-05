namespace SqlStreamStore.FSharp


open System
open Insurello.AsyncExtra
open SqlStreamStore
open SqlStreamStore.FSharp
open SqlStreamStore.Streams

type MessageDetails =
    { id: Id
      type_: string
      jsonData: string
      jsonMetadata: string }

and Id =
    | Custom of System.Guid
    | Auto

module Append =
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
                    store.AppendToStream
                        (streamDetails.streamName, Helpers.toVersion streamDetails.version, createMessage messageDetails)
                    |> Async.AwaitTask

            append store streamDetails messageDetails

module AppendExtras =
    let appendNewMessage: IStreamStore -> StreamDetails -> MessageDetails -> AsyncResult<AppendResult, AppendException> =
        fun store streamDetails messageDetails ->
            Append.appendNewMessage store streamDetails messageDetails
            |> Async.Catch
            |> Async.map (function
                | Choice1Of2 response -> Ok response
                | Choice2Of2 exn ->
                    Error
                    <| match exn with
                       | :? AggregateException as exn -> exn.InnerException |> AppendException.WrongExpectedVersion   
                       | _ as exn -> exn |> AppendException.Other)          
