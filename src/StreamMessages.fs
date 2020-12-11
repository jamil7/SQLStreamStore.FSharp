namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.Streams

type StreamMessages(messages: AsyncResult<StreamMessage list, exn>) =
    member this.filter(predicate: StreamMessage -> bool) =
        let messages' =
            asyncResult {
                let! messages' = messages
                return List.filter predicate messages'
            }

        StreamMessages(messages')

    member this.length() =
        asyncResult {
            let! messages' = messages
            return messages'.Length
        }

    member this.JsonData() =
        asyncResult {
            let! messages' = messages

            return! messages'
                    |> List.map (fun msg -> msg.GetJsonData)
                    |> List.map AsyncResult.ofTask
                    |> AsyncResult.sequence
        }
