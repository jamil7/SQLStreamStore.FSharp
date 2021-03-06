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
    /// Creates a NewStreamEvent with the following defaults:
    /// id = Guid.NewGuid()
    /// timestamp = DateTimeOffset.Now
    /// correlationId = Guid.NewGuid()
    /// causationId = None
    /// metadata = None
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
        fun (NewStreamEvent event) -> NewStreamEvent { event with id = id }

    let withTimestamp (timestamp: DateTimeOffset): NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent event) -> NewStreamEvent { event with timestamp = timestamp }

    let withCorrelationId (correlationId: Guid): NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent event) ->
            NewStreamEvent
                { event with
                      correlationId = correlationId }

    let withCausationId (causationId: Guid): NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent event) ->
            NewStreamEvent
                { event with
                      causationId = Some causationId }

    let withMetadata (metadata: string): NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent event) -> NewStreamEvent { event with metadata = Some metadata }

    let internal toNewStreamMessage: NewStreamEvent<'a> -> NewStreamMessage =
        fun (NewStreamEvent event) ->
            let metadata: Metadata =
                { timestamp = event.timestamp
                  author = event.author
                  causationId = event.causationId
                  correlationId = event.correlationId
                  meta = event.metadata }

            let unionToString: 'a -> string =
                fun a ->
                    Reflection.FSharpValue.GetUnionFields(a, typeof<'a>)
                    |> fst
                    |> fun case -> case.Name

            NewStreamMessage
                (event.id,
                 "Event::" + unionToString event.data,
                 Serializer.serialize event.data,
                 Serializer.serialize metadata)

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
            Serializer.deserialize<Metadata> msg.JsonMetadata

        let getData () =
            asyncResult {
                let! json = msg.GetJsonData()
                return Serializer.deserialize<'a> json
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
