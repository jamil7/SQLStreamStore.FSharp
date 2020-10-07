namespace SqlStreamStore.FSharp.Postgres

type PostgresConfig =
    { host: string
      port: string
      username: string
      password: string
      database: string }

module Postgres =
    let createStore: PostgresConfig -> SqlStreamStore.PostgresStreamStore =
        fun config ->

            let storeSettings: string =
                sprintf
                    "Host=%s;Port=%s;User Id=%s;Password=%s;Database=%s"
                    config.host
                    config.port
                    config.username
                    config.password
                    config.database

            new SqlStreamStore.PostgresStreamStore(SqlStreamStore.PostgresStreamStoreSettings(storeSettings))

    let createSchema: SqlStreamStore.PostgresStreamStore -> Async<unit> =
        fun conn -> conn.CreateSchemaIfNotExists() |> Async.AwaitTask
