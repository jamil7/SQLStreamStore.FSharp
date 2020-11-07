namespace SqlStreamStore.FSharp.Benchmarks

open SqlStreamStore
open SqlStreamStore.FSharp

module PrefetchVsNoPrefetch =
    let private getMessages (readAllPage: Streams.ReadAllPage): string [] =
        readAllPage.Messages
        |> Array.map
            ((fun msg -> msg.GetJsonData())
             >> Async.awaitTaskWithInnerException
             >> Async.RunSynchronously)

    let getMessagesNoPrefetch (store: IStreamStore): Async<string []> =
        async {
            let! readAllPage = ReadRaw.allForwards store StartPosition.Any 10000000

            return getMessages readAllPage
        }

    let getMessagesPrefetch (store: IStreamStore): Async<string []> =
        async {
            let! readAllPage = ReadRaw.allForwardsPrefetch store StartPosition.Any 10000000

            return getMessages readAllPage
        }
