using Polizia.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Polizia.Controllers
{
    public class HomeController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Polizia"].ToString();

        //Azione per visualizzare l'elenco dei trasgressori
        public ActionResult Anagrafica()
        {
            List<Anagrafica> trasgressori = GetTrasgressori();
            return View(trasgressori);
        }


        //Azione per visualizzare l'elenco delle violazioni
        public ActionResult ElencoViolazioni()
        {
            List<Verbale> elencoViolazioni = GetElencoViolazioni();
            return View(elencoViolazioni);
        }


        // Azione per compilare un nuovo verbale (GET)
        public ActionResult CompilaVerbale()
        {
            // Recupera l'elenco dei trasgressori per popolare la vista
            ViewBag.Anagrafiche = GetTrasgressori();

            // Recupera l'elenco dei tipi di violazione per popolare la vista
            ViewBag.TipiViolazione = GetElencoViolazioni();

            return View();
        }

        // Azione per compilare un nuovo verbale (POST)
        [HttpPost]
        public ActionResult CompilaVerbale(Verbale verbale)
        {
            if (ModelState.IsValid)
            {
                // salva il verbale nel DB
                SaveVerbaleToDatabase(verbale);
                return RedirectToAction("Anagrafica");
            }

            // Se il modello non è valido, ripopola le dropdowns
            ViewBag.Anagrafiche = GetTrasgressori();
            ViewBag.TipiViolazione = GetElencoViolazioni();

            return View(verbale);
        }


        // Metodo privato per recuperare l'elenco dei trasgressori dal DB
        private List<Anagrafica> GetTrasgressori()
        {
            List<Anagrafica> trasgressori = new List<Anagrafica>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM ANAGRAFICA";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Crea un oggetto Anagrafica e lo aggiunge alla lista
                        Anagrafica trasgressore = new Anagrafica
                        {
                            IdAnagrafica = Convert.ToInt32(reader["idanagrafica"]),
                            Cognome = reader["Cognome"].ToString(),
                            Nome = reader["Nome"].ToString(),
                            Indirizzo = reader["Indirizzo"].ToString(),
                            Citta = reader["Città"].ToString(),
                            CAP = reader["CAP"].ToString(),
                            CF = reader["CF"].ToString()
                        };

                        trasgressori.Add(trasgressore);
                    }
                }
            }
            // Restituisce la lista degli oggetti Anagrafica
            return trasgressori;
        }

        // Metodo per recuperare l'elenco delle violazioni dal database
        private List<Verbale> GetElencoViolazioni()
        {
            List<Verbale> elencoViolazioni = new List<Verbale>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Verbale";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Crea un oggetto Verbale e lo aggiunge alla lista
                        Verbale violazione = new Verbale
                        {
                            IdVerbale = Convert.ToInt32(reader["IdVerbale"]),
                            DataViolazione = reader.GetDateTime(reader.GetOrdinal("DataViolazione")),
                            IndirizzoViolazione = reader["IndirizzoViolazione"].ToString(),
                            NominativoAgente = reader["NominativoAgente"].ToString(),
                            DataTrascrizioneVerbale = reader.GetDateTime(reader.GetOrdinal("DataTrascrizioneVerbale")),
                            Importo = reader.GetDecimal(reader.GetOrdinal("Importo")),
                            DecurtamentoPunti = Convert.ToInt32(reader["DecurtamentoPunti"]),
                            IdAnagrafica = Convert.ToInt32(reader["IdAnagrafica"]),
                            IdViolazione = Convert.ToInt32(reader["IdViolazione"])

                        };
                        // Restituisce la lista degli oggetti Verbale
                        elencoViolazioni.Add(violazione);
                    }
                }
            }

            return elencoViolazioni;
        }


        // Metodo per salvare un verbale nel database
        private void SaveVerbaleToDatabase(Verbale verbale)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO VERBALE (DataViolazione, IndirizzoViolazione, NominativoAgente, DataTrascrizioneVerbale, Importo, DecurtamentoPunti, idanagrafica, idviolazione) " +
                               "VALUES (@DataViolazione, @IndirizzoViolazione, @NominativoAgente, @DataTrascrizioneVerbale, @Importo, @DecurtamentoPunti, @idanagrafica, @idviolazione)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DataViolazione", verbale.DataViolazione);
                    cmd.Parameters.AddWithValue("@IndirizzoViolazione", verbale.IndirizzoViolazione);
                    cmd.Parameters.AddWithValue("@NominativoAgente", verbale.NominativoAgente);
                    cmd.Parameters.AddWithValue("@DataTrascrizioneVerbale", verbale.DataTrascrizioneVerbale);
                    cmd.Parameters.AddWithValue("@Importo", verbale.Importo);
                    cmd.Parameters.AddWithValue("@DecurtamentoPunti", verbale.DecurtamentoPunti);
                    cmd.Parameters.AddWithValue("@idanagrafica", verbale.IdAnagrafica);
                    cmd.Parameters.AddWithValue("@idviolazione", verbale.IdViolazione);

                    // Esegue la query di inserimento
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Azione per visualizzare l'elenco dei verbali per trasgressore
        public ActionResult VerbaliPerTrasgressore()
        {
            List<VerbaliPerTrasgressore> verbaliPerTrasgressore = GetVerbaliPerTrasgressore();
            return View(verbaliPerTrasgressore);
        }


        // Metodo privato per ottenere l'elenco dei verbali per trasgressore dal database
        private List<VerbaliPerTrasgressore> GetVerbaliPerTrasgressore()
        {

            // Lista che conterrà gli oggetti VerbaliPerTrasgressore
            List<VerbaliPerTrasgressore> verbaliPerTrasgressore = new List<VerbaliPerTrasgressore>();

            // Recupera l'elenco dei trasgressori
            List<Anagrafica> trasgressori = GetTrasgressori();

            // Itera sui trasgressori
            foreach (Anagrafica trasgressore in trasgressori)
            {
                // Per ogni trasgressore, recupera i verbali associati
                List<Verbale> verbali = GetVerbaliPerTrasgressore(trasgressore.IdAnagrafica);

                // Aggiungi i dati alla vista
                VerbaliPerTrasgressore verbale = new VerbaliPerTrasgressore
                {
                    NomeTrasgressore = $"{trasgressore.Nome} {trasgressore.Cognome}",
                    Verbali = verbali
                };

                verbaliPerTrasgressore.Add(verbale);
            }

            // Restituisce la lista degli oggetti VerbaliPerTrasgressore
            return verbaliPerTrasgressore;
        }

        // Metodo  per ottenere l'elenco dei verbali associati a un trasgressore dal DB
        private List<Verbale> GetVerbaliPerTrasgressore(int idAnagrafica)
        {
            List<Verbale> verbali = new List<Verbale>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Query  per selezionare i dati dei verbali associati a un trasgressore
                string query = @"
            SELECT v.*
            FROM VERBALE v
            JOIN ANAGRAFICA a ON v.idanagrafica = a.idanagrafica
            WHERE a.idanagrafica = @IdAnagrafica";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    // Imposta i parametri per la query con l'ID del trasgressore
                    cmd.Parameters.AddWithValue("@IdAnagrafica", idAnagrafica);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Verbale verbale = new Verbale
                            {
                                IdVerbale = Convert.ToInt32(reader["IdVerbale"]),
                                DataViolazione = reader.GetDateTime(reader.GetOrdinal("DataViolazione")),
                                IndirizzoViolazione = reader["IndirizzoViolazione"].ToString(),
                                NominativoAgente = reader["NominativoAgente"].ToString(),
                                DataTrascrizioneVerbale = reader.GetDateTime(reader.GetOrdinal("DataTrascrizioneVerbale")),
                                Importo = reader.GetDecimal(reader.GetOrdinal("Importo")),
                                DecurtamentoPunti = Convert.ToInt32(reader["DecurtamentoPunti"]),
                                IdAnagrafica = Convert.ToInt32(reader["IdAnagrafica"]),
                                IdViolazione = Convert.ToInt32(reader["IdViolazione"])
                            };

                            verbali.Add(verbale);
                        }
                    }
                }
            }

            return verbali;
        }

        // Azione per visualizzare il totale dei punti decurtati per trasgressore
        public ActionResult PuntiDecurtatiPerTrasgressore()
        {
            List<PuntiDecurtatiPerTrasgressore> puntiDecurtatiPerTrasgressore = GetPuntiDecurtatiPerTrasgressore();
            return View(puntiDecurtatiPerTrasgressore);
        }

        // Metodo  per ottenere l'elenco dei punti decurtati per trasgressore dal database
        private List<PuntiDecurtatiPerTrasgressore> GetPuntiDecurtatiPerTrasgressore()
        {
            List<PuntiDecurtatiPerTrasgressore> puntiDecurtatiPerTrasgressore = new List<PuntiDecurtatiPerTrasgressore>();

            // Recupera l'elenco dei trasgressori
            List<Anagrafica> trasgressori = GetTrasgressori();

            // Itera sui trasgressori
            foreach (Anagrafica trasgressore in trasgressori)
            {
                // Per ogni trasgressore, recupera il totale dei punti decurtati associati
                int totalePuntiDecurtati = GetTotalePuntiDecurtatiPerTrasgressore(trasgressore.IdAnagrafica);

                // aggiunge i dati alla vista
                PuntiDecurtatiPerTrasgressore puntiDecurtati = new PuntiDecurtatiPerTrasgressore
                {
                    NomeTrasgressore = $"{trasgressore.Nome} {trasgressore.Cognome}",
                    TotalePuntiDecurtati = totalePuntiDecurtati
                };

                puntiDecurtatiPerTrasgressore.Add(puntiDecurtati);
            }
            // Restituisce la lista degli oggetti PuntiDecurtatiPerTrasgressore
            return puntiDecurtatiPerTrasgressore;
        }

        // Metodo per ottenere il totale dei punti decurtati per un trasgressore dal database
        private int GetTotalePuntiDecurtatiPerTrasgressore(int idAnagrafica)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT SUM(DecurtamentoPunti) AS TotalePuntiDecurtati
            FROM VERBALE
            WHERE idanagrafica = @IdAnagrafica";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdAnagrafica", idAnagrafica);

                    // Esegue la query e restituisce il risultato
                    var result = cmd.ExecuteScalar();

                    // Verifica se il risultato non è DBNull
                    if (result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        // Restituisci 0 se il risultato è DBNull
                        return 0;
                    }
                }
            }
        }

        // Azione per visualizzare le violazioni con punti decurtati superiori a 10
        public ActionResult ViolazioniSuperioriA10Punti()
        {
            List<ViolazioneSuperioreA10Punti> violazioniSuperioriA10Punti = GetViolazioniSuperioriA10Punti();
            return View(violazioniSuperioriA10Punti);
        }

        // Metodo  per ottenere l'elenco delle violazioni con punti decurtati superiori a 10 dal database
        private List<ViolazioneSuperioreA10Punti> GetViolazioniSuperioriA10Punti()
        {
            List<ViolazioneSuperioreA10Punti> violazioniSuperioriA10Punti = new List<ViolazioneSuperioreA10Punti>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT v.Importo, a.Cognome, a.Nome, v.DataViolazione, v.DecurtamentoPunti
            FROM VERBALE v
            JOIN ANAGRAFICA a ON v.idanagrafica = a.idanagrafica
            WHERE v.DecurtamentoPunti > 10";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ViolazioneSuperioreA10Punti violazione = new ViolazioneSuperioreA10Punti
                        {
                            Importo = reader.GetDecimal(reader.GetOrdinal("Importo")),
                            Cognome = reader["Cognome"].ToString(),
                            Nome = reader["Nome"].ToString(),
                            DataViolazione = reader.GetDateTime(reader.GetOrdinal("DataViolazione")),
                            DecurtamentoPunti = Convert.ToInt32(reader["DecurtamentoPunti"])
                        };

                        violazioniSuperioriA10Punti.Add(violazione);
                    }
                }
            }

            return violazioniSuperioriA10Punti;
        }

        public ActionResult VisualizzaViolazioniImportoSuperiore()
        {
            List<Verbale> violazioniImportoSuperiore = GetViolazioniImportoSuperiore();
            return View(violazioniImportoSuperiore);
        }

        private List<Verbale> GetViolazioniImportoSuperiore()
        {
            List<Verbale> violazioni = new List<Verbale>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT *
            FROM VERBALE
            WHERE Importo > 400";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Verbale violazione = new Verbale
                        {
                            IdVerbale = Convert.ToInt32(reader["IdVerbale"]),
                            DataViolazione = reader.GetDateTime(reader.GetOrdinal("DataViolazione")),
                            IndirizzoViolazione = reader["IndirizzoViolazione"].ToString(),
                            NominativoAgente = reader["NominativoAgente"].ToString(),
                            DataTrascrizioneVerbale = reader.GetDateTime(reader.GetOrdinal("DataTrascrizioneVerbale")),
                            Importo = reader.GetDecimal(reader.GetOrdinal("Importo")),
                            DecurtamentoPunti = Convert.ToInt32(reader["DecurtamentoPunti"]),
                            IdAnagrafica = Convert.ToInt32(reader["IdAnagrafica"]),
                            IdViolazione = Convert.ToInt32(reader["IdViolazione"])
                        };

                        violazioni.Add(violazione);
                    }
                }
            }

            return violazioni;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NuovoTrasgressore()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NuovoTrasgressore(Anagrafica nuovoTrasgressore)
        {
            if (ModelState.IsValid)
            {
                // Chiamata al metodo per salvare il nuovo trasgressore nel database
                SalvaNuovoTrasgressore(nuovoTrasgressore);


                return RedirectToAction("Anagrafica");
            }

            // Se il modello non è valido, rimani sulla stessa pagina con i messaggi di errore
            return View(nuovoTrasgressore);
        }

        private void SalvaNuovoTrasgressore(Anagrafica nuovoTrasgressore)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            INSERT INTO ANAGRAFICA (Cognome, Nome, Indirizzo, Città, CAP, CF)
            VALUES (@Cognome, @Nome, @Indirizzo, @Città, @CAP, @CF)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Cognome", nuovoTrasgressore.Cognome);
                    cmd.Parameters.AddWithValue("@Nome", nuovoTrasgressore.Nome);
                    cmd.Parameters.AddWithValue("@Indirizzo", nuovoTrasgressore.Indirizzo);
                    cmd.Parameters.AddWithValue("@Città", nuovoTrasgressore.Citta);
                    cmd.Parameters.AddWithValue("@CAP", nuovoTrasgressore.CAP);
                    cmd.Parameters.AddWithValue("@CF", nuovoTrasgressore.CF);

                    cmd.ExecuteNonQuery();
                }
            }
        }



    }
}