namespace SqlStreamStore.FSharp

open SqlStreamStore

type Stream =
    private
        {
            store: IStreamStore
            streamId: string
        }

module Connect =
    let toStream streamId store = { streamId = streamId; store = store }
