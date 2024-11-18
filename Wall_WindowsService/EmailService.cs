using System;
using System.Net.Mail;
using System.ServiceProcess;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Wall_WindowsService.Models;
using HtmlAgilityPack;
using Wall_WindowsService.Repositories;
using System.Management.Instrumentation;

namespace Wall_WindowsService
{
    public partial class EmailService : ServiceBase
    {
        Timer _timer = new Timer();
        private string _env = "P3NA - Prod. SI Nucléaire";
        private int _envID = 1;
        private readonly EvenementRepository _evenementRepository;
        private readonly ListeDiffusionRepository _listeDiffusionRepository;
        private readonly AuditCommRepository _auditCommRepository;
        public EmailService()
        {
            InitializeComponent();
            this.ServiceName = "EmailService";
            _evenementRepository = new EvenementRepository();
            _listeDiffusionRepository = new ListeDiffusionRepository();
            _auditCommRepository = new AuditCommRepository();
        }

        protected override void OnStart(string[] args)
        {
            Logging.Log("Service started at " + DateTime.Now);
            _timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            _timer.Interval = 60 * 1000; // Wait for 1 minute
            _timer.Enabled = true;
            _timer.Start();
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _timer.Dispose();
            Logging.Log("Service stoped at " + DateTime.Now);
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Logging.Log("Timer triggered.");
            SendEmails();
        }
        private void SendEmails()
        {
            try
            {
                Logging.Log("START emails process.");
                var intervals = _evenementRepository.GetMailIntervals();

                Logging.Log("Total intervals found : ==> : " + intervals.Count());
                int count = 0;
                foreach (var interval in intervals)
                {
                    Logging.Log($"Getting potemtial events for the period  {interval} hours: ==> : ");
                    var events = _evenementRepository.GetEvenementsForMailScheduler(_env, interval);

                    Logging.Log($"{events.Count()} events found for the period  {interval} hours:");

                    foreach (var item in events)
                    {
                        Logging.Log("START Traitement for Event : ==> : " + item.ID);

                        SendEmail(item);
                        count++;

                        Logging.Log("END Traitement for Event : ==> : " + item.ID);

                    }
                }
                Logging.Log($"END emails process, {count} emails sent in total.");
            }
            catch (Exception ex)
            {
                Logging.Log($"Error sending emails: {ex.Message}");
            }
        }
        public void SendEmail(Evenement evenement)
        {
            var destinataire = _listeDiffusionRepository.GetListesDiffusionsByEvenement(evenement.ID);
            var objet = GetMailObjet(evenement);
            var htmlbody = GetMailTemplate(evenement);
            Logging.Log("BEFORE sending email from Event " + evenement.ID.ToString());


            try
            {
                htmlbody = filterhtml(htmlbody);
                SmtpClient client = new SmtpClient("mailhost.der.edf.fr", 25); //SmtpClient("localhost", 25); //

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


                Logging.Log("BEFORE sending email from Event " + evenement.ID.ToString());

                htmlbody = htmlbody.Replace("'", "\\'");


                _auditCommRepository.InsertAuditComm("WALL-SdIN", DateTime.Now, objet, destinataire, evenement.ID, htmlbody,
                                "Envoi de mail", "commentaire de l'incident");

                _evenementRepository.UpdateGUID(evenement.ID);
            }
            catch (System.Exception excep)
            {

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
            //if (_envID == 1)
            //{
            headerImagepath = imagesPath + "header_p3na.jpg";
            byte[] headerImageBytes = System.IO.File.ReadAllBytes(headerImagepath);
            string srcheader = "data:image/jpeg;base64," + Convert.ToBase64String(headerImageBytes);
            nodeheaderImage.SetAttributeValue("src", srcheader);
            //}
            //else if (_envID == 3)
            //{
            //    headerImagepath = imagesPath + "header_u3na.jpg";
            //    byte[] headerImageBytes = System.IO.File.ReadAllBytes(headerImagepath);
            //    string srcheader = "data:image/jpeg;base64," + Convert.ToBase64String(headerImageBytes);
            //    nodeheaderImage.SetAttributeValue("src", srcheader);
            //}


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
    }
}
