CREATE TABLE events(
       id serial NOT NULL primary key,
       category varchar(500) NOT NULL,
       streamid uuid NOT NULL,
       transactionid uuid NOT NULL,
       metadata text NOT NULL,
       bodytype varchar(500) NOT NULL,
       body text NOT NULL,
       by uuid NOT NULL,
       at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
       version int NOT NULL,
       appversion varchar(20) NOT NULL,
       sessionid uuid NULL);