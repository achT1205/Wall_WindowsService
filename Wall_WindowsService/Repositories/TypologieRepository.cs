using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using Wall_WindowsService.Models;
using System;

namespace Wall_WindowsService.Repositories
{
    public class TypologieRepository
    {
        private readonly string _connectionString;

        public TypologieRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["WalleSdinIntranetDb"].ConnectionString;
        }

        public IQueryable<Typologie> GetTypologies()
        {
            List<Typologie> typos = new List<Typologie>();

            string query = "[dbo].[SP_GetTypologies]";

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = query;

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            Typologie typo = new Typologie();
                            typo.ID = reader.GetInt32(0);
                            typo.TypologieNom = reader.GetString(1);
                            typo.Nomcourt = reader.GetString(2);
                            if (reader["Type_SIVISION"] != DBNull.Value)
                                typo.Type_SIVISION = reader.GetString(3);
                            typos.Add(typo);
                        }
                    }
                }
                return typos.AsQueryable();
            }
            catch (Exception ex)
            {

                Logging.Log("ERROR in GetTypologies " + ex.Message);

                throw ex;

            }
        }


        public Typologie GetTypologieByID(int? typoId)
        {
            if (typoId == null)
                return null;
            var typologies = GetTypologies();
            return typologies.FirstOrDefault(t => t.ID == typoId);
        }
    }
}
