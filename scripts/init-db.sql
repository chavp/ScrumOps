-- Database initialization script for ScrumOps PostgreSQL
-- This script creates the schemas for the different bounded contexts

-- Create schemas for each bounded context
CREATE SCHEMA IF NOT EXISTS "TeamManagement";
CREATE SCHEMA IF NOT EXISTS "ProductBacklog";  
CREATE SCHEMA IF NOT EXISTS "SprintManagement";

-- Grant permissions to the scrumops user
GRANT ALL PRIVILEGES ON SCHEMA "TeamManagement" TO scrumops;
GRANT ALL PRIVILEGES ON SCHEMA "ProductBacklog" TO scrumops;
GRANT ALL PRIVILEGES ON SCHEMA "SprintManagement" TO scrumops;

-- Grant usage on all schemas
GRANT USAGE ON SCHEMA "TeamManagement" TO scrumops;
GRANT USAGE ON SCHEMA "ProductBacklog" TO scrumops;
GRANT USAGE ON SCHEMA "SprintManagement" TO scrumops;

-- Set default privileges for future tables
ALTER DEFAULT PRIVILEGES IN SCHEMA "TeamManagement" GRANT ALL ON TABLES TO scrumops;
ALTER DEFAULT PRIVILEGES IN SCHEMA "ProductBacklog" GRANT ALL ON TABLES TO scrumops;
ALTER DEFAULT PRIVILEGES IN SCHEMA "SprintManagement" GRANT ALL ON TABLES TO scrumops;

-- Set default privileges for sequences
ALTER DEFAULT PRIVILEGES IN SCHEMA "TeamManagement" GRANT ALL ON SEQUENCES TO scrumops;
ALTER DEFAULT PRIVILEGES IN SCHEMA "ProductBacklog" GRANT ALL ON SEQUENCES TO scrumops;
ALTER DEFAULT PRIVILEGES IN SCHEMA "SprintManagement" GRANT ALL ON SEQUENCES TO scrumops;