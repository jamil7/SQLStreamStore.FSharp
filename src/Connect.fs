namespace SqlStreamStore.FSharp

open SqlStreamStore

type private StreamInternal =
    {
        store: IStreamStore
        streamId: string
    }

type Stream = private Stream of StreamInternal

module Connect =
    let toStream streamId store =
        Stream { streamId = streamId; store = store }
