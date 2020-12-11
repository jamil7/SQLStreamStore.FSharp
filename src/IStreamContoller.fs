namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.FSharp.Types
open SqlStreamStore.Streams

type IAllStreamController<'position> =
    abstract length: ?messageCount:int * ?position:'position -> AsyncResult<int64, exn>
    abstract messagesJsonData: ?messageCount:int * ?position:'position -> AsyncResult<string list, exn>

type IStreamController<'position> =
    inherit IAllStreamController<'position>
    abstract append: MessageDetails list * AppendVersion option -> AsyncResult<AppendResult, exn>
