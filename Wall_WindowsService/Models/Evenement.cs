using System;
using System.Collections.Generic;

namespace Wall_WindowsService.Models
{
    public class Evenement
    {
        internal string idMessageLinked;

        public List<Application> Applications { get; set; }
        public List<Site> Sites { get; set; }
        public virtual Etat Etat { get; set; }
        public string Libelle { get; set; }
        public int CriticiteMax { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string Description { get; set; }
        public Typologie Typologie { get; set; }
        public Pilote Pilote { get; set; }

        private List<string> listImpacts;
        public List<string> ListImpacts
        {
            get { return listImpacts; }
            set
            {
                listImpacts = value;
                Impact = "";
                foreach (string impactPart in listImpacts) Impact = Impact + impactPart + "\r\n";
            }
        }
        public int ID { get; set; }
        public int PiloteID { get; set; }
        public int EtatID { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime DateModification { get; set; }
        public bool? GestionInterne { get; set; }
        public string Impact { get; set; }
        public DateTime? DateFermeture { get; set; }
        public int EnvID { get; set; }
        public Environnement Environnement { get; set; }
        public int PorteurID { get; set; }
        public Porteur Porteur { get; set; }
        public string GUID { get; set; }
    }
}