CREATE TABLE IF NOT EXISTS Keys
(
    id                 SERIAL,
    login              VARCHAR(50)  NOT NULL,
    password_hash      VARCHAR(200) NOT NULL,
    password_salt      VARCHAR(200) NOT NULL,
    encrypted_password VARCHAR(200) NOT NULL,
    role               VARCHAR(50)  NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS Clients
(
    id                   SERIAL,
    first_name           VARCHAR(50)  NOT NULL,
    last_name            VARCHAR(50)  NOT NULL,
    gender               VARCHAR(50)  NOT NULL,
    sex                  VARCHAR(50)  NOT NULL,
    sexual_orientation   VARCHAR(50)  NOT NULL,
    location             VARCHAR(50)  NOT NULL,
    registration_number  VARCHAR(50)  NOT NULL,
    registered_on        DATE         NOT NULL,
    age                  SMALLINT     NOT NULL,
    height               SMALLINT     NOT NULL,
    weight               SMALLINT     NOT NULL,
    zodiac_sign          VARCHAR(50)  NOT NULL,
    description          VARCHAR(255) NOT NULL,
    has_declined_service BOOLEAN DEFAULT FALSE,
    PRIMARY KEY (id),
    CONSTRAINT clients_age_check CHECK (age > 0),
    CONSTRAINT clients_height_check CHECK (height > 0),
    CONSTRAINT clients_weight_check CHECK (weight > 0)
);

CREATE TABLE IF NOT EXISTS PartnerRequirements
(
    requirement_id SERIAL,
    gender         VARCHAR(50),
    sex            VARCHAR(50),
    min_age        SMALLINT,
    max_age        SMALLINT,
    min_height     SMALLINT,
    max_height     SMALLINT,
    min_weight     SMALLINT,
    max_weight     SMALLINT,
    zodiac_sign    VARCHAR(50),
    location       VARCHAR(50),
    client_id      INTEGER,
    PRIMARY KEY (requirement_id),
    FOREIGN KEY (client_id) REFERENCES clients (id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT partnerRequirements_minAge_check CHECK (min_age < max_age),
    CONSTRAINT partnerRequirements_minHeight_check CHECK (min_height < max_height),
    CONSTRAINT partnerRequirements_minWeight_check CHECK (min_weight < max_weight)
);


CREATE TABLE IF NOT EXISTS Meetings
(
    meeting_id SERIAL,
    date       TIMESTAMP   NOT NULL,
    inviter_id INTEGER,
    invitee_id INTEGER,
    location   VARCHAR(50) NOT NULL,
    result     VARCHAR(50) NOT NULL,
    PRIMARY KEY (meeting_id),
    FOREIGN KEY (inviter_id) REFERENCES Clients (id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (invitee_id) REFERENCES Clients (id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS MeetingVisit
(
    id         SERIAL,
    client_id  INTEGER,
    meeting_id INTEGER,
    visited    BOOLEAN,
    PRIMARY KEY (id),
    FOREIGN KEY (client_id) REFERENCES clients (id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (meeting_id) REFERENCES Meetings (meeting_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS MeetingReview
(
    id             SERIAL,
    inviter_score  SMALLINT    NOT NULL,
    inviter_review VARCHAR(50) NOT NULL,
    invitee_score  SMALLINT    NOT NULL,
    invitee_review VARCHAR(50) NOT NULL,
    meeting_id     INTEGER,
    PRIMARY KEY (id),
    FOREIGN KEY (meeting_id) REFERENCES Meetings (meeting_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS CoupleArchive
(
    couple_archive_id SERIAL,
    first_client_id   INTEGER,
    second_client_id  INTEGER,
    couple_created_on DATE        NOT NULL,
    additional_info   VARCHAR(50) NOT NULL,
    archived_on       TIMESTAMP   NOT NULL,
    PRIMARY KEY (couple_archive_id),
    FOREIGN KEY (first_client_id) REFERENCES Clients (id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (second_client_id) REFERENCES Clients (id) ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE IF NOT EXISTS ClientRatings
(
    rating_id   SERIAL,
    client_id   INTEGER,
    rating      SMALLINT    NOT NULL,
    comment     VARCHAR(50) NOT NULL,
    rating_date TIMESTAMP   NOT NULL,
    PRIMARY KEY (rating_id),
    FOREIGN KEY (client_id) REFERENCES Clients (id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS Invitations
(
    invitation_id   SERIAL,
    inviter_id      INTEGER,
    invitee_id      INTEGER,
    location        VARCHAR(50) NOT NULL,
    date_of_meeting TIMESTAMP   NOT NULL,
    created_on      DATE        NOT NULL,
    active_to       DATE        NOT NULL,
    is_accepted     BOOLEAN     NOT NULL,
    PRIMARY KEY (invitation_id),
    FOREIGN KEY (inviter_id) REFERENCES Clients (id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (invitee_id) REFERENCES Clients (id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS Complaints
(
    complaint_id     SERIAL,
    complainant_id   INTEGER,
    complainee_id    INTEGER,
    date             TIMESTAMP   NOT NULL,
    text             VARCHAR(50) NOT NULL,
    complaint_status VARCHAR(50) NOT NULL,
    PRIMARY KEY (complaint_id),
    FOREIGN KEY (complainant_id) REFERENCES Clients (id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (complainee_id) REFERENCES Clients (id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS AdditionalContacts
(
    id        SERIAL,
    client_id INTEGER,
    telegram  VARCHAR(50),
    facebook  VARCHAR(50),
    instagram VARCHAR(50),
    tiktok    VARCHAR(50),
    PRIMARY KEY (id),
    FOREIGN KEY (client_id) REFERENCES Clients (id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS PhoneNumbers
(
    id                     SERIAL,
    phone_number           VARCHAR(50) NOT NULL,
    additional_contacts_id INTEGER,
    PRIMARY KEY (id),
    FOREIGN KEY (additional_contacts_id) REFERENCES AdditionalContacts (id) ON DELETE CASCADE ON UPDATE CASCADE
);