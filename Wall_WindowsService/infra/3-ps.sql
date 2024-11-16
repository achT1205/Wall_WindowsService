------------------------------------------------------------------------ PS------------------------------------------------

USE WALLE_INTRANET;
GO

CREATE PROCEDURE [dbo].[sp_GetEvenementsForMailScheduler]
    @EnvName NVARCHAR(100),       -- Environment Name
    @IntervalHours INT            -- Time interval in hours
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        e.ID,                                    -- 0: Evenement ID
        e.DateDebut,                             -- 1: Start Date
        e.DateFin,                               -- 2: End Date
        e.DateFermeture,                         -- 3: Closure Date (Nullable)
        e.Libelle,                               -- 4: Event Title
        e.Impact,                                -- 5: Impact Description
        e.EnvID,                                 -- 6: Environnement ID
        env.Valeur AS Environnement,             -- 7: Environnement Value
        e.PorteurID,                             -- 8: Porteur ID
        port.Valeur AS Porteur,                  -- 9: Porteur Value
        e.PiloteID,                              -- 10: Pilote ID
        p.Valeur AS Pilote,                      -- 11: Pilote Value
        e.EtatID,                                -- 12: Etat ID
        et.Valeur AS Etat,                       -- 13: Etat Value
        e.Description,                           -- 14: Description (Nullable)
        e.DateCreation,                          -- 15: Creation Date
        e.DateModification,                      -- 16: Last Modification Date
        e.InstantMajID,
        e.TypologieID,                           -- 18: Typologie ID (Nullable)
        t.TypologieNom AS Typologie,             -- 19: Typologie Name (Nullable)
        e.GestionInterne                         -- 20: Gestion Interne (Nullable)
    FROM 
        Evenement e
    LEFT JOIN 
        Environnement env ON e.EnvID = env.ID
    LEFT JOIN 
        Porteur port ON e.PorteurID = port.ID
    LEFT JOIN 
        Pilote p ON e.PiloteID = p.ID
    LEFT JOIN 
        Etat et ON e.EtatID = et.ID
    LEFT JOIN 
        Typologie t ON e.TypologieID = t.ID
    --WHERE 
    --    (@EnvName IS NULL OR env.Valeur = @EnvName) -- Filter by environment name
    --    AND DATEDIFF(HOUR, e.DateModification, GETDATE()) <= @IntervalHours -- Filter by interval
    --ORDER BY 
    --    e.DateDebut ASC; -- Order by start date
END;
GO





CREATE PROCEDURE [dbo].[SP_GetApplicationsByEvenement]
    @EvenementID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        A.ID,
        A.ApplicationName,
        A.Entite,
        A.NNI,
        A.EstActif
    FROM 
        Application A
    INNER JOIN 
        Evenement_Application EA ON A.ID = EA.ApplicationID
    --WHERE 
    --    EA.EvenementID = @EvenementID
    --ORDER BY 
    --    A.ApplicationName ASC; -- Optional: Order by application name
END;
GO


-- Insert the data into the AuditCom table

CREATE PROCEDURE [dbo].[SP_InsertAuditComm]
    @NNI NVARCHAR(128),
    @DateEnvoi DATETIME = NULL,
    @Objet NVARCHAR(255),
    @ListeDiffusion NVARCHAR(MAX),
    @Contenu NVARCHAR(MAX),
    @IdEvenement INT = NULL,
    @IdIncident INT = NULL,
    @Type NVARCHAR(128),
    @Commentaire NVARCHAR(2048) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    
    INSERT INTO AuditCom (
        NNI,
        DateEnvoi,
        Objet,
        ListeDiffusion,
        Contenu,
        IdEvenement,
        IdIncident,
        Type,
        Commentaire
    )
    VALUES (
        @NNI,
        @DateEnvoi,
        @Objet,
        @ListeDiffusion,
        @Contenu,
        @IdEvenement,
        @IdIncident,
        @Type,
        @Commentaire
    );
END;
GO

  
 -- Retrieve the list of sites associated with the specified EvenementID

CREATE PROCEDURE [dbo].[SP_GetSitesByEvenement]
    @EvenementID INT
AS
BEGIN
    SET NOCOUNT ON;

   
    SELECT 
        s.ID,
        s.SiteName,
        s.NomCourt,
        s.Direction
    FROM 
        Site s
    --INNER JOIN 
    --    Evenement_Site es ON s.ID = es.SiteID
    ----WHERE 
    ----    es.EvenementID = @EvenementID
    ----ORDER BY 
    ----    s.SiteName ASC; -- Order by site name (optional)
END;
GO


    -- Retrieve all typologies
CREATE PROCEDURE [dbo].[SP_GetTypologies]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ID,
        TypologieNom,
        Nomcourt,
        Type_SIVISION
    FROM 
        Typologie
    --ORDER BY 
    --    TypologieNom ASC; -- Order by typology name (optional)
END;
GO