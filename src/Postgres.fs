namespace SqlStreamStore.FSharp.Postgres

type PostgresConfig =
    { host: string
      port: string
      username: string
      password: string
      database: string }

module Postgres =
    let createStore (config: PostgresConfig): SqlStreamStore.PostgresStreamStore =
        let storeSettings: string =
            sprintf
                "Host=%s;Port=%s;User Id=%s;Password=%s;Database=%s"
                config.host
                config.port
                config.username
                config.password
                config.database

        new SqlStreamStore.PostgresStreamStore(SqlStreamStore.PostgresStreamStoreSettings(storeSettings))

    let createSchema (store: SqlStreamStore.PostgresStreamStore): Async<unit> =
        store.CreateSchemaIfNotExists() |> Async.AwaitTask
