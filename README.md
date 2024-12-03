# **EmailService**

Un service Windows conçu pour automatiser l'envoi d'e-mails basés sur des événements enregistrés dans une base de données SQL Server. Ce service vérifie les événements planifiés, envoie des e-mails via SMTP, et consigne les activités pour un suivi et une auditabilité détaillés.

---

## **Fonctionnalités**

- Vérifie périodiquement une base de données pour détecter les événements à venir.
- Envoie des e-mails automatisés basés sur des horaires prédéfinis.
- Maintient des journaux d'activité incluant les e-mails envoyés et les erreurs détectées.
- Permet de configurer facilement les paramètres SMTP et les connexions à la base de données.
- Gestion robuste des erreurs et des mécanismes de réessai.

---

## **Prérequis**

- Système d'exploitation Windows avec des droits administratifs.
- [.NET Framework 4.7.2 ou supérieur](https://dotnet.microsoft.com/download/dotnet-framework).
- Accès à une base de données SQL Server.
- Serveur SMTP pour la distribution des e-mails.

---

## **Installation**

### **1. Cloner ou Télécharger le Projet**
- Placez les fichiers suivants dans un répertoire cible, par exemple :  
  `D:\EDF WALL-E\WALLSDIN_WINDOWS_SERVICE\`
  ```
  Wall_WindowsService.exe
  Install.ps1
  Uninstall.ps1
  Fichier de configuration (ex. : Wall_WindowsService.config)
  ```

### **2. Modifier le Fichier de Configuration**
- Mettez à jour la section `connectionStrings` avec les détails de votre base de données :
  ```xml
  <connectionStrings>
    <add name="WalleSdinIntranetDb" connectionString="Server=VOTRE_SERVEUR;Database=VOTRE_BDD;User Id=UTILISATEUR;Password=MOTDEPASSE;" providerName="System.Data.SqlClient" />
  </connectionStrings>
  ```
- Mettez à jour la section `appSettings` avec vos paramètres SMTP :
  ```xml
  <appSettings>
    <add key="SmtpHost" value="mail.example.com" />
    <add key="SmtpPort" value="25" />
    <add key="EmailSender" value="noreply@example.com" />
    <add key="Interval" value="60000" />
  </appSettings>
  ```

### **3. Exécuter le Script d'Installation**
1. Ouvrez PowerShell en **tant qu'administrateur**.
2. Naviguez vers le répertoire contenant les fichiers :
   ```powershell
   cd "D:\EDF WALL-E\WALLSDIN_WINDOWS_SERVICE"
   ```
3. Lancez le script d'installation :
   ```powershell
   .\Install.ps1
   ```

### **4. Vérifier l'Installation**
- Ouvrez `services.msc` et assurez-vous que le service **Wall_Windows Service (EmailService)** est en état **En cours d'exécution**.

---

## **Utilisation**

### **Démarrage et Arrêt du Service**
- Pour démarrer le service :
  ```powershell
  Start-Service -Name "EmailService"
  ```
- Pour arrêter le service :
  ```powershell
  Stop-Service -Name "EmailService"
  ```

### **Accès aux Journaux**
- Les journaux d'activité sont générés dans le répertoire `Logs\` situé à la racine du service :
  ```
  D:\EDF WALL-E\WALLSDIN_WINDOWS_SERVICE\Logs\EmailServiceLog_YYYY_MM_DD.txt
  ```

---

## **Désinstallation**

### **Procédure de Désinstallation**
1. Ouvrez PowerShell en **tant qu'administrateur**.
2. Naviguez vers le répertoire du service :
   ```powershell
   cd "D:\EDF WALL-E\WALLSDIN_WINDOWS_SERVICE"
   ```
3. Lancez le script de désinstallation :
   ```powershell
   .\Uninstall.ps1
   ```

4. Vérifiez dans `services.msc` que le service a bien été supprimé.

---

## **Configuration**

### **Fichier de Configuration**
- **Emplacement :** Le fichier XML de configuration se trouve dans le même dossier que l’exécutable.
- **Sections clés :**
  - **Connexion à la base de données :**
    ```xml
    <connectionStrings>
      <add name="WalleSdinIntranetDb" connectionString="Server=VOTRE_SERVEUR;Database=VOTRE_BDD;User Id=UTILISATEUR;Password=MOTDEPASSE;" providerName="System.Data.SqlClient" />
    </connectionStrings>
    ```
  - **Paramètres SMTP :**
    ```xml
    <appSettings>
      <add key="SmtpHost" value="mail.example.com" />
      <add key="SmtpPort" value="25" />
      <add key="EmailSender" value="noreply@example.com" />
    </appSettings>
    ```
  - **Intervalle d'exécution (en millisecondes) :**
    ```xml
    <add key="Interval" value="60000" />
    ```

---

## **Dépannage**

| **Problème**                      | **Cause Possible**                           | **Solution**                                         |
|-----------------------------------|---------------------------------------------|-----------------------------------------------------|
| Le service ne démarre pas         | Fichier de configuration invalide           | Vérifiez les paramètres SMTP et base de données.    |
| Les e-mails ne sont pas envoyés   | Serveur SMTP inaccessible                    | Vérifiez les logs et les paramètres du serveur SMTP.|
| Le service s'arrête immédiatement | Erreur dans le fichier de configuration      | Vérifiez la syntaxe et les valeurs des paramètres.  |
| Les journaux ne sont pas générés  | Permissions insuffisantes                   | Assurez-vous que le service a accès au répertoire `Logs`.|

---

## **Maintenance**

### **Mise à Jour**
1. Arrêtez le service :
   ```powershell
   Stop-Service -Name "EmailService"
   ```
2. Remplacez le fichier `Wall_WindowsService.exe` par la nouvelle version.
3. Redémarrez le service :
   ```powershell
   Start-Service -Name "EmailService"
   ```

### **Sauvegarde**
- Avant toute mise à jour, sauvegardez le fichier de configuration pour préserver les paramètres.

## **Contact**

Pour toute assistance technique ou question, veuillez contacter l'équipe de support :
- **Email :** achilletuglo12@gmail.com
