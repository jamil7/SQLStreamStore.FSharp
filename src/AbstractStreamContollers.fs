namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.FSharp.Types
open SqlStreamStore.Streams

[<AbstractClass>]
type AbstractAllStreamController<'position>() =
    abstract filteredMessagesJsonData: (StreamMessage -> bool) * ?messageCount:int * ?position:'position
     -> AsyncResult<string list, exn>

    abstract length: ?messageCount:int * ?position:'position -> AsyncResult<int64, exn>
    abstract messagesJsonData: ?messageCount:int * ?position:'position -> AsyncResult<string list, exn>


[<AbstractClass>]
type AbstractStreamController<'position>() =
    inherit AbstractAllStreamController<'position>()
    abstract append: MessageDetails list * AppendVersion option -> AsyncResult<AppendResult, exn>
