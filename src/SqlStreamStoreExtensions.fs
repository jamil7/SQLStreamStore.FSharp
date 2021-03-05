namespace SqlStreamStore.FSharp.EventSourcing

open FSharp.Prelude
open SqlStreamStore.FSharp
open SqlStreamStore.Streams

[<AutoOpen>]
module EventSourcingExtensions =
    let private filterEvents<'a> (messages : StreamMessage []) =
        messages
        |> Array.filter (fun msg -> msg.Type.Contains ("Event::"))
        |> Array.toList

    type StreamSlice with
        member this.events<'a>() =
            asyncResult {
                let! page = this.ReadStreamPage
                return filterEvents<'a> page.Messages
            }
            |> StreamEvents<'a>

    type AllStreamSlice with
        member this.events<'a>() =
            asyncResult {
                let! page = this.ReadAllPage
                return filterEvents<'a> page.Messages
            }
            |> StreamEvents<'a>

    type Stream with
        member this.appendEvents(events : NewStreamEvent<'a> list, ?expectedVersion : int) =
            let expectedVersion' =
                defaultArg expectedVersion ExpectedVersion.Any

            let messages =
                events
                |> List.map (fun event -> event.toStreamMessage ())

            this.appendMessages (messages, expectedVersion')
