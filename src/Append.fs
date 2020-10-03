namespace SqlStreamStore.FSharp

open SqlStreamStore
open SqlStreamStore.Streams

type NewMessage = NewStreamMessage
type Message = StreamMessage
type MessageDetails = {
    id : Id 
    event: string
    body: string
    metadata: string
}
and Id = Custom of System.Guid | Auto

module append =
        let appendMessage : MessageDetails -> unit  =
            fun msg ->
//                 let hi i = function | Custom guid -> guid | Auto -> System.Guid.NewGuid ()
                    
            failwith "hi"