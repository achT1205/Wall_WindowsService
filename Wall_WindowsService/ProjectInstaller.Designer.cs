namespace Wall_WindowsService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.EmailServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.EmailServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // EmailServiceProcessInstaller
            // 
            this.EmailServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.EmailServiceProcessInstaller.Password = null;
            this.EmailServiceProcessInstaller.Username = null;
            // 
            // EmailServiceInstaller
            // 
            this.EmailServiceInstaller.Description = "Un service pour rattraper les communications.";
            this.EmailServiceInstaller.DisplayName = "Wall_Windows Service (EmailService)";
            this.EmailServiceInstaller.ServiceName = "EmailService";
            this.EmailServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.EmailServiceProcessInstaller,
            this.EmailServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller EmailServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller EmailServiceInstaller;
    }
}