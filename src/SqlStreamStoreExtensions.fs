namespace SqlStreamStore.FSharp

open System.Threading
open FSharp.Prelude
open SqlStreamStore
open SqlStreamStore.Streams
open System

[<AutoOpen>]
module SqlStreamExtensions =

    // StreamMessage extensions

    let private getJsonData (streamMessage: StreamMessage) =
        asyncResult { return! streamMessage.GetJsonData() }

    let private getJsonDataAs (streamMessage: StreamMessage) =
        asyncResult { return! streamMessage.GetJsonDataAs() }

    type StreamMessage with
        member this.GetJsonData() = getJsonData this
        member this.GetJsonDataAs() = getJsonDataAs this

    // IStreamStore extensions

    let private listAllStreams (store: IStreamStore) (maxCount: int) (continuationToken: string) =
        asyncResult { return! store.ListStreams(maxCount, continuationToken) }

    let private readHeadPosition (store: IStreamStore) token =
        asyncResult { return! store.ReadHeadPosition(token) }

    let private appendToStream (store: IStreamStore) streamId expectedVersion messages token =
        asyncResult { return! store.AppendToStream(StreamId streamId, expectedVersion, messages, token) }

    type IStreamStore with

        ///Appends a collection of messages to a stream.
        member this.AppendToStream(streamId, expectedVersion, messages, ?cancellationToken) =
            let cancellationToken' =
                defaultArg cancellationToken Unchecked.defaultof<CancellationToken>

            appendToStream this streamId expectedVersion messages cancellationToken'


        /// Reads the head position (the position of the very latest message).
        member this.ReadHeadPosition(?cancellationToken) =
            let cancellationToken' =
                defaultArg cancellationToken Unchecked.defaultof<CancellationToken>

            asyncResult { return! readHeadPosition this cancellationToken' }

        /// Lists Streams in SQL Stream Store.
        /// Defaults: maxCount = 1000, continuationToken = null
        member this.ListStreams(?maxCount: int, ?continuationToken: string) =
            let maxCount' = defaultArg maxCount 1000

            let continuationToken' = defaultArg continuationToken null

            asyncResult { return! listAllStreams this maxCount' continuationToken' }

        /// List all the streams in the stream store.
        /// N(streams_that_can_be_retrieved) <= Int.Max
        /// Defaults: continuationToken = null
        member this.ListAllStreams(?continuationToken: string) =
            let continuationToken' = defaultArg continuationToken null

            this.ListStreams(Int32.MaxValue, continuationToken')
