namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.FSharp.EventSourcing
open SqlStreamStore.Streams

[<AutoOpen>]
module SqlStreamExtensions =
    let private getJsonData (streamMessage: StreamMessage) =
        asyncResult { return! streamMessage.GetJsonData() }

    let private getJsonDataAs (streamMessage: StreamMessage) =
        asyncResult { return! streamMessage.GetJsonDataAs() }

    type StreamMessage with
        member this.GetJsonData() = getJsonData this
        member this.GetJsonDataAs() = getJsonDataAs this
