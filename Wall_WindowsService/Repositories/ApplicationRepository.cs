using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Wall_WindowsService.Models;

namespace Wall_WindowsService.Repositories
{
    public class ApplicationRepository
    {
        private readonly string _connectionString;

        public ApplicationRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["WalleSdinIntranetDb"].ConnectionString;
        }

        public List<Application> GetApplicationsByEvent(int EvenementID)
        {
            List<Application> applications = new List<Application>();

            string query = "[dbo].[SP_GetApplicationsByEvenement]";

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
                        Application appli = new Application();
                        appli.ID = reader.GetInt32(0);
                        appli.ApplicationName = reader.GetString(1);
                        appli.Entite = reader.GetString(2);
                        appli.NNI = reader.GetString(3);
                        appli.EstActif = reader.GetBoolean(4);
                        applications.Add(appli);
                    }
                }
            }
            return applications;
        }

    }
}
