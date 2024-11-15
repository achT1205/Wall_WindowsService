using System;
using System.Net.Mail;
using System.Net;
using System.ServiceProcess;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Management.Instrumentation;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data;
using Wall_WindowsService.Models;
using System.Xml;
using HtmlAgilityPack;

namespace Wall_WindowsService
{
    public partial class EmailService : ServiceBase
    {
        Timer _timer = new Timer();
        private int[] timeIntervals;
        private string _env = "P3NA - Prod. SI Nucléaire";
        private int? _envID = 1;
        private IQueryable<Typologie> _Typologies;

        public EmailService()
        {
            InitializeComponent();
            this.ServiceName = "EmailService";
        }

        protected override void OnStart(string[] args)
        {
            Log("Service started at " + DateTime.Now);
            _timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            _timer.Interval = 10000;
            _timer.Enabled = true;
            _timer.Start();
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Log("Timer triggered.");
            SendEmails();
        }
        private void SendEmails()
        {
            try
            {
                Log("Start sending emails.");
                var intervals = new List<int>() { 64, 168 };
                foreach (var interval in intervals)
                {
                    var events = GetEvenementsForMailScheduler(_env, interval);
                    foreach (var item in events)
                    {
                        SendEmail(item);
                    }
                }


                //foreach (var eventItem in events)
                //{
                //    // Customize the email body with event details
                //    string emailBody = $"Event: {eventItem.EventName}\nDate: {eventItem.EventDate}";

                //    // Send the email
                //    using (var client = new SmtpClient("smtp.example.com"))
                //    {
                //        client.Port = 587;
                //        client.Credentials = new NetworkCredential("username", "password");
                //        client.EnableSsl = true;

                //        var mail = new MailMessage("from@example.com", "to@example.com")
                //        {
                //            Subject = $"Upcoming Event: {eventItem.EventName}",
                //            Body = emailBody
                //        };

                //        client.Send(mail);
                //        Log($"Email sent for event {eventItem.EventName}.");
                //    }
                //}
                Log("End sending emails.");
            }
            catch (Exception ex)
            {
                Log($"Error sending emails: {ex.Message}");
            }
        }


        private void Log(string message)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filePah = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\EmailServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
                if (!File.Exists(filePah))
                {
                    using (StreamWriter sw = new StreamWriter(filePah))
                    {
                        sw.WriteLine($"{DateTime.Now}: {message}");
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filePah))
                    {
                        sw.WriteLine($"{DateTime.Now}: {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log message: {ex.Message}");
            }
        }

        public void SendEmail(Evenement evenement)
        {
            var destinataire = GetListesDiffusionsByEvenement(evenement.ID);
            var objet = GetMailObjet(evenement);
            var htmlbody = GetMailTemplate(evenement);

            Log("BEFORE client.Send(message)");

            if (_env != null)
            {
                int? evID = -1;
                try
                {
                    htmlbody = filterhtml(htmlbody);
                    SmtpClient client = new SmtpClient("localhost", 25); //SmtpClient("mailhost.der.edf.fr");
                    //client.Port = 25;
                    MailAddress from = new MailAddress("wallsdin-noreply@edf.fr");
                    string[] listdestinataires = destinataire.Split(new Char[] { ',', ';' });
                    MailMessage message = new MailMessage();
                    message.From = from;
                    List<string> mailAddresses = new List<string>();
                    foreach (string d in listdestinataires)
                    {
                        if (d.Contains('@') && !mailAddresses.Contains(d.Trim()))
                        {
                            mailAddresses.Add(d.Trim());
                            message.Bcc.Add(new MailAddress(d));
                        }
                    }

                    message.Subject = objet;
                    AlternateView htmlView = LoadImagesIn(htmlbody);
                    message.AlternateViews.Add(htmlView);

 
                    client.Send(message);

                    //var evenementID = Convert.ToInt32(evenement.ID);
                    //evID = evenementID;
                    //htmlbody = htmlbody.Replace("'", "\\'");
                    Log("AFTER client.Send(message) ==========================================================>");





                    //    if (HttpContext != null)
                    //        insertAuditCom(HttpContext.User.Identity.Name, DateTime.Now, objet, destinataire, evenementID, htmlbody,
                    //                        "Envoi de mail", "commentaire de l'incident");
                    //    else
                    //        insertAuditCom("WALL-SdIN", DateTime.Now, objet, destinataire, evenementID, htmlbody,
                    //                    "Envoi de mail", "commentaire de l'incident");
                    //}


                    //EvenementRepository rep = new EvenementRepository();
                    //rep.UpdateGUID(int.Parse(id));
                }
                catch (System.Exception excep)
                {

                }
            }
            else
            {
                //System.Web.Security.FormsAuthentication.SignOut();
                //return RedirectToAction("Index", "Main");

            }
        }

