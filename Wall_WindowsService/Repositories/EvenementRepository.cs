using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using Wall_WindowsService.Models;
using System;

namespace Wall_WindowsService.Repositories
{
    public class EvenementRepository
    {
        private readonly string _connectionString;
        private readonly TypologieRepository _typologieRepository;
        private readonly SiteRepository _siteRepository;
        private readonly ApplicationRepository _applicationRepository;

        public EvenementRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["WalleSdinIntranetDb"].ConnectionString;
            _typologieRepository = new TypologieRepository();
            _siteRepository = new SiteRepository();
            _applicationRepository = new ApplicationRepository();
        }


        public int[] GetMailIntervals()
        {
            List<int> intervales = new List<int>();
            try
            {
                string query = "SELECT [IntervalHourBeforeEvent] FROM [dbo].[MailScheduler]";

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.CommandText = query;
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader[0] != DBNull.Value)
                                intervales.Add(reader.GetInt32(0));
                        }
                        reader.Close();
                    }
                }
                return intervales.ToArray();
            }
            catch (Exception ex)
            {

                Logging.Log("ERROR in GetMailIntervals " + ex.Message);

                throw ex;

            }
        }


        public void UpdateGUID(int ID)
        {
            try
            {
                string query = "[dbo].[SP_UpdateEvenementGUID]";

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        con.Open();

                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = query;
                        var prmValeur = new SqlParameter("@ID", SqlDbType.Int);
                        prmValeur.Value = ID;
                        cmd.Parameters.Add(prmValeur);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

                Logging.Log("ERROR in UpdateGUID " + ex.Message);

                throw ex;

            }
        }
        public IQueryable<Evenement> GetEvenementsForMailScheduler(string envName, int interval)
        {
            try
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
                            evenement.Porteur = new Porteur();  
                            evenement.Environnement = new Environnement();

                            evenement.ID = reader.GetInt32(0);
                            evenement.DateDebut = reader.GetDateTime(1);
                            evenement.DateFin = reader.GetDateTime(2);
                            evenement.DateFermeture = (reader.IsDBNull(3)) ? (DateTime?)null : reader.GetDateTime(3); //NULLABLE
                            evenement.Libelle = reader.GetString(4);
                            evenement.Impact = reader.GetString(5);
                            evenement.EnvID = reader.GetInt32(6);
                            evenement.Environnement.ID = evenement.EnvID;
                            evenement.Environnement.Valeur = reader.GetString(7);
                            evenement.PorteurID = reader.GetInt32(8);
                            evenement.Porteur.ID = evenement.PorteurID;
                            evenement.Porteur.Valeur = reader.GetString(9);
                            evenement.PiloteID = reader.GetInt32(10);
                            evenement.Pilote.ID = evenement.PiloteID;
                            evenement.Pilote.Valeur = reader.GetString(11);
                            evenement.EtatID = reader.GetInt32(12);
                            evenement.Etat.ID = evenement.EtatID;
                            evenement.Etat.Valeur = reader.GetString(13);
                            evenement.Description = (reader.IsDBNull(14)) ? null : reader.GetString(14);  //NULLABLE
                            evenement.DateCreation = reader.GetDateTime(15);
                            evenement.DateModification = reader.GetDateTime(16);

                            if (!reader.IsDBNull(18)) evenement.Typologie = _typologieRepository.GetTypologieByID(reader.GetInt32(18));
                            else evenement.Typologie = null;

                            evenement.Sites = _siteRepository.GetSitesByEvent(evenement.ID);
                            evenement.Applications = _applicationRepository.GetApplicationsByEvent(evenement.ID);
                            evenement.GestionInterne = (reader.IsDBNull(20)) ? null : (bool?)reader.GetBoolean(20);
                            evenement.CriticiteMax = reader.GetInt16(21);

                            evenements.Add(evenement);
                        }
                    }
                }

                return evenements.AsQueryable();
            }
            catch (Exception ex)
            {

                Logging.Log("ERROR in GetEvenementsForMailScheduler " + ex.Message);

                throw ex;

            }

        }
    }
}
