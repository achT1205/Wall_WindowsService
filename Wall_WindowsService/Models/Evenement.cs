using System;
using System.Collections.Generic;

namespace Wall_WindowsService.Models
{
    public class Evenement
    {
        public List<Application> Applications { get; set; }
        public List<Site> Sites { get; set; }
        public virtual Etat Etat { get;  set; }
        public string Libelle { get;  set; }
        public int CriticiteMax { get;  set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; internal set; }
        public string Description { get; internal set; }
        public Typologie Typologie { get; internal set; }
        public Pilote Pilote { get; internal set; }
        public List<string> ListImpacts { get; internal set; }
        public int ID { get; internal set; }
        public int PiloteID { get; internal set; }
        public int EtatID { get; internal set; }
        public DateTime DateCreation { get; internal set; }
        public DateTime DateModification { get; internal set; }
        public bool? GestionInterne { get; internal set; }
        public string Impact { get; internal set; }
    }
}