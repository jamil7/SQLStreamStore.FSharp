namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams
open SqlStreamStore.FSharp.Types
open FSharp.Prelude

type InitConfig =
    { store: SqlStreamStore.IStreamStore
      stream: string
      readDirection: ReadDirection option
      readVersion: ReadVersion option
      messageCount: int option
      prefetch: bool option }

module SController =
    let init store stream =
        { store = store
          stream = stream
          readDirection = None
          readVersion = None
          messageCount = None
          prefetch = None }

module With =
    let readDirection (readDirection: ReadDirection) (initConfig: InitConfig): InitConfig =
        { initConfig with
              readDirection = Some readDirection }

    let readVersion (readVersion: ReadVersion) (initConfig: InitConfig): InitConfig =
        { initConfig with
              readVersion = Some readVersion }

    let messageCount (messageCount: int) (initConfig: InitConfig): InitConfig =
        { initConfig with
              messageCount = Some messageCount }

    let prefetch (initConfig: InitConfig): InitConfig = { initConfig with prefetch = Some true }

module SMessage =
    let streamReadMatcher direction prefetch store stream readVersion messageCount =
        match direction, prefetch with
        | (ReadDirection.Forward, false) -> ReadStream.forwards store stream readVersion messageCount
        | (ReadDirection.Backward, false) -> ReadStream.backwards store stream readVersion messageCount
        | (ReadDirection.Forward, true) -> ReadStream.forwardsPrefetch store stream readVersion messageCount
        | (ReadDirection.Backward, true) -> ReadStream.backwardsPrefetch store stream readVersion messageCount
        | _ -> Async.singleton (Error IllegalArgumentException)

    let readPager (initConfig: InitConfig): AsyncResult<ReadStreamPage, exn> =
        let readDirection =
            Option.defaultValue ReadDirection.Forward initConfig.readDirection

        let readVersion =
            Option.defaultValue ReadVersion.Any initConfig.readVersion

        let messageCount =
            Option.defaultValue 1000 initConfig.messageCount

        let prefetch =
            Option.defaultValue false initConfig.prefetch

        streamReadMatcher readDirection prefetch initConfig.store initConfig.stream readVersion messageCount

    let isEnd initConfig: AsyncResult<bool, exn> =
        asyncResult {
            let! readPage = readPager initConfig
            return readPage.IsEnd
        }


    let messages initConfig: AsyncResult<StreamMessage list, exn> =
        asyncResult {
            let! readPage = readPager initConfig
            return List.ofArray readPage.Messages
        }

module Test =
    let store = new SqlStreamStore.InMemoryStreamStore()

    let caseMessages =
        SController.init store "Case::1"
        |> With.prefetch
        |> With.messageCount 200
        |> With.readVersion ReadVersion.Any
        |> SMessage.messages
