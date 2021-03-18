namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore

type PostgresConfig =
    { host: string
      port: string
      username: string
      password: string
      database: string }
    member this.toConnectionString(?maxPoolSize): string =
        let maxPoolSize' = defaultArg maxPoolSize "10"

        sprintf
            "Host=%s;Port=%s;Username=%s;Password=%s;Database=%s;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=%s"
            this.host
            this.port
            this.username
            this.password
            this.database
            maxPoolSize'

//type NewStreamStore =
//    /// Represents an in-memory implementation of a stream store. Use for testing or high/speed + volatile scenarios.
//    static member inMemoryStore() = new InMemoryStreamStore()
//
//    /// Connect to a Postgres Database.
//    /// Defaults: schema = public, createSchemaIfNotExists = true
//    static member postgresStore(config: PostgresConfig, ?schema: string, ?createSchemaIfNotExists: bool) =
//        let createSchemaIfNotExists' = defaultArg createSchemaIfNotExists true
//
//        let storeSettings =
//            let settings =
//                PostgresStreamStoreSettings(config.toConnectionString ())
//
//            match schema with
//            | None -> settings
//            | Some schema' ->
//                settings.Schema <- schema'
//                settings
//
//        let store = new PostgresStreamStore(storeSettings)
//
//        asyncResult {
//            if createSchemaIfNotExists' then do! store.CreateSchemaIfNotExists() else ()
//
//            return store :> IStreamStore
//        }

type private NewStreamStoreInternal =
    { config: PostgresConfig
      schema: string option
      createSchemaIfNotExists: bool }

type NewStreamStore = private NewStreamStore of NewStreamStoreInternal

module NewStreamStore =

    /// Represents an in-memory implementation of a stream store. Use for testing or high/speed + volatile scenarios.
    let inMemoryStore () = new InMemoryStreamStore()

    /// Connect to a Postgres Database.
    let postgresStore (config: PostgresConfig): NewStreamStore -> NewStreamStore =
        fun (NewStreamStore store) ->
            NewStreamStore
                { store with
                      config = config
                      createSchemaIfNotExists = false }

    let withSchema (schema: string): NewStreamStore -> NewStreamStore =
        fun (NewStreamStore store) -> NewStreamStore { store with schema = Some schema }

    let withCreateSchemaIfNotExists: NewStreamStore -> NewStreamStore =
        fun (NewStreamStore store) ->
            NewStreamStore
                { store with
                      createSchemaIfNotExists = true }

    let connect: NewStreamStore -> AsyncResult<IStreamStore, exn> =
        fun (NewStreamStore store) ->
            let storeSettings =
                let settings =
                    PostgresStreamStoreSettings(store.config.toConnectionString ())

                match store.schema with
                | None -> settings
                | Some schema' ->
                    settings.Schema <- schema'
                    settings

            let store' = new PostgresStreamStore(storeSettings)

            asyncResult {
                if store.createSchemaIfNotExists then do! store'.CreateSchemaIfNotExists() else ()

                return store' :> IStreamStore
            }
