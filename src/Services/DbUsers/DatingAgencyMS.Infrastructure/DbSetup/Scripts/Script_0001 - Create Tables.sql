CREATE TABLE IF NOT EXISTS Keys
(
    id            SERIAL,
    login         VARCHAR(50)  NOT NULL,
    password_hash VARCHAR(200) NOT NULL,
    password_salt VARCHAR(200) NOT NULL,
    role          VARCHAR(50)  NOT NULL,
    PRIMARY KEY (id)
)