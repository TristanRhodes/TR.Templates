-- Load Extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create Tables

CREATE TABLE items ( 
  item_id uuid NOT NULL DEFAULT uuid_generate_v4 (),
  name VARCHAR(255) NOT NULL,
  description VARCHAR(255) NOT NULL,
  PRIMARY KEY(item_id)
  );