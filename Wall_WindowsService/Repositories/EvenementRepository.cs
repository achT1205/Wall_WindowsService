using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using Wall_WindowsService.Models;

namespace Wall_WindowsService.Repositories
{
    public class EvenementRepository
    {
        private readonly string _connectionString;
        private readonly TypologieRepository _typologieRepository;
        private readonly SiteRepository  _siteRepository;
        private readonly ApplicationRepository  _applicationRepository;

        public EvenementRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["WalleSdinIntranetDb"].ConnectionString;
            _typologieRepository = new TypologieRepository();
            _siteRepository = new SiteRepository();
            _applicationRepository = new ApplicationRepository();
        }

        public IQueryable<Evenement> GetEvenementsForMailScheduler(string envName, int interval)
        {
            var evenements = new List<Evenement>();
            string query = "[dbo].[sp_GetEvenementsForMailScheduler]";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = query;

                    var prmEnvName = new SqlParameter("EnvName", SqlDbType.NVarChar, 100);
                    prmEnvName.Value = envName;
                    cmd.Parameters.Add(prmEnvName);
                    cmd.Parameters.Add("IntervalHours", SqlDbType.Int).Value = interval;

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Evenement evenement = new Evenement();
                        evenement.Etat = new Etat();

                        evenement.Pilote = new Pilote();

                        evenement.ID = reader.GetInt32(0);
                        evenement.DateDebut = reader.GetDateTime(1);
                        evenement.DateFin = reader.GetDateTime(2);
                        evenement.Libelle = reader.GetString(4);
                        evenement.PiloteID = reader.GetInt32(10);
                        evenement.Pilote.ID = evenement.PiloteID;
                        evenement.Pilote.Valeur = reader.GetString(11);
                        evenement.EtatID = reader.GetInt32(12);
                        evenement.Etat.ID = evenement.EtatID;
                        evenement.Etat.Valeur = reader.GetString(13);
                        evenement.Description = (reader.IsDBNull(14)) ? null : reader.GetString(14);
                        evenement.DateCreation = reader.GetDateTime(15);
                        evenement.DateModification = reader.GetDateTime(16);

                        if (!reader.IsDBNull(18)) evenement.Typologie = _typologieRepository.GetTypologieByID(reader.GetInt32(18));
                        else evenement.Typologie = null;

                        evenement.Sites = _siteRepository.GetSitesByEvent(evenement.ID);
                        evenement.Applications = _applicationRepository.GetApplicationsByEvent(evenement.ID);
                        evenement.GestionInterne = (reader.IsDBNull(20)) ? null : (bool?)reader.GetBoolean(20);
                        evenements.Add(evenement);
                    }
                }
            }
            return evenements.AsQueryable();

        }
    }
}
