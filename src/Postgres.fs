namespace SqlStreamStore.FSharp.Postgres

open SqlStreamStore

type PostgresConfig =
    { host: string
      port: string
      username: string
      password: string
      database: string }

type PostgresConnection = PostgresStreamStore

module Postgres =
    let createStore: PostgresConfig -> PostgresConnection =
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

    let createSchema: PostgresConnection -> Async<unit> =
        fun conn -> conn.CreateSchemaIfNotExists() |> Async.AwaitTask
