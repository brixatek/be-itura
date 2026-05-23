-- Create schemas for each service
CREATE SCHEMA IF NOT EXISTS itura_auth;
CREATE SCHEMA IF NOT EXISTS itura_users;
CREATE SCHEMA IF NOT EXISTS itura_coaching;
CREATE SCHEMA IF NOT EXISTS itura_payments;
CREATE SCHEMA IF NOT EXISTS itura_wellness;
CREATE SCHEMA IF NOT EXISTS itura_community;
CREATE SCHEMA IF NOT EXISTS itura_notifications;
CREATE SCHEMA IF NOT EXISTS itura_corporate;
CREATE SCHEMA IF NOT EXISTS itura_content;
CREATE SCHEMA IF NOT EXISTS itura_gamification;
CREATE SCHEMA IF NOT EXISTS itura_analytics;

-- Enable extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";    -- for fuzzy search
CREATE EXTENSION IF NOT EXISTS "btree_gin";  -- for composite GIN indexes
