#!/bin/bash
set -e

# This script is run in two ways:
#
# 1. Automatically by the postgres docker image when it creates the database.
# 2. Manually by use in Makefile, to create new schemas people might have added.
#
# In the latter case we pass "update" as an argument and do an extra check:
# If the database isn’t ready for connections yet, skip the whole thing. That happens
# when building the postgres service from scratch (no volume). In that case, the
# postgres docker image will run this script for us anyway (case #1 above).
if test "$1" = "update" && ! (echo >/dev/tcp/localhost/5432) 2>/dev/null; then
    exit
fi

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE SCHEMA IF NOT EXISTS test;
EOSQL
