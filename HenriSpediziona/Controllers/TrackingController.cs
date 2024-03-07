using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using HenriSpediziona.Models;

public class TrackingController : Controller
{
    // Mostra il form di inserimento per il tracciamento della spedizione
    public ActionResult Index()
    {
        return View(new TrackingViewModel());
    }

    [HttpPost]
    public ActionResult Index(TrackingViewModel model)
    {
        if (ModelState.IsValid)
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString))
            {
                con.Open();
                string userQuery = model.IsAzienda ?
                    "SELECT IdUtente FROM Utenti WHERE P_iva = @Codice" :
                    "SELECT IdUtente FROM Utenti WHERE Cod_Fisc = @Codice";

                using (var userCmd = new SqlCommand(userQuery, con))
                {
                    userCmd.Parameters.AddWithValue("@Codice", model.CodiceFiscalePartitaIVA);
                    var userId = userCmd.ExecuteScalar() as int?;

                    if (userId.HasValue)
                    {
                        string shipmentQuery = "SELECT * FROM Spedizioni WHERE IdUtente = @IdUtente AND IdSpedizione = @IdSpedizione";
                        using (var shipmentCmd = new SqlCommand(shipmentQuery, con))
                        {
                            shipmentCmd.Parameters.AddWithValue("@IdUtente", userId.Value);
                            shipmentCmd.Parameters.AddWithValue("@IdSpedizione", model.IdSpedizione);

                            using (var reader = shipmentCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Prepara i dati per la visualizzazione dello stato della spedizione
                                    model.IdSpedizione = (int)reader["IdSpedizione"];
                                    model.Utente = new Utente // Supponiamo che ci sia una corrispondenza 1 a 1
                                    {
                                        IdUtente = (int)reader["IdUtente"]
                                        // Completa con altri dati dell'utente se necessario
                                    };
                                    // Ricorda di adattare i campi in base alla struttura del tuo DB
                                }
                            }
                        }

                        // Ora recupera gli stati della spedizione, dato che sappiamo che la spedizione esiste
                        string statusQuery = "SELECT * FROM StatoSpedizioni WHERE IdSpedizione = @IdSpedizione ORDER BY DataOraAggiornamento DESC";
                        using (var statusCmd = new SqlCommand(statusQuery, con))
                        {
                            statusCmd.Parameters.AddWithValue("@IdSpedizione", model.IdSpedizione);
                            using (var reader = statusCmd.ExecuteReader())
                            {
                                var statiSpedizione = new List<StatoSpedizione>();
                                while (reader.Read())
                                {
                                    statiSpedizione.Add(new StatoSpedizione
                                    {
                                        // Assicurati che i nomi dei campi corrispondano con quelli del tuo database
                                        IdStato = (int)reader["IdStato"],
                                        StatoConsegna = reader["StatoConsegna"].ToString(),
                                        PosizionePacco = reader["PosizionePacco"].ToString(),
                                        Descrizione = reader["Descrizione"]?.ToString(),
                                        DataOraAggiornamento = (DateTime)reader["DataOraAggiornamento"]
                                    });
                                }
                                model.StatiSpedizione = statiSpedizione;
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Nessuna corrispondenza trovata per i dati inseriti.");
                    }
                }
            }
        }

        // Indipendentemente dall'esito, mostra di nuovo la pagina con i risultati aggiornati o gli errori di validazione
        return View(model);
    }
}
