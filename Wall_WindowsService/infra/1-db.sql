-- Create the Database
CREATE DATABASE WALLE_INTRANET;
GO

-- Use the Database
USE WALLE_INTRANET;
GO

-- Create Etat Table
CREATE TABLE Etat (
    ID INT IDENTITY PRIMARY KEY,
    Valeur NVARCHAR(255) NOT NULL
);
GO

-- Create Environnement Table
CREATE TABLE Environnement (
    ID INT IDENTITY(1,1) PRIMARY KEY, -- Primary key with auto-increment
    Valeur NVARCHAR(255) NOT NULL -- Environment value, required
);
GO

-- Create Porteur Table
CREATE TABLE Porteur (
    ID INT IDENTITY(1,1) PRIMARY KEY, -- Primary key with auto-increment
    Valeur NVARCHAR(255) NOT NULL -- Porteur value, required
);
GO

-- Create Application Table
CREATE TABLE Application (
    ID INT IDENTITY PRIMARY KEY,
    ApplicationName NVARCHAR(255) NOT NULL,
    Entite NVARCHAR(255) NOT NULL,
    NNI NVARCHAR(255) NOT NULL,
    EstActif BIT NOT NULL
);
GO

-- Create Pilote Table
CREATE TABLE Pilote (
    ID INT IDENTITY PRIMARY KEY,
    Valeur NVARCHAR(255) NOT NULL
);
GO

-- Create Site Table
CREATE TABLE Site (
    ID INT IDENTITY PRIMARY KEY,
    NomCourt NVARCHAR(255) NOT NULL,
    SiteName NVARCHAR(255) NOT NULL,
    Direction NVARCHAR(255) NOT NULL
);
GO

-- Create Typologie Table
CREATE TABLE Typologie (
    ID INT IDENTITY PRIMARY KEY,
    TypologieNom NVARCHAR(255) NOT NULL,
    Nomcourt NVARCHAR(255) NOT NULL,
    Type_SIVISION NVARCHAR(255) NULL
);
GO



CREATE TABLE Evenement (
    ID INT IDENTITY(1,1) PRIMARY KEY,         -- Primary key, auto-increment
    DateDebut DATETIME NOT NULL,              -- Start date
    DateFin DATETIME NOT NULL,                -- End date
    DateFermeture DATETIME NULL,              -- Closure date (Nullable)
    Libelle NVARCHAR(255) NOT NULL,           -- Event title
    Impact NVARCHAR(MAX) NULL,                -- Event impact description
    EnvID INT NULL,                           -- Foreign key to Environnement table (Nullable)
    PorteurID INT NULL,                       -- Foreign key to Porteur table (Nullable)
    PiloteID INT NOT NULL,                    -- Foreign key to Pilote table
    EtatID INT NOT NULL,                      -- Foreign key to Etat table
    TypologieID INT NULL,                     -- Foreign key to Typologie table (Nullable)
    InstantMajID INT NULL,                    -- Instant update ID (Nullable)
    Description NVARCHAR(MAX) NULL,           -- Event description (Nullable)
    DateCreation DATETIME NOT NULL DEFAULT GETDATE(), -- Creation date
    DateModification DATETIME NOT NULL DEFAULT GETDATE(), -- Last modification date
    GestionInterne BIT NULL,                  -- Internal management flag (Nullable)
    [CriticiteMax] [smallint] NULL,
    [GUID] [uniqueidentifier] NULL,
    [idMessageLinked] [uniqueidentifier] NOT NULL,

    -- Foreign Key Constraints
    CONSTRAINT FK_Evenement_Environnement FOREIGN KEY (EnvID) REFERENCES Environnement(ID),
    CONSTRAINT FK_Evenement_Porteur FOREIGN KEY (PorteurID) REFERENCES Porteur(ID),
    CONSTRAINT FK_Evenement_Pilote FOREIGN KEY (PiloteID) REFERENCES Pilote(ID),
    CONSTRAINT FK_Evenement_Etat FOREIGN KEY (EtatID) REFERENCES Etat(ID),
    CONSTRAINT FK_Evenement_Typologie FOREIGN KEY (TypologieID) REFERENCES Typologie(ID)
);
GO


-- Create Many-to-Many Relationship Tables
-- Evenement-Application Relationship Table
CREATE TABLE Evenement_Application (
    EvenementID INT NOT NULL,
    ApplicationID INT NOT NULL,
    PRIMARY KEY (EvenementID, ApplicationID),
    FOREIGN KEY (EvenementID) REFERENCES Evenement(ID),
    FOREIGN KEY (ApplicationID) REFERENCES Application(ID)
);
GO

-- Evenement-Site Relationship Table
CREATE TABLE Evenement_Site (
    EvenementID INT NOT NULL,
    SiteID INT NOT NULL,
    PRIMARY KEY (EvenementID, SiteID),
    FOREIGN KEY (EvenementID) REFERENCES Evenement(ID),
    FOREIGN KEY (SiteID) REFERENCES Site(ID)
);
GO



-- Create AuditCom Table
CREATE TABLE AuditCom (
    ID INT IDENTITY(1,1) PRIMARY KEY, -- Primary key
    NNI NVARCHAR(255) NOT NULL, -- Identifier for the user or system
    DateEnvoi DATETIME NULL, -- Nullable datetime for the sent date
    Objet NVARCHAR(255) NOT NULL, -- Subject of the communication
    ListeDiffusion NVARCHAR(MAX) NOT NULL, -- Distribution list
    Contenu NVARCHAR(MAX) NOT NULL, -- Content of the communication
    IdEvenement INT NULL, -- Foreign key to Evenement table
    IdIncident INT NULL, -- Foreign key to Incident table, if applicable
    Type NVARCHAR(50) NOT NULL, -- Type of the communication
    Commentaire NVARCHAR(MAX) NULL, -- Additional comments
    EtatID INT NULL, -- Foreign key to Etat table
    FOREIGN KEY (IdEvenement) REFERENCES Evenement(ID), -- Foreign key constraint
    FOREIGN KEY (EtatID) REFERENCES Etat(ID) -- Foreign key constraint
);
GO

