namespace SqlStreamStore.FSharp.Postgres

open Prelude
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

module Create =

    let postgresStore (config: PostgresConfig) (schema: string option) : PostgresStreamStore =

        let storeSettings =
            let settings =
                PostgresStreamStoreSettings(config.ToConnectionString())

            match schema with
            | None -> settings
            | Some schema' ->
                settings.Schema <- schema'
                settings

        new PostgresStreamStore(storeSettings)

    let schemaIfNotExists (store: PostgresStreamStore) : AsyncResult<IStreamStore, exn> =
        asyncResult {
            do! store.CreateSchemaIfNotExists()
            return store :> IStreamStore
        }
