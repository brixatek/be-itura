#!/usr/bin/env bash
# Creates one database per service. Runs automatically on first postgres start
# because Docker mounts this into /docker-entrypoint-initdb.d/
set -e

DATABASES=(
  itura_auth
  itura_users
  itura_ai
  itura_mood
  itura_journal
  itura_community
  itura_coach
  itura_booking
  itura_payment
  itura_notification
  itura_content
  itura_media
  itura_corporate
  itura_gamification
  itura_analytics
  itura_search
)

for db in "${DATABASES[@]}"; do
  echo "Creating database: $db"
  psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname postgres <<-SQL
    SELECT 'CREATE DATABASE $db'
    WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = '$db')\gexec

    \c $db
    CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
    CREATE EXTENSION IF NOT EXISTS "pg_trgm";
SQL
done

echo "All databases ready."
