namespace SqlStreamStore.FSharp

open System.Threading
open System.Threading.Tasks
open FSharp.Prelude
open SqlStreamStore
open SqlStreamStore.Streams
open System
open SqlStreamStore.Subscriptions

[<AutoOpen>]
module SqlStreamExtensions =

    // StreamMessage extensions

    let private getJsonData (streamMessage: StreamMessage) =
        asyncResult { return! streamMessage.GetJsonData() }

    let private getJsonDataAs<'a> (streamMessage: StreamMessage) =
        asyncResult {
            let! json = getJsonData streamMessage
            return Serializer.deserialize<'a> json
        }

    type StreamMessage with
        /// Gets the Json Data of the message. If prefetch is enabled, this will be a fast operation.
        member this.GetJsonData() = getJsonData this

        /// Deserializes the json data using the bundled json serializer.
        member this.GetJsonDataAs<'a>() = getJsonDataAs<'a> this

    // IStreamStore extensions

    let private listAllStreams (store: IStreamStore) (maxCount: int) (continuationToken: string) =
        asyncResult { return! store.ListStreams(maxCount, continuationToken) }

    let private readHeadPosition (store: IStreamStore) cancellationToken =
        asyncResult { return! store.ReadHeadPosition cancellationToken }

    let private appendToStream (store: IStreamStore) streamId expectedVersion messages cancellationToken =
        asyncResult { return! store.AppendToStream(StreamId streamId, expectedVersion, messages, cancellationToken) }

    let private deleteMessage (store: IStreamStore) streamId msgId cancellationToken =
        asyncResult { return! store.DeleteMessage(StreamId streamId, msgId, cancellationToken) }

    let private deleteStream (store: IStreamStore) streamId version cancellationToken =
        asyncResult { return! store.DeleteStream(StreamId streamId, version, cancellationToken) }

    let private getStreamMetadata (store: IStreamStore) streamId cancellationToken =
        asyncResult { return! store.GetStreamMetadata(streamId, cancellationToken) }

    let private readStreamForwards
        (store: IStreamStore)
        streamId
        fromVersionInclusive
        maxCount
        prefetchJsonData
        cancellationToken
        =
        asyncResult {
            return!
                store.ReadStreamForwards(
                    StreamId streamId,
                    fromVersionInclusive,
                    maxCount,
                    prefetchJsonData,
                    cancellationToken
                )
        }

    let private readStreamBackwards
        (store: IStreamStore)
        streamId
        fromVersionInclusive
        maxCount
        prefetchJsonData
        cancellationToken
        =
        asyncResult {
            return!
                store.ReadStreamBackwards(
                    StreamId streamId,
                    fromVersionInclusive,
                    maxCount,
                    prefetchJsonData,
                    cancellationToken
                )
        }

    let private setStreamMetadata
        (store: IStreamStore)
        streamId
        expectedStreamMetadataVersion
        maxAge
        maxCount
        metadataJson
        cancellationToken
        =
        asyncResult {
            return!
                store.SetStreamMetadata(
                    StreamId streamId,
                    expectedStreamMetadataVersion,
                    maxAge,
                    maxCount,
                    metadataJson,
                    cancellationToken
                )
        }

    type IStreamStore with

        /// Appends a collection of messages to a stream.
        member this.AppendToStream(streamId, expectedVersion, messages, ?cancellationToken) =
            let cancellationToken' =
                defaultArg cancellationToken Unchecked.defaultof<CancellationToken>

            appendToStream this streamId expectedVersion messages cancellationToken'

        /// Hard deletes a message from the stream.
        /// Deleting a message will result in a '$message-deleted' message being appended to the '$deleted' stream.
        /// See SqlStreamStore.Streams.Deleted.MessageDeleted for the message structure.
        member this.DeleteMessage(streamId, messageId, ?cancellationToken) =
            let cancellationToken' =
                defaultArg cancellationToken Unchecked.defaultof<CancellationToken>

            deleteMessage this streamId messageId cancellationToken'

        /// Hard deletes a stream and all of its messages.
        /// Deleting a stream will result in a '$stream-deleted' message being appended to the '$deleted' stream.
        /// See SqlStreamStore.Streams.Deleted.StreamDeleted for the message structure.
        member this.DeleteStream(streamId, expectedVersion, ?cancellationToken) =
            let cancellationToken' =
                defaultArg cancellationToken Unchecked.defaultof<CancellationToken>

            deleteStream this streamId expectedVersion cancellationToken'

        /// Gets the stream metadata.
        member this.GetStreamMetadata(streamId, ?cancellationToken) =
            let cancellationToken' =
                defaultArg cancellationToken Unchecked.defaultof<CancellationToken>

            getStreamMetadata this streamId cancellationToken'

        /// Sets the metadata for a stream.
        member this.SetStreamMetadata
            (
                streamId,
                ?expectedStreamMetadataVersion,
                ?maxAge: int,
                ?maxCount: int,
                ?metadataJson,
                ?cancellationToken
            ) =
            let expectedStreamMetadataVersion' =
                defaultArg expectedStreamMetadataVersion -2

            let maxAge' =
                match maxAge with
                | None -> Nullable()
                | Some age -> Nullable age

            let maxCount' =
                match maxCount with
                | None -> Nullable()
                | Some count -> Nullable count

            let metadataJson' = defaultArg metadataJson null

            let cancellationToken' =
                defaultArg cancellationToken Unchecked.defaultof<CancellationToken>

            setStreamMetadata
                this
                streamId
                expectedStreamMetadataVersion'
                maxAge'
                maxCount'
                metadataJson'
                cancellationToken'

        /// Reads the head position (the position of the very latest message).
        member this.ReadHeadPosition(?cancellationToken) =
            let cancellationToken' =
                defaultArg cancellationToken Unchecked.defaultof<CancellationToken>

            readHeadPosition this cancellationToken'

        /// Reads messages from a stream forwards.
        member this.ReadStreamForwards(streamId, ?fromVersionInclusive, ?maxCount, ?prefetch, ?cancellationToken) =
            let fromVersionInclusive' =
                defaultArg fromVersionInclusive StreamVersion.Start

            let maxCount' = defaultArg maxCount 1000

            let prefetch' = defaultArg prefetch true

            let cancellationToken' =
                defaultArg cancellationToken Unchecked.defaultof<CancellationToken>


            readStreamForwards this streamId fromVersionInclusive' maxCount' prefetch' cancellationToken'

        /// Reads messages from a stream backwards.
        member this.ReadStreamBackwards(streamId, ?fromVersionInclusive, ?maxCount, ?prefetch, ?cancellationToken) =
            let fromVersionInclusive' =
                defaultArg fromVersionInclusive StreamVersion.Start

            let maxCount' = defaultArg maxCount 1000

            let prefetch' = defaultArg prefetch true

            let cancellationToken' =
                defaultArg cancellationToken Unchecked.defaultof<CancellationToken>


            readStreamBackwards this streamId fromVersionInclusive' maxCount' prefetch' cancellationToken'

        /// Lists Streams in SQLStreamStore.
        /// Defaults: maxCount = 1000, continuationToken = null
        member this.ListStreams(?maxCount: int, ?continuationToken: string) =
            let maxCount' = defaultArg maxCount 1000

            let continuationToken' = defaultArg continuationToken null

            listAllStreams this maxCount' continuationToken'

        /// List all the streams in the stream store.
        /// N(streams_that_can_be_retrieved) <= Int.Max
        /// Defaults: continuationToken = null
        member this.ListAllStreams(?continuationToken: string) =
            let continuationToken' = defaultArg continuationToken null

            this.ListStreams(Int32.MaxValue, continuationToken')
