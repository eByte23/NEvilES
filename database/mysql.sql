CREATE TABLE events(
       id bigint autoincrement NOT NULL PRIMARY KEY,
       category varchar(500) NOT NULL,
       streamid CHAR(36) NOT NULL,
       transactionid CHAR(36) NOT NULL,
       metadata varchar(max) NOT NULL,
       bodytype varchar(500) NOT NULL,
       body varchar(max) NOT NULL,
       by CHAR(36) NOT NULL,
       at datetime NOT NULL,
       version int NOT NULL,
       appversion varchar(20) NOT NULL,
       sessionid CHAR(36) NULL);