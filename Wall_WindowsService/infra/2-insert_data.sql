---------------------------------------------------------------Insertions -------------------------------------------------------------------


USE WALLE_INTRANET;
GO

-- Insert into Etat Table
INSERT INTO Etat (Valeur)
VALUES 
('En cours'),
('Ferme');
GO

INSERT INTO Environnement (Valeur)
VALUES 
('Production'),
('Staging'),
('Development'),
('Test');
GO

INSERT INTO Porteur (Valeur)
VALUES 
('John Doe'),
('Jane Smith'),
('Project Manager'),
('Team Lead');
GO

-- Insert into Pilote Table
INSERT INTO Pilote (Valeur)
VALUES ('John Doe');
GO


-- Insert into Application Table
INSERT INTO Application (ApplicationName, Entite, NNI, EstActif)
VALUES 
('EAM', 'SI r�nov�', 'EAM', 1),
('BI', 'SI r�nov�', 'INF', 1),
('MRS', 'SI r�nov�', 'MRS', 1),
('GPS', 'SI r�nov�', 'GPS', 1),
('GMO2', 'SI r�nov�', 'GM2', 1),
('Micado 5', 'SI r�nov�', 'MCO', 1),
('DOSIAP', 'SI r�nov�', 'DSA', 1),
('Epsilon 2', 'SI r�nov�', 'EPS', 1),
('AP913', 'SI r�nov�', 'AP9', 1),
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
('R�seau', 'R�seau', 'Changement technique'),
('Stockage', 'Stockage', 'Changement technique'),
('Flux', 'Flux', 'Changement technique')--,
--('Divers', 'Divers', NULL);
GO



