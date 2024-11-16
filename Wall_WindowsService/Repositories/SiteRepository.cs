using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Wall_WindowsService.Models;

namespace Wall_WindowsService.Repositories
{
    public class SiteRepository
    {
        private readonly string _connectionString;

        public SiteRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["WalleSdinIntranetDb"].ConnectionString;
        }

        public List<Site> GetSitesByEvent(int EvenementID)
        {
            List<Site> sites = new List<Site>();
            string query = "[dbo].[SP_GetSitesByEvenement]";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = query;
                    var prmEnvID = new SqlParameter("EvenementID", SqlDbType.Int);
                    prmEnvID.Value = EvenementID;
                    cmd.Parameters.Add(prmEnvID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Site site = new Site();
                        site.ID = reader.GetInt32(0);
                        site.SiteName = reader.GetString(1);
                        site.NomCourt = reader.GetString(2);
                        site.Direction = reader.GetString(3);
                        sites.Add(site);
                    }
                }
            }
            return sites;
        }

    }
}
