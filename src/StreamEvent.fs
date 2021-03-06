namespace SqlStreamStore.FSharp.EventSourcing

open FSharp.Prelude
open SqlStreamStore.FSharp
open SqlStreamStore.Streams
open System

type private Metadata =
    { timestamp: DateTimeOffset
      author: string
      causationId: Guid option
      correlationId: Guid
      meta: string option }

type private NewStreamEventInternal<'a> =
    { data: 'a
      author: string
      id: Guid
      timestamp: DateTimeOffset
      correlationId: Guid
      causationId: Guid option
      metadata: string option }

type NewStreamEvent<'a> = private NewStreamEvent of NewStreamEventInternal<'a>

module NewStreamEvent =
    let create<'a> (author: string) (data: 'a): NewStreamEvent<'a> =
        NewStreamEvent
            { data = data
              author = author
              id = Guid.NewGuid()
              timestamp = DateTimeOffset.Now
              correlationId = Guid.NewGuid()
              causationId = None
              metadata = None }

    let withId (id: Guid): NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent eventData) -> NewStreamEvent { eventData with id = id }

    let withTimestamp (timestamp: DateTimeOffset): NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent eventData) -> NewStreamEvent { eventData with timestamp = timestamp }

    let withCorrelationId (correlationId: Guid): NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent eventData) ->
            NewStreamEvent
                { eventData with
                      correlationId = correlationId }

    let withCausationId (causationId: Guid): NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent eventData) ->
            NewStreamEvent
                { eventData with
                      causationId = Some causationId }

    let withMetadata (metadata: string): NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent eventData) ->
            NewStreamEvent
                { eventData with
                      metadata = Some metadata }

[<Struct>]
type StreamEvent<'a> =
    { getData: unit -> AsyncResult<'a, exn>
      author: string
      id: Guid
      timestamp: DateTimeOffset
      correlationId: Guid
      causationId: Guid option
      metadata: string option
      position: int64
      streamVersion: int
      streamId: string }

module StreamEvent =
    let ofStreamMessage (msg: StreamMessage): StreamEvent<'a> =
        let meta =
            JayJson.decode<Metadata> msg.JsonMetadata

        let getData () =
            asyncResult {
                let! json = msg.GetJsonData()
                return JayJson.decode<'a> json
            }

        { author = meta.author
          getData = getData
          id = msg.MessageId
          timestamp = meta.timestamp
          correlationId = meta.correlationId
          causationId = meta.causationId
          metadata = meta.meta
          position = msg.Position
          streamVersion = msg.StreamVersion
          streamId = msg.StreamId }
