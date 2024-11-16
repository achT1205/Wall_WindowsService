using System;

namespace Wall_WindowsService.Models
{
    public class AuditCom
    {

        public string NNI { get; set; }

        public Nullable<DateTime> DateEnvoi { get; set; }

        public string Objet { get; set; }

        public string ListeDiffusion { get; set; }

        public string Contenu { get; set; }

        public Nullable<int> IdEvenement { get; set; }

        public Nullable<int> IdIncident { get; set; }
        public string Type { get; set; }

        public string Commentaire { get; set; }

        public int? EtatID { get; set; }

    }
}
