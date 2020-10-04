namespace SqlStreamStore.FSharp.Postgres

open SqlStreamStore

type PostgresConfig =
    { host: string
      port: string
      username: string
      password: string
      database: string }

module Postgres =
    let createStore: PostgresConfig -> PostgresStreamStore =
        fun config ->

            let storeSettings: string =
                sprintf
                    "Host=%s;Port=%s;User Id=%s;Password=%s;Database=%s"
                    config.host
                    config.port
                    config.username
                    config.password
                    config.database

            new PostgresStreamStore(PostgresStreamStoreSettings(storeSettings))

    let createSchema: PostgresStreamStore -> Async<unit> =
        fun conn -> conn.CreateSchemaIfNotExists() |> Async.AwaitTask