        private string filterhtml(string htmlbody)
        {
            if (htmlbody != null)
            {


                htmlbody = htmlbody.Replace("border=\"1px\"", "border=\"1\"");
                int index = htmlbody.IndexOf("<p");
                if (index > 0) htmlbody = htmlbody.Substring(0, index);

            }
            return htmlbody;
        }


        private AlternateView LoadImagesIn(string htmlbody)
        {

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlbody);
            if (doc == null) return null;


            List<LinkedResource> listLinksImages = new List<LinkedResource>();
            int i = 1;
            foreach (var node in doc.DocumentNode.Descendants("img"))
            {
                string tosplit = node.GetAttributeValue("src", null);
                string[] image64List = Regex.Split(tosplit, ";base64,");
                System.IO.Stream imageStream = LoadImage(image64List[1]);
                LinkedResource imageLink = new LinkedResource(imageStream);
                imageLink.ContentId = "image" + i;
                string mimeType = Regex.Split(image64List[0], "data:")[1];
                imageLink.ContentType = new System.Net.Mime.ContentType(mimeType);
                listLinksImages.Add(imageLink);
                node.SetAttributeValue("src", "cid:" + imageLink.ContentId);
                i++;
            }
            htmlbody = doc.DocumentNode.OuterHtml;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlbody);
            htmlView.ContentType = new System.Net.Mime.ContentType("text/html");

            foreach (LinkedResource imageLink in listLinksImages)
            {
                htmlView.LinkedResources.Add(imageLink);
            }
            return htmlView;

        }

        public System.IO.Stream LoadImage(string image64)
        {
            byte[] bytes = Convert.FromBase64String(image64);
            MemoryStream ms = new MemoryStream(bytes);
            return ms;
        }

        public string GetListesDiffusionsByEvenement(int EvenementID)
        {
            string liste = "";
            string diff = "achilletuglo12@gmail.com";//"agnes-externe.victor-bala@edf.fr;Isabelle-externe.maurier@edf.fr, Olivier.bot@edf.fr";
            liste = diff + " ; " + liste;

            //string query = "[dbo].[SP_GetListesDiffusionsByEvenement]";

            //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WalleSdinIntranetDb"].ConnectionString))
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
            return liste;
        }
        private string GetMailObjet(Evenement evenement)
        {
            string labelApplications = "";
            int nbApplis = 0;
            for (int i = 0; i < evenement.Applications.Count(); i++)
            {
                if ((evenement.Applications.Count() == 1) || (nbApplis == evenement.Applications.Count() - 1)) labelApplications = labelApplications + evenement.Applications[i].ApplicationName;
                else labelApplications = labelApplications + evenement.Applications[i].ApplicationName + ", ";
                nbApplis = nbApplis + 1;
            }
            string labelSites = "";
            int nbSites = 0;
            for (int i = 0; i < evenement.Sites.Count(); i++)
            {
                if ((evenement.Sites.Count() == 1) || (nbSites == evenement.Sites.Count() - 1)) labelSites = labelSites + evenement.Sites[i].SiteName;
                else labelSites = labelSites + evenement.Sites[i].NomCourt + ", ";
                nbSites = nbSites + 1;
            }

            return "[Opération planifiée - " + evenement.Etat.Valeur + "] " + labelApplications + " - " + labelSites + " - " + evenement.Libelle;

        }
        public string GetMailTemplate(Evenement evenement)
        {
            string pathtemplate, imagesPath;

            pathtemplate = AppDomain.CurrentDomain.BaseDirectory + "/Resources/MailGmailUtf8Data.html";
            imagesPath = AppDomain.CurrentDomain.BaseDirectory + "/Resources/Images/email_template/";

            string htmlbody = System.IO.File.ReadAllText(pathtemplate);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlbody);

            HtmlNode nodeheaderImage = doc.GetElementbyId("headerImage");
            string headerImagepath = "";
            if (_envID == 1)
            {
                headerImagepath = imagesPath + "header_p3na.jpg";
                byte[] headerImageBytes = System.IO.File.ReadAllBytes(headerImagepath);
                string srcheader = "data:image/jpeg;base64," + Convert.ToBase64String(headerImageBytes);
                nodeheaderImage.SetAttributeValue("src", srcheader);
            }
            else if (_envID == 3)
            {
                headerImagepath = imagesPath + "header_u3na.jpg";
                byte[] headerImageBytes = System.IO.File.ReadAllBytes(headerImagepath);
                string srcheader = "data:image/jpeg;base64," + Convert.ToBase64String(headerImageBytes);
                nodeheaderImage.SetAttributeValue("src", srcheader);
            }


            HtmlNode nodebarreInfo = doc.GetElementbyId("barreInfo");
            string barreInfopath = imagesPath + "barre-info-op-non.jpg";
            if (evenement.CriticiteMax > 0) barreInfopath = imagesPath + "barre-info-op-imp.jpg";

            byte[] barreInfoBytes = System.IO.File.ReadAllBytes(barreInfopath);
            string src = "data:image/jpeg;base64," + Convert.ToBase64String(barreInfoBytes);
            nodebarreInfo.SetAttributeValue("src", src);

            HtmlNode nodelabelEtat = doc.GetElementbyId("labelEtat");
            nodelabelEtat.InnerHtml = evenement.Etat.Valeur;
            HtmlNode nodelabelDateDebut = doc.GetElementbyId("labelDateDebut");
            string dateDebut = evenement.DateDebut.ToShortDateString() + " à " + evenement.DateDebut.ToShortTimeString();
            nodelabelDateDebut.InnerHtml = dateDebut;

            HtmlNode nodelabelDateFin = doc.GetElementbyId("labelDateFin");
            if (evenement.DateFin != null)
            {
                string dateFin = ((DateTime)evenement.DateFin).ToShortDateString() + " à " + ((DateTime)evenement.DateFin).ToShortTimeString();
                nodelabelDateFin.InnerHtml = dateFin;
            }
            else nodelabelDateFin.InnerHtml = "";

            HtmlNode nodelabelApplications = doc.GetElementbyId("labelApplications");
            string labelApplications = "";
            int nbApplis = 0;
            for (int i = 0; i < evenement.Applications.Count(); i++)
            {
                if ((evenement.Applications.Count() == 1) || (nbApplis == evenement.Applications.Count() - 1)) labelApplications = labelApplications + evenement.Applications[i].ApplicationName;
                else labelApplications = labelApplications + evenement.Applications[i].ApplicationName + ", ";
                nbApplis = nbApplis + 1;
            }
            nodelabelApplications.InnerHtml = labelApplications;


            HtmlNode nodelabelSites = doc.GetElementbyId("labelSites");
            string labelSites = "";
            int nbSites = 0;
            for (int i = 0; i < evenement.Sites.Count(); i++)
            {
                if ((evenement.Sites.Count() == 1) || (nbSites == evenement.Sites.Count() - 1)) labelSites = labelSites + evenement.Sites[i].SiteName;
                else labelSites = labelSites + evenement.Sites[i].SiteName + ", ";
                nbSites = nbSites + 1;
            }
            nodelabelSites.InnerHtml = labelSites;

            HtmlNode nodeLabelLibele = doc.GetElementbyId("LabelLibele");
            nodeLabelLibele.InnerHtml = evenement.Libelle;

            HtmlNode nodeLabelDescription = doc.GetElementbyId("LabelDescription");
            if (evenement.Description == null) evenement.Description = "";
            nodeLabelDescription.InnerHtml = evenement.Description.Replace("\n", "<br/>").Replace("  ", " &nbsp;");

            HtmlNode nodelabelTypologie = doc.GetElementbyId("labelTypologie");
            if (evenement.Typologie != null) nodelabelTypologie.InnerHtml = evenement.Typologie.TypologieNom;

            HtmlNode nodelabelPilote = doc.GetElementbyId("labelPilote");
            nodelabelPilote.InnerHtml = evenement.Pilote.Valeur;

            HtmlNode nodeimpact1 = doc.GetElementbyId("impact1");

            HtmlNode nodeimpactTable = doc.GetElementbyId("impactTable");
            HtmlNode nodeimpactSuivant = doc.GetElementbyId("impactSuivant");
            HtmlNode nodeimpactSuivantClone = nodeimpactSuivant.CloneNode(true);
            string originalimpactSuivantHTML = nodeimpactSuivant.InnerHtml;

            nodeimpactSuivant.Remove();
            if ((evenement.ListImpacts != null) && (evenement.ListImpacts.Count() > 0))
            {
                setImpact(evenement.ListImpacts[0], nodeimpact1);
                for (int i = 1; i < evenement.ListImpacts.Count(); i++)
                {
                    nodeimpactTable.AppendChild(nodeimpactSuivantClone);
                    setImpact(evenement.ListImpacts[i], nodeimpactSuivantClone);
                    nodeimpactSuivantClone = nodeimpactSuivantClone.CloneNode(true);
                    nodeimpactSuivantClone.InnerHtml = originalimpactSuivantHTML;
                }

            }
            SetLabelColors(doc, evenement.CriticiteMax);
            htmlbody = doc.DocumentNode.OuterHtml;
            return htmlbody;
        }
        private void setImpact(string impact, HtmlNode nodeimpact)
        {
            int prio = getPrio(impact);
            string impactLabel = impact;

            if (impact.IndexOf("[]") != -1)
            {
                impactLabel = impactLabel.Replace("[]", "");
            }
            else if (impact.IndexOf("[+]") != -1)
            {
                impactLabel = impactLabel.Replace("[+]", "");
            }
            else if (impact.IndexOf("[++]") != -1)
            {
                impactLabel = impactLabel.Replace("[++]", "");
            }

            impactLabel = (impactLabel == "") ? " " : impactLabel;

            string imagesPath;
            imagesPath = AppDomain.CurrentDomain.BaseDirectory + "/Resources/Images/email_template/";
            string critiquepath = "";
            string src = "data:image/jpeg;base64,";
            if (prio == 1) critiquepath = imagesPath + "critique1.jpg";
            else if (prio == 2) critiquepath = imagesPath + "critique2.jpg";
            else if (prio == 3) critiquepath = imagesPath + "critique3.jpg";
            if (prio > 0)
            {
                byte[] critiqueBytes = System.IO.File.ReadAllBytes(critiquepath);
                src = src + Convert.ToBase64String(critiqueBytes);
                List<HtmlNode> listNodes = new List<HtmlNode>();
                foreach (var node in nodeimpact.Descendants("img"))
                {
                    listNodes.Add(node);
                }

                foreach (var node in listNodes)
                {
                    node.SetAttributeValue("src", src);
                    node.ParentNode.InsertAfter(HtmlNode.CreateNode(impactLabel), node);
                }
            }
            else
            {
                List<HtmlNode> listNodes = new List<HtmlNode>();
                foreach (var node in nodeimpact.Descendants("img"))
                {
                    listNodes.Add(node);
                }

                foreach (var node in listNodes)
                {

                    node.ParentNode.InsertAfter(HtmlNode.CreateNode(impactLabel), node);
                    node.Remove();
                }
            }
        }
        private void SetLabelColors(HtmlDocument doc, int CriticiteMax)
        {
            string style = "width:20%;font-family:arial;color:#509E2F;font-size:13px;";

            if (CriticiteMax > 0) style = "width:20%;font-family:arial;color:#FE5815;font-size:13px;";


            foreach (var node in doc.DocumentNode.Descendants("td"))
            {
                if (node.Id == "label") node.SetAttributeValue("style", style);
            }
        }
        private int getPrio(string impact)
        {
            int prio = 0;
            if (impact.Contains("[++]")) prio = 3;
            else if (impact.Contains("[+]")) prio = 2;
            else if (impact.Contains("[]")) prio = 1;
            return prio;
        }


        public IQueryable<Evenement> GetEvenementsForMailScheduler(string envName, int interval)
        {
            List<Evenement> evenements = new List<Evenement>();
            var etat1 = new Etat() { ID = 1, Valeur = "Fermé" };
            var etat2 = new Etat() { ID = 1, Valeur = "Fermé" };
            var etat3 = new Etat() { ID = 1, Valeur = "Fermé" };

            var pilote = new Pilote() { ID = 1, Valeur = "Pilote" };
            var typology = new Typologie() { ID = 1, Nomcourt = "", Type_SIVISION = "", TypologieNom = "" };


            List<Evenement> Evenements = new List<Evenement>
            {
                new Evenement
                {
                    ID = 30167,
                    DateDebut = DateTime.Parse("2022-06-08 12:00:00"),
                    DateFin = DateTime.Parse("2022-06-08 12:25:00"),
                    DateFermeture = DateTime.Parse("2022-06-08 12:32:17.597"),
                    Libelle = "Opération de maintenance",
                    Impact = "[++] Indisponibilité de l'application",
                    EnvID = 1,
                    PiloteID = 2,
                    Pilote = pilote,
                    EtatID = 3,
                    Etat = etat1,
                    Description = "L'opération technique sur l'application FORQUART, prévue le 08/06/2022 de 12h00 à 14h00 est terminée. L'application est de nouveau pleinement disponible.",
                    DateCreation = DateTime.Parse("2022-06-03 15:21:21.267"),
                    DateModification = DateTime.Parse("2022-06-08 12:32:34.150"),
                    Typologie = getTypologieByID(2),
                    GestionInterne = false,
                    Sites =  GetSitesByEvent(30167),
                    Applications = GetApplicationsByEvent(30167)
                },
                 new Evenement
                {
                    ID = 30167,
                    DateDebut = DateTime.Parse("2022-06-08 12:00:00"),
                    DateFin = DateTime.Parse("2022-06-08 12:25:00"),
                    DateFermeture = DateTime.Parse("2022-06-08 12:32:17.597"),
                    Libelle = "Opération de maintenance",
                    Impact = "[++] Indisponibilité de l'application",
                    EnvID = 1,
                    PiloteID = 2,
                    Pilote = pilote,
                    EtatID = 3,
                    Etat = etat1,
                    Description = "L'opération technique sur l'application FORQUART, prévue le 08/06/2022 de 12h00 à 14h00 est terminée. L'application est de nouveau pleinement disponible.",
                    DateCreation = DateTime.Parse("2022-06-03 15:21:21.267"),
                    DateModification = DateTime.Parse("2022-06-08 12:32:34.150"),
                    Typologie = getTypologieByID(1),
                    GestionInterne = false,
                    Sites =  GetSitesByEvent(30167),
                    Applications = GetApplicationsByEvent(30167)
                }
            };
            //string query = "[dbo].[sp_GetEvenementsForMailScheduler]";

            //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WalleSdinIntranetDb"].ConnectionString))
            //{
            //    using (SqlCommand cmd = new SqlCommand(query, con))
            //    {
            //        con.Open();
            //        cmd.Connection = con;
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.CommandText = query;

            //        var prmEnvName = new SqlParameter("EnvName", SqlDbType.NVarChar, 100);
            //        prmEnvName.Value = envName;
            //        cmd.Parameters.Add(prmEnvName);
            //        cmd.Parameters.Add("IntervalHours", SqlDbType.Int).Value = interval;

            //        SqlDataReader reader = cmd.ExecuteReader();

            //        while (reader.Read())
            //        {
            //            Evenement evenement = new Evenement();
            //            evenement.Etat = new Etat();

            //            evenement.Pilote = new Pilote();

            //            evenement.ID = reader.GetInt32(0);
            //            evenement.DateDebut = reader.GetDateTime(1);
            //            evenement.DateFin = reader.GetDateTime(2);
            //            evenement.Libelle = reader.GetString(4);
            //            evenement.PiloteID = reader.GetInt32(10);
            //            evenement.Pilote.ID = evenement.PiloteID;
            //            evenement.Pilote.Valeur = reader.GetString(11);
            //            evenement.EtatID = reader.GetInt32(12);
            //            evenement.Etat.ID = evenement.EtatID;
            //            evenement.Etat.Valeur = reader.GetString(13);
            //            evenement.Description = (reader.IsDBNull(14)) ? null : reader.GetString(14);
            //            evenement.DateCreation = reader.GetDateTime(15);
            //            evenement.DateModification = reader.GetDateTime(16);

            //            if (!reader.IsDBNull(18)) evenement.Typologie = getTypologieByID(reader.GetInt32(18));
            //            else evenement.Typologie = null;

            //            evenement.Sites = GetSitesByEvent(evenement.ID);
            //            evenement.Applications = GetApplicationsByEvent(evenement.ID);
            //            evenement.GestionInterne = (reader.IsDBNull(20)) ? null : (bool?)reader.GetBoolean(20);
            //            evenements.Add(evenement);
            //        }
            //    }
            //}
            //return evenements.AsQueryable();

            return Evenements.AsQueryable();
        }

        private Typologie getTypologieByID(int? typoID)
        {
            if (typoID != null)
            {
                if (_Typologies == null) _Typologies = GetTypologies();
                if (_Typologies != null) return _Typologies.Where(t => t.ID == typoID).First();
                else return null;
            }
            else return null;
        }

        public IQueryable<Typologie> GetTypologies()
        {
            //List<Typologie> typos = new List<Typologie>();
            //string query = "[dbo].[SP_GetTypologies]";

            //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WalleSdinIntranetDb"].ConnectionString))
            //{
            //    using (SqlCommand cmd = new SqlCommand(query, con))
            //    {
            //        con.Open();
            //        cmd.Connection = con;
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.CommandText = query;

            //        SqlDataReader reader = cmd.ExecuteReader();

            //        while (reader.Read())
            //        {
            //            Typologie typo = new Typologie();
            //            typo.ID = reader.GetInt32(0);
            //            typo.TypologieNom = reader.GetString(1);
            //            typo.Nomcourt = reader.GetString(2);
            //            if (reader["Type_SIVISION"] != DBNull.Value)
            //                typo.Type_SIVISION = reader.GetString(3);
            //            typos.Add(typo);
            //        }
            //    }
            //}
            //return typos.AsQueryable();

            List<Typologie> Typologies = new List<Typologie>
            {
                new Typologie
                {
                    ID = 1,
                    TypologieNom = "Applicatif",
                    Nomcourt = "Appli",
                    Type_SIVISION = "Changement fonctionnel"
                },
                new Typologie
                {
                    ID = 2,
                    TypologieNom = "Infrastructure",
                    Nomcourt = "Infra",
                    Type_SIVISION = "Changement infrastructure"
                },
                new Typologie
                {
                    ID = 3,
                    TypologieNom = "Réseau",
                    Nomcourt = "Réseau",
                    Type_SIVISION = "Changement technique"
                },
                new Typologie
                {
                    ID = 4,
                    TypologieNom = "Stockage",
                    Nomcourt = "Stockage",
                    Type_SIVISION = "Changement technique"
                },
                new Typologie
                {
                    ID = 5,
                    TypologieNom = "Flux",
                    Nomcourt = "Flux",
                    Type_SIVISION = "Changement technique"
                },
                new Typologie
                {
                    ID = 6,
                    TypologieNom = "Divers",
                    Nomcourt = "Divers",
                    Type_SIVISION = null
                }
            };

            return Typologies.AsQueryable();

        }


        public List<Site> GetSitesByEvent(int EvenementID)
        {
            //List<Site> sites = new List<Site>();
            //string query = "[dbo].[SP_GetSitesByEvenement]";

            //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WalleSdinIntranetDb"].ConnectionString))
            //{
            //    using (SqlCommand cmd = new SqlCommand(query, con))
            //    {
            //        con.Open();
            //        cmd.Connection = con;
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.CommandText = query;
            //        var prmEnvID = new SqlParameter("EvenementID", SqlDbType.Int);
            //        prmEnvID.Value = EvenementID;
            //        cmd.Parameters.Add(prmEnvID);
            //        SqlDataReader reader = cmd.ExecuteReader();

            //        while (reader.Read())
            //        {
            //            Site site = new Site();
            //            site.ID = reader.GetInt32(0);
            //            site.SiteName = reader.GetString(1);
            //            site.NomCourt = reader.GetString(2);
            //            site.Direction = reader.GetString(3);
            //            sites.Add(site);
            //        }
            //    }
            //}
            //return sites;

            List<Site> Sites = new List<Site>
            {
                new Site
                {
                    ID = 34,
                    SiteName = "Tous les sites",
                    NomCourt = "Tous",
                    Direction = "Tous"
                },
                new Site
                {
                    ID = 1,
                    SiteName = "Belleville",
                    NomCourt = "BEL",
                    Direction = "CNPE"
                },
                new Site
                {
                    ID = 2,
                    SiteName = "Blayais",
                    NomCourt = "BLA",
                    Direction = "CNPE"
                },
                new Site
                {
                    ID = 22,
                    SiteName = "Brennillis",
                    NomCourt = "BRE",
                    Direction = "CNPE"
                },
                new Site
                {
                    ID = 17,
                    SiteName = "Penly",
                    NomCourt = "PEN",
                    Direction = "CNPE"
                },
                new Site
                {
                    ID = 13,
                    SiteName = "Golfech",
                    NomCourt = "GOL",
                    Direction = "CNPE"
                },
                new Site
                {
                    ID = 12,
                    SiteName = "Flamanville 3",
                    NomCourt = "FA3",
                    Direction = "CNPE"
                }
            };

            return Sites;
        }


        public List<Application> GetApplicationsByEvent(int EvenementID)
        {
            //List<Application> applications = new List<Application>();

            List<Application> Applications = new List<Application>
        {
            new Application { ID = 1, ApplicationName = "EAM", Entite = "SI rénové", NNI = "EAM", EstActif = true },
            new Application { ID = 4, ApplicationName = "BI", Entite = "SI rénové", NNI = "INF", EstActif = true },
            new Application { ID = 6, ApplicationName = "MRS", Entite = "SI rénové", NNI = "MRS", EstActif = true },
            new Application { ID = 7, ApplicationName = "GPS", Entite = "SI rénové", NNI = "GPS", EstActif = true },
            new Application { ID = 9, ApplicationName = "GMO2", Entite = "SI rénové", NNI = "GM2", EstActif = true },
            new Application { ID = 12, ApplicationName = "Micado 5", Entite = "SI rénové", NNI = "MCO", EstActif = true },
            new Application { ID = 13, ApplicationName = "DOSIAP", Entite = "SI rénové", NNI = "DSA", EstActif = true },
            new Application { ID = 16, ApplicationName = "Epsilon 2", Entite = "SI rénové", NNI = "EPS", EstActif = true },
            new Application { ID = 17, ApplicationName = "AP913", Entite = "SI rénové", NNI = "AP9", EstActif = true },
            new Application { ID = 20, ApplicationName = "Espace", Entite = "SI existant", NNI = "BLC", EstActif = true },
            new Application { ID = 21, ApplicationName = "Merlin", Entite = "SI existant", NNI = "MLN", EstActif = true },
            new Application { ID = 22, ApplicationName = "Cahier de Quart", Entite = "SI existant", NNI = "CDQ", EstActif = true },
            new Application { ID = 23, ApplicationName = "PDME", Entite = "SI existant", NNI = "old", EstActif = true },
            new Application { ID = 24, ApplicationName = "AIC", Entite = "SI existant", NNI = "AIC", EstActif = true }
        };
            //string query = "[dbo].[SP_GetApplicationsByEvenement]";

            //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WalleSdinIntranetDb"].ConnectionString))
            //{
            //    using (SqlCommand cmd = new SqlCommand(query, con))
            //    {
            //        con.Open();
            //        cmd.Connection = con;
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.CommandText = query;
            //        var prmEnvID = new SqlParameter("EvenementID", SqlDbType.Int);
            //        prmEnvID.Value = EvenementID;
            //        cmd.Parameters.Add(prmEnvID);
            //        SqlDataReader reader = cmd.ExecuteReader();

            //        while (reader.Read())
            //        {
            //            Application appli = new Application();
            //            appli.ID = reader.GetInt32(0);
            //            appli.ApplicationName = reader.GetString(1);
            //            appli.Entite = reader.GetString(2);
            //            appli.NNI = reader.GetString(3);
            //            appli.EstActif = reader.GetBoolean(4);
            //            applications.Add(appli);
            //        }
            //    }
            //}
            //return applications;

            return Applications;
        }
    }
}
