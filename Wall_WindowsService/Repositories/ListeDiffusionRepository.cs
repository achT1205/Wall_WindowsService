﻿using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Wall_WindowsService.Repositories
{
    public class ListeDiffusionRepository
    {
        private readonly string _connectionString;

        public ListeDiffusionRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["WalleSdinIntranetDb"].ConnectionString;
        }

        public string GetListesDiffusionsByEvenement(int EvenementID)
        {
            //string liste = "";
            //string query = "[dbo].[SP_GetListesDiffusionsByEvenement]";

            //using (SqlConnection con = new SqlConnection(_connectionString))
            //{
            //    using (SqlCommand cmd = new SqlCommand(query, con))
            //    {
            //        con.Open();
            //        cmd.Connection = con;
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.CommandText = query;
            //        var prmEvenementID = new SqlParameter("EvenementID", SqlDbType.Int);
            //        prmEvenementID.Value = EvenementID;
            //        cmd.Parameters.Add(prmEvenementID);

            //        SqlDataReader reader = cmd.ExecuteReader();

            //        while (reader.Read())
            //        {
            //            string diff = reader.GetString(1);
            //            liste = diff + " ; " + liste;
            //        }
            //    }
            //}
            //return liste;
            return "agnes-externe.victor-bala@edf.fr;Isabelle-externe.maurier@edf.fr, Olivier.bot@edf.fr";
        }
    }
}