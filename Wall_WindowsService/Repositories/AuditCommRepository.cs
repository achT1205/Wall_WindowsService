using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Wall_WindowsService.Models;
using System;
using System.Runtime.Remoting.Contexts;

namespace Wall_WindowsService.Repositories
{
    public class AuditCommRepository
    {
        private readonly string _connectionString;

        public AuditCommRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["WalleSdinIntranetDb"].ConnectionString;
        }

        public void InsertAuditComm(AuditCom auditcom)
        {
            string query = "[dbo].[SP_InsertAuditComm]";

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        con.Open();
                        cmd.Connection = con;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = query;

                        var prmNNI = new SqlParameter("NNI", SqlDbType.NVarChar, 128);
                        prmNNI.Value = auditcom.NNI;
                        cmd.Parameters.Add(prmNNI);

                        var prmDateEnvoi = new SqlParameter("DateEnvoi", SqlDbType.DateTime);
                        prmDateEnvoi.Value = auditcom.DateEnvoi;
                        cmd.Parameters.Add(prmDateEnvoi);

                        var prmObjet = new SqlParameter("Objet", SqlDbType.NVarChar, 255);
                        prmObjet.Value = auditcom.Objet;
                        cmd.Parameters.Add(prmObjet);

                        var prmListeDiffusion = new SqlParameter("ListeDiffusion", SqlDbType.NVarChar);
                        prmListeDiffusion.Value = auditcom.ListeDiffusion;
                        cmd.Parameters.Add(prmListeDiffusion);

                        var prmContenu = new SqlParameter("Contenu", SqlDbType.NVarChar);
                        prmContenu.Value = auditcom.Contenu;
                        cmd.Parameters.Add(prmContenu);

                        var prmIdEvenement = new SqlParameter("IdEvenement", SqlDbType.Int);
                        if (auditcom.IdEvenement == null) prmIdEvenement.Value = DBNull.Value;
                        else prmIdEvenement.Value = auditcom.IdEvenement;
                        cmd.Parameters.Add(prmIdEvenement);

                        var prmIdIncident = new SqlParameter("IdIncident", SqlDbType.Int);
                        if (auditcom.IdIncident == null) prmIdIncident.Value = DBNull.Value;
                        else prmIdIncident.Value = auditcom.IdIncident;

                        cmd.Parameters.Add(prmIdIncident);

                        var prmType = new SqlParameter("Type", SqlDbType.NVarChar, 128);
                        prmType.Value = auditcom.Type;
                        cmd.Parameters.Add(prmType);

                        var prmCommentaire = new SqlParameter("Commentaire", SqlDbType.NVarChar, 2048);
                        prmCommentaire.Value = auditcom.Commentaire;
                        cmd.Parameters.Add(prmCommentaire);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

                Logging.Log("ERROR in InsertAuditComm " + ex.Message);

                throw ex;

            }

        }

        public void InsertAuditComm(string NNI, DateTime date, string Objet, string ListDiffusion, int EvenementId, string contenu, string type, string commentaire)
        {
            AuditCom aud = new AuditCom();
            aud.NNI = NNI;
            aud.DateEnvoi = date;
            aud.Objet = Objet;
            aud.ListeDiffusion = ListDiffusion;
            aud.IdEvenement = EvenementId;
            aud.Contenu = contenu;
            aud.Type = type;
            aud.Commentaire = commentaire;

            InsertAuditComm(aud);
        }
    }
}
