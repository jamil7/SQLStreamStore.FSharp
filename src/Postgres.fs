namespace SqlStreamStore.FSharp.Postgres

open SqlStreamStore

type Config =
    { host: string
      port: string
      username: string
      password: string
      database: string }

type PostgresStore = PostgresStreamStore

module Postgres =
    let createStore: Config -> PostgresStore =
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

    let createSchema: PostgresStore -> Async<unit> =
        fun store -> store.CreateSchemaIfNotExists() |> Async.AwaitTask
