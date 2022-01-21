namespace SqlStreamStore.FSharp

open Prelude.ErrorHandling
open SqlStreamStore
open SqlStreamStore.Streams
open SqlStreamStore.Subscriptions
open System.Threading
open System.Threading.Tasks

[<RequireQualifiedAccess>]
type StreamSubOption =
    | HasCaughtUp of (bool -> unit)
    | MaxCountPerRead of int
    | NoPrefetch
    | SubscriptionDropped of (IStreamSubscription -> SubscriptionDroppedReason -> exn -> unit)

[<RequireQualifiedAccess>]
type AllStreamSubOption =
    | HasCaughtUp of (bool -> unit)
    | MaxCountPerRead of int
    | NoPrefetch
    | SubscriptionDropped of (IAllStreamSubscription -> SubscriptionDroppedReason -> exn -> unit)

module Subscribe =
    let toStreamMessages'
        (subscriptionName: string)
        (continueAfterVersion: int)
        (streamMessageReceived: IStreamSubscription -> StreamMessage -> CancellationToken -> AsyncResult<_, _>)
        (streamSubOptions: StreamSubOption list)
        (stream: Stream)
        : IStreamSubscription =

        let mutable hasCaughtUp = null
        let mutable maxCountPerRead = 10
        let mutable prefetch = true
        let mutable subscriptionDropped = null

        let streamMessageReceived' =
            let subs: IStreamSubscription -> StreamMessage -> CancellationToken -> Task =
                fun iStreamSubscription msg cancellationToken ->
                    streamMessageReceived iStreamSubscription msg cancellationToken
                    |> Async.StartImmediateAsTask
                    :> Task

            StreamMessageReceived subs

        streamSubOptions
        |> List.iter
            (function
            | StreamSubOption.HasCaughtUp f -> hasCaughtUp <- HasCaughtUp f
            | StreamSubOption.MaxCountPerRead n -> maxCountPerRead <- n
            | StreamSubOption.NoPrefetch -> prefetch <- false
            | StreamSubOption.SubscriptionDropped f -> subscriptionDropped <- SubscriptionDropped f)

        let sub =
            stream.store.SubscribeToStream(
                streamId = StreamId stream.streamId,
                continueAfterVersion = System.Nullable continueAfterVersion,
                streamMessageReceived = streamMessageReceived',
                subscriptionDropped = subscriptionDropped,
                hasCaughtUp = hasCaughtUp,
                prefetchJsonData = prefetch,
                name = subscriptionName
            )

        sub.MaxCountPerRead <- maxCountPerRead
        sub

    let toStreamMessages
        (subscriptionName: string)
        (continueAfterVersion: int)
        (streamMessageReceived: IStreamSubscription -> StreamMessage -> CancellationToken -> AsyncResult<_, _>)
        : Stream -> IStreamSubscription =
        toStreamMessages' subscriptionName continueAfterVersion streamMessageReceived []

    let toAllStreamMessages'
        (subscriptionName: string)
        (continueAfterPosition: int64)
        (streamMessageReceived: IAllStreamSubscription -> StreamMessage -> CancellationToken -> AsyncResult<_, _>)
        (streamSubOptions: AllStreamSubOption list)
        : IStreamStore -> IAllStreamSubscription =

        let mutable hasCaughtUp = null
        let mutable maxCountPerRead = 10
        let mutable prefetch = true
        let mutable subscriptionDropped = null

        let streamMessageReceived' =
            let subs: IAllStreamSubscription -> StreamMessage -> CancellationToken -> Task =
                fun iAllStreamSubscription msg cancellationToken ->
                    streamMessageReceived iAllStreamSubscription msg cancellationToken
                    |> Async.StartImmediateAsTask
                    :> Task

            AllStreamMessageReceived subs

        streamSubOptions
        |> List.iter
            (function
            | AllStreamSubOption.HasCaughtUp f -> hasCaughtUp <- HasCaughtUp f
            | AllStreamSubOption.MaxCountPerRead n -> maxCountPerRead <- n
            | AllStreamSubOption.NoPrefetch -> prefetch <- false
            | AllStreamSubOption.SubscriptionDropped f -> subscriptionDropped <- AllSubscriptionDropped f)

        fun store ->
            let sub =
                store.SubscribeToAll(
                    continueAfterPosition = System.Nullable continueAfterPosition,
                    streamMessageReceived = streamMessageReceived',
                    subscriptionDropped = subscriptionDropped,
                    hasCaughtUp = hasCaughtUp,
                    prefetchJsonData = prefetch,
                    name = subscriptionName
                )

            sub.MaxCountPerRead <- maxCountPerRead
            sub

    let toAllStreamMessages
        (subscriptionName: string)
        (continueAfterPosition: int64)
        (streamMessageReceived: IAllStreamSubscription -> StreamMessage -> CancellationToken -> AsyncResult<_, _>)
        : IStreamStore -> IAllStreamSubscription =

        toAllStreamMessages' subscriptionName continueAfterPosition streamMessageReceived []
