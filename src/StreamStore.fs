namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore

type PostgresConfig =
    {
        host: string
        port: string
        username: string
        password: string
        database: string
    }
    member this.ToConnectionString(?maxPoolSize) : string =
        let maxPoolSize' = defaultArg maxPoolSize "10"

        sprintf
            "Host=%s;Port=%s;Username=%s;Password=%s;Database=%s;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=%s"
            this.host
            this.port
            this.username
            this.password
            this.database
            maxPoolSize'

type PostgresStoreOptions =
    | Schema of string
    | CreateSchemaIfNotExists

module NewStreamStore =

    /// Represents an in-memory implementation of a stream store. Use for testing or high/speed + volatile scenarios.
    let inMemoryStore : unit -> InMemoryStreamStore = fun _ -> new InMemoryStreamStore()

    let postgresStore'
        (config: PostgresConfig)
        (postgresStoreOptions: PostgresStoreOptions list)
        : PostgresStreamStore =

        let mutable schema = None
        let mutable createSchemaIfNotExists = false

        postgresStoreOptions
        |> List.iter
            (function
            | Schema s -> schema <- Some s
            | CreateSchemaIfNotExists -> createSchemaIfNotExists <- true)

        let storeSettings =
            let settings =
                PostgresStreamStoreSettings(config.ToConnectionString())

            match schema with
            | None -> settings
            | Some schema' ->
                settings.Schema <- schema'
                settings

        new PostgresStreamStore(storeSettings)

    let postgresStore (config: PostgresConfig) = postgresStore' config []

    let createSchemaIfNotExists (store: PostgresStreamStore) : AsyncResult<IStreamStore, exn> =
        asyncResult {
            do! store.CreateSchemaIfNotExists()
            return store :> IStreamStore
        }
