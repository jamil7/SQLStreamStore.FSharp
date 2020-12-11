namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.FSharp.Types
open SqlStreamStore.Streams

[<AbstractClass>]
type AbstractController<'position>() =
    abstract isEnd: ?readDirection:ReadDirection * ?position:'position * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<bool, exn>

    abstract messages: ?readDirection:ReadDirection * ?position:'position * ?messageCount:int * ?prefetch:bool
     -> StreamMessages

[<AbstractClass>]
type AbstractAllStreamController() =
    inherit AbstractController<StartPosition>()

    abstract direction: ?readDirection:ReadDirection * ?startPosition:StartPosition * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<ReadDirection, exn>

    abstract fromPosition: ?readDirection:ReadDirection * ?startPosition:StartPosition * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<int64, exn>

    abstract nextPosition: ?readDirection:ReadDirection * ?startPosition:StartPosition * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<int64, exn>

[<AbstractClass>]
type AbstractStreamController() =
    inherit AbstractController<ReadVersion>()

    abstract append: MessageDetails list * AppendVersion option -> AsyncResult<AppendResult, exn>

    abstract readDirection: ?readDirection:ReadDirection * ?readVersion:ReadVersion * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<ReadDirection, exn>

    abstract streamId: ?readDirection:ReadDirection * ?readVersion:ReadVersion * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<string, exn>

    abstract fromStreamVersion: ?readDirection:ReadDirection * ?readVersion:ReadVersion * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<int, exn>

    abstract lastStreamPosition: ?readDirection:ReadDirection * ?readVersion:ReadVersion * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<int64, exn>

    abstract lastStreamVersion: ?readDirection:ReadDirection * ?readVersion:ReadVersion * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<int, exn>

    abstract nextStreamVersion: ?readDirection:ReadDirection * ?readVersion:ReadVersion * ?messageCount:int * ?prefetch:bool
     -> AsyncResult<int, exn>
