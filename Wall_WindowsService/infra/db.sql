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

-- Create Evenement Table
CREATE TABLE Evenement (
    ID INT IDENTITY PRIMARY KEY,
    Libelle NVARCHAR(255) NOT NULL,
    CriticiteMax INT NULL,
    DateDebut DATETIME NOT NULL,
    DateFin DATETIME NOT NULL,
    Description NVARCHAR(MAX),
    EtatID INT NOT NULL,
    TypologieID INT NOT NULL,
    PiloteID INT NOT NULL,
    DateCreation DATETIME NOT NULL DEFAULT GETDATE(),
    DateModification DATETIME NOT NULL DEFAULT GETDATE(),
    GestionInterne BIT,
    Impact NVARCHAR(MAX),
    FOREIGN KEY (EtatID) REFERENCES Etat(ID),
    FOREIGN KEY (TypologieID) REFERENCES Typologie(ID),
    FOREIGN KEY (PiloteID) REFERENCES Pilote(ID)
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



---------------------------------------------------------------Insertions -------------------------------------------------------------------


USE WALLE_INTRANET;
GO

-- Insert into Etat Table
INSERT INTO Etat (Valeur)
VALUES 
('En cours'),
('Ferme');
GO

-- Insert into Pilote Table
INSERT INTO Pilote (Valeur)
VALUES ('John Doe');
GO


-- Insert into Application Table
INSERT INTO Application (ApplicationName, Entite, NNI, EstActif)
VALUES 
('EAM', 'SI rénové', 'EAM', 1),
('BI', 'SI rénové', 'INF', 1),
('MRS', 'SI rénové', 'MRS', 1),
('GPS', 'SI rénové', 'GPS', 1),
('GMO2', 'SI rénové', 'GM2', 1),
('Micado 5', 'SI rénové', 'MCO', 1),
('DOSIAP', 'SI rénové', 'DSA', 1),
('Epsilon 2', 'SI rénové', 'EPS', 1),
('AP913', 'SI rénové', 'AP9', 1),
('Espace', 'SI existant', 'BLC', 1),
('Merlin', 'SI existant', 'MLN', 1),
('Cahier de Quart', 'SI existant', 'CDQ', 1),
('PDME', 'SI existant', 'old', 1),
('AIC', 'SI existant', 'AIC', 1);
GO


-- Insert into Site Table
INSERT INTO Site (SiteName, NomCourt, Direction)
VALUES 
('UTO', 'UTO', 'Non-CNPE'),
('Tous les sites', 'Tous', 'Tous'),
('Belleville', 'BEL', 'CNPE'),
('Blayais', 'BLA', 'CNPE'),
('Brennillis', 'BRE', 'CNPE'),
('Penly', 'PEN', 'CNPE'),
('Golfech', 'GOL', 'CNPE'),
('Flamanville 3', 'FA3', 'CNPE');
GO


-- Insert into Typologie Table
INSERT INTO Typologie (TypologieNom, Nomcourt, Type_SIVISION)
VALUES 
('Applicatif', 'Appli', 'Changement fonctionnel'),
('Infrastructure', 'Infra', 'Changement infrastructure'),
('Réseau', 'Réseau', 'Changement technique'),
('Stockage', 'Stockage', 'Changement technique'),
('Flux', 'Flux', 'Changement technique')--,
--('Divers', 'Divers', NULL);
GO



INSERT INTO Evenement 
    (CriticiteMax, DateDebut, DateFin, Libelle, Impact, PiloteID, EtatID, Description, DateCreation, DateModification, TypologieID, GestionInterne)
VALUES
    (1,
    '2022-06-08 12:00:00.000', 
    '2022-06-08 12:25:00.000', 
     'Opération de maintenance', 
     '[++] Indisponibilité de l''application', 
     1,
     1,
     'L''opération technique sur l''application FORQUART, prévue le 08/06/2022 de 12h00 à 14h00 est terminée. L''application est de nouveau pleinement disponible.', 
     '2022-06-03 15:21:21.267', 
     '2022-06-08 12:32:34.150', 
     7,
     0),

    (1,
    '2022-06-14 12:00:00.000', 
    '2022-06-14 12:18:00.000', 
     'Opération de maintenance', 
     '[++] Indisponibilité de l''application', 
     1,
     1,
     'L''opération technique sur l''application ANY, prévue le 14/06/2022 de 12h00 à 13h00, est terminée. L''application est de nouveau pleinement disponible.', 
     '2022-06-03 15:31:30.073', 
     '2022-06-14 12:22:47.047', 
     7,
     0),

    (2,
    '2022-06-14 09:00:00.000', 
    '2022-06-15 13:25:00.000', 
     'Opération de maintenance', 
     '[++] Indisponibilité de l''application BI le 15/06 de 12h00 à 13h30  [+] Dégradation de la fraicheurs de données le 14/06 de 10h00 à 11h30', 
     1,
     1,
     'L''opération de maintenance sur l''infrastructure de l''application BI, prévue ce jour, s''est terminée avec succès. Celle-ci est dorénavant pleinement disponible et fonctionnelle. Veuillez nous excuser pour la gêne occasionnée.', 
     '2022-06-08 17:29:34.970', 
     '2022-06-15 13:27:51.420', 
     7,
     0),

    (2,
    '2022-06-24 20:00:00.000', 
    '2022-06-25 14:00:00.000', 
     'Mise en production EAM V04.00.00 BI V08.01.00 et EASYPA V03.03.00', 
     '[+] Les univers BI ne seront à jour que le dimanche 26/06 au matin. [+] Les données pour le service RDT d''EOX seront figées du 24/06 à 20h au 25/06 05h30 [+] Données en provenance de l''EAM non rafraichies du vendredi 24/06 13h au mardi 28/06 matin [++] Indisponiblité EAM et BI du 24/06 de 20h00 au 25/06 à 08h30 [++] Indisponiblité EASYPA du 24/06 de 20h00 au 25/06 à 09h15', 
     1,
     1,
     'Dans le cadre de la Mise en Production de l''application EAM V04.00.00, celle-ci est de nouveau disponible depuis le 25/06 à 05h30. [...] Veuillez nous excuser pour la gêne occasionnée.', 
     '2022-06-17 14:34:53.537', 
     '2022-06-25 14:51:28.180', 
     7,
     0),

    (1,
    '2022-07-06 18:50:00.000', 
    '2022-07-06 21:55:00.000', 
     'Lot de correction de données EAM', 
     '[+] Perturbations service RDT d''ESPACE OPERATIONNEL [++] Indisponibilité des applications EAM et EasyPA de 19h30 à 21H45', 
     1,
     1,
     'La Mise en Production du Lot de correction de données EAM_Data66 de l''application EAM s''est déroulé le Mercredi 06/07/2022 de 18h50 à 21H55. [...] Veuillez nous excuser pour la gêne occasionnée.', 
     '2022-06-20 10:12:17.977', 
     '2022-07-06 22:10:08.910', 
     7,
     0),

    (1,
    '2022-06-28 08:30:00.000', 
    '2022-06-28 08:31:00.000', 
     'Opération de maintenance', 
     '[++] Données EAM non mises à jour dans EOX et ESPADON READER de 9h à 15h30', 
     1,
     2,
     'Suite à une opération de maintenance ESPADON le 28/06/2022 de 08h30 à 15h30, les données EAM ne seront pas rafraichies de 09h00 à 15h30 dans les applications ESPACE OPERATIONNEL et ESPADON READER. [...] Veuillez nous excuser pour la gêne occasionnée.', 
     '2022-06-24 09:36:13.693', 
     '2022-06-28 10:02:01.270', 
     7,
     0);
GO


------------------------------------------------------------------------ PS------------------------------------------------

USE WALLE_INTRANET;
GO

CREATE PROCEDURE sp_GetEvenementsForMailScheduler
AS
BEGIN
    SET NOCOUNT ON;

    -- Retrieve list of Evenements
    SELECT 
        ID,
        CriticiteMax,
        DateDebut,
        DateFin,
        Libelle,
        Impact,
        PiloteID,
        EtatID,
        Description,
        DateCreation,
        DateModification,
        TypologieID,
        GestionInterne
    FROM 
        Evenement
END;
GO



exec sp_GetEvenementsForMailScheduler