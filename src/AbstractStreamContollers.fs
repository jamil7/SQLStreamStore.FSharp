namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.FSharp.Types
open SqlStreamStore.Streams

[<AbstractClass>]
type AbstractAllStreamController<'position>() =
    abstract direction: ?readDirection:ReadDirection * ?startPosition:StartPosition * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<ReadDirection, exn>

    abstract fromPosition: ?readDirection:ReadDirection * ?startPosition:StartPosition * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<int64, exn>
     
     abstract isEnd: ?readDirection:ReadDirection * ?startPosition:StartPosition * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<bool, exn>
     
     abstract nextPosition: ?readDirection:ReadDirection * ?startPosition:StartPosition * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<int64, exn>
     
     abstract messages: ?readDirection:ReadDirection * ?startPosition:StartPosition * ?messageCount:int * ?prefetch:bool
     -> StreamMessages    

[<AbstractClass>]
type AbstractStreamController<'position>() =
    inherit AbstractAllStreamController<'position>()

    abstract append: MessageDetails list * AppendVersion option -> AsyncResult<AppendResult, exn>