INSERT INTO Evenement (
    DateDebut,
    DateFin,
    DateFermeture,
    Libelle,
    Impact,
    EnvID,
    PorteurID,
    PiloteID,
    EtatID,
    TypologieID,
    InstantMajID,
    Description,
    DateCreation,
    DateModification,
    GestionInterne,
	CriticiteMax,
    GUID,
    idMessageLinked
) VALUES (
    '2022-06-24 20:00:00.000',                          -- DateDebut
    '2022-06-25 14:00:00.000',                          -- DateFin
    '2022-06-25 14:49:43.043',                          -- DateFermeture
    'Mise en production EAM V04.00.00 BI V08.01.00 et EASYPA V03.03.00', -- Libelle
    '[+] Les univers BI  ne seront � jour que le dimanche 26/06 au matin.  [+] Les donn�es pour le service RDT d''EOX seront fig�es du 24/06 � 20h au 25/06 05h30  [+] Donn�es en provenance de l''EAM non rafraichies du vendredi 24/06 13h au mardi 28/06 matin    [++] Indisponiblit� EAM et BI du 24/06 de 20h00 au 25/06 � 08h30  [++] Indisponiblit� EASYPA du 24/06 de 20h00 au 25/06 � 09h15', -- Impact
    1,                                                  -- EnvID
    1,                                                  -- PorteurID
    1,                                                  -- PiloteID
    1,                                                  -- EtatID
    1,                                                  -- TypologieID
    NULL,                                               -- InstantMajID (interpreted as null from -9.22337E+18)
    'Dans le cadre de la Mise en Production de l''application EAM V04.00.00, celle-ci est de nouveau disponible depuis le 25/06 � 05h30.    Dans le cadre de la Mise en Production de l''application EASYPA V03.03.00, celle-ci est de nouveau disponible depuis le 25/06 � 06h30.    Dans le cadre de la Mise en Production de l''application BI V08.01.00, elle-ci est de nouveau disponible depuis le 25/06 � 14h00.    La pose EAM comprend les modifications suivantes :  - Des corrections d�anomalies fonctionnelles et techniques  - Des �volutions fonctionnelles sur les processus : P002, P014, P019, P020, P021, P022, P025, P030  - Des nouveaux webservices pour easyPA et eDRT  Cette version inclura aussi un lot de param�trage permettant :  - Arr�t des notifications des messages AAA  - Pour ceux qui ont le droit d''approbation des DT, ils pourront repasser une DT � NOUVEAU    Pour plus d�informations sur le p�rim�tre impact� : https://edfonline.sharepoint.com/:p:/r/sites/TEAM688/Documents%20partages/General/EAM%204.0%20CDC%20V0.pptx?d=w2691f562b88a441d9718878d471ea4d8&csf=1&web=1&e=BmOhas    Durant ce cr�neau, les interfaces avec l�EAM ont �t� indisponibles.    Concernant ESPACE OPERATIONNEL : Le service RDT a �t� impact�. Les donn�es ont �t� fig�es � l''heure de l''arr�t de l''EAM soit le 24/06 � 20h. Les donn�es seront rafraichies � partir du 25/06 05h30.    Concernant le BI : Les ROP BI seront aussi indisponibles du 24 Juin � 20h00 au 25 Juin � 14h00. La nuit applicative a �t� d�sactiv�e le 24/06. Les univers ne seront � jour que le dimanche 26/06 au matin.    Concernant EASYPA : L''application est disponible sur sa nouvelle URL EASYPA : https://prod-sdin.edf.fr/easypa/web/ et EASYPERMIS  https://prod-sdin.edf.fr/easypa/web/search/prms     Concernant ESPADON : les donn�es EAM ne seront pas rafraichies du vendredi 24/06 13h au mardi 28/06 matin pour les applications ESPACE OPERATIONNEL, ADREX, PLURITOOLS, OMAIRE, CAMELEON, CADOR, ORGE, ESPADON READER    ISODATA sera recharg� avec les donn�es de production EAM du 22/06 matin. Il sera mis en lecture seule pour l�utilisateur TESTEUR via l''URL : https://prod-sdin-i3na-g506.edf.fr/as/ui/    Veuillez nous excuser pour la g�ne occasionn�e.', -- Description
    '2022-06-17 14:34:53.537',                          -- DateCreation
    '2022-06-25 14:51:28.180',                          -- DateModification
    0,                                                   -- GestionInterne
    0,
    'AC3651C9-D97A-4951-AEE1-87FD6DA448AD',
    'AC3651C9-D97A-4951-AEE1-87FD6DA448AD'
),
(
    '2024-11-30 09:40:00.000',
    '2024-11-30 17:35:00.000',
    NULL,
    'Arr�t/Relance de l''application',
    '[+] test J-1 29/11 17h00',
    1,                                                  -- EnvID
    1,                                                  -- PorteurID
    1,                                                  -- PiloteID
    1,                                                  -- EtatID
    1,                                                  -- TypologieID
    NULL,                                               -- InstantMajID (interpreted as null from -9.22337E+18)
    'test J-1 29/11 17h00 Achille avec impact [+] test J-1 29/11 17h00',
    '2024-11-28 14:28:53.597',
    '2024-11-29 09:36:08.327',
    0,
     1,
    'AC3651C9-D97A-4951-AEE1-87FD6DA448AD',
    'AC3651C9-D97A-4951-AEE1-87FD6DA448AD'
),
(
    '2023-06-20 15:30:00.000',
    '2023-06-21 02:30:00.000',
    '2023-10-12 07:54:08.590',
    'Op�ration de maintenance',
    'Aucun impact',
    1,                                                  -- EnvID
    1,                                                  -- PorteurID
    1,                                                  -- PiloteID
    1,                                                  -- EtatID
    1,                                                  -- TypologieID
   NULL,
    'TEST',
    '2023-06-19 15:28:48.573',
    '2024-11-29 09:12:40.170',
    0,
     2,
    'AC3651C9-D97A-4951-AEE1-87FD6DA448AD',
    'AC3651C9-D97A-4951-AEE1-87FD6DA448AD'
),
(
    '2023-06-20 15:30:00.000',
    '2023-06-21 02:30:00.000',
    '2023-10-12 07:54:08.590',
    'Op�ration de maintenance',
    'Aucun impact',
    1,                                                  -- EnvID
    1,                                                  -- PorteurID
    1,                                                  -- PiloteID
    1,                                                  -- EtatID
    1,                                                  -- TypologieID
   NULL,
    'TEST',
    '2023-06-19 15:28:48.573',
    '2024-11-29 09:12:40.170',
    0,
     3,
    'AC3651C9-D97A-4951-AEE1-87FD6DA448AD',
    'AC3651C9-D97A-4951-AEE1-87FD6DA448AD'
)
GO
   



--select * from Evenement