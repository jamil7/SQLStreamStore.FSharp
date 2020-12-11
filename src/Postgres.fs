namespace SqlStreamStore.FSharp.Postgres

open FSharp.Prelude

type PostgresConfig =
    { host: string
      port: string
      username: string
      password: string
      database: string
      schema: string option }

module Postgres =
    let private storeSettings (config: PostgresConfig): string =
        sprintf
            "Host=%s;Port=%s;User Id=%s;Password=%s;Database=%s"
            config.host
            config.port
            config.username
            config.password
            config.database

    let private setSettingsSchema (config: PostgresConfig)
                                  (settings: SqlStreamStore.PostgresStreamStoreSettings)
                                  : SqlStreamStore.PostgresStreamStoreSettings =
        match config.schema with
        | None -> ()
        | Some (schema) -> settings.Schema <- schema

        settings

    let createStore (config: PostgresConfig): AsyncResult<SqlStreamStore.IStreamStore, exn> =
        let settings =
            SqlStreamStore.PostgresStreamStoreSettings(storeSettings config)
            |> setSettingsSchema config

        let store =
            new SqlStreamStore.PostgresStreamStore(settings)

        asyncResult {
            do! store.CreateSchemaIfNotExists()
            return store :> SqlStreamStore.IStreamStore
        }
