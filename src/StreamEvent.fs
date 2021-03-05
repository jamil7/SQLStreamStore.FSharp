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

type NewStreamEvent<'a>(author: string,
                        data: 'a,
                        ?id: Guid,
                        ?metadata: string,
                        ?timestamp: DateTimeOffset,
                        ?causationId: Guid,
                        ?correlationId: Guid,
                        ?encoder: 'a -> string) =

    let id' = defaultArg id (Guid.NewGuid())

    let timestamp' = defaultArg timestamp DateTimeOffset.Now

    let correlationId' =
        defaultArg correlationId (Guid.NewGuid())

    member this.toStreamMessage(): NewStreamMessage =
        let metadata': Metadata =
            { timestamp = timestamp'
              author = author
              causationId = causationId
              correlationId = correlationId'
              meta = metadata }

        let encode =
            match encoder with
            | None -> JayJson.encode
            | Some enc -> enc

        let unionToString: 'a -> string =
            fun a ->
                let (case, _) =
                    FSharp.Reflection.FSharpValue.GetUnionFields(a, typeof<'a>)

                case.Name

        NewStreamMessage(id', "Event::" + unionToString data, encode data, JayJson.encode metadata')
