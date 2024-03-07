using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using HenriSpediziona.Models;

public class SpedizioniController : Controller
{
    // Metodo GET per la creazione di una nuova spedizione
    public ActionResult Create()
    {
        if (Session["AdminLogged"] == null || !(bool)Session["AdminLogged"])
        {
            return RedirectToAction("Login", "Admin");
        }

        ViewBag.IdUtente = new SelectList(GetUtenti(), "Value", "Text");
        return View(new Spedizione { DataSpedizione = DateTime.Now });
    }

    // Metodo POST per la creazione di una nuova spedizione
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Spedizione spedizione)
    {
        if (Session["AdminLogged"] == null || !(bool)Session["AdminLogged"])
        {
            return RedirectToAction("Login", "Admin");
        }

        if (ModelState.IsValid)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"INSERT INTO Spedizioni (IdUtente, DataSpedizione, Peso, CittaDestinataria, IndirizzoDestinatario, NomeDestinatario, CognomeDestinatario, CostoSpedizione, DataDiConsegna)
                                 VALUES (@IdUtente, @DataSpedizione, @Peso, @CittaDestinataria, @IndirizzoDestinatario, @NomeDestinatario, @CognomeDestinatario, @CostoSpedizione, @DataDiConsegna)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    // Imposta i parametri qui
                    cmd.Parameters.AddWithValue("@IdUtente", spedizione.IdUtente);
                    cmd.Parameters.AddWithValue("@DataSpedizione", spedizione.DataSpedizione);
                    cmd.Parameters.AddWithValue("@Peso", spedizione.Peso);
                    cmd.Parameters.AddWithValue("@CittaDestinataria", spedizione.CittaDestinataria);
                    cmd.Parameters.AddWithValue("@IndirizzoDestinatario", spedizione.IndirizzoDestinatario);
                    cmd.Parameters.AddWithValue("@NomeDestinatario", spedizione.NomeDestinatario ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CognomeDestinatario", spedizione.CognomeDestinatario ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CostoSpedizione", spedizione.CostoSpedizione);
                    cmd.Parameters.AddWithValue("@DataDiConsegna", spedizione.DataDiConsegna ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        ViewBag.IdUtente = new SelectList(GetUtenti(), "Value", "Text", spedizione.IdUtente);
        return View(spedizione);
    }

    // Recupera una lista degli utenti per la selezione nella vista di creazione
    private IEnumerable<SelectListItem> GetUtenti()
    {
        List<SelectListItem> utenti = new List<SelectListItem>();
        string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string query = "SELECT IdUtente, Nome, Cognome FROM Utenti";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    utenti.Add(new SelectListItem
                    {
                        Value = reader["IdUtente"].ToString(),
                        Text = reader["Nome"].ToString() + " " + reader["Cognome"].ToString()
                    });
                }
            }
        }
        return utenti;
    }

    // Metodo GET per visualizzare l'elenco delle spedizioni
    public ActionResult Index()
    {
        if (Session["AdminLogged"] == null || !(bool)Session["AdminLogged"])
        {
            return RedirectToAction("Login", "Admin");
        }

        List<Spedizione> spedizioni = new List<Spedizione>();
        Dictionary<string, int> conteggioPerCitta = new Dictionary<string, int>();

        string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string querySpedizioni = "SELECT * FROM Spedizioni";
            using (SqlCommand cmdSpedizioni = new SqlCommand(querySpedizioni, con))
            {
                SqlDataReader reader = cmdSpedizioni.ExecuteReader();
                while (reader.Read())
                {
                    Spedizione spedizione = new Spedizione
                    {
                        IdSpedizione = (int)reader["IdSpedizione"],
                        IdUtente = (int)reader["IdUtente"],
                        DataSpedizione = (DateTime)reader["DataSpedizione"],
                        Peso = (int)reader["Peso"],
                        CittaDestinataria = reader["CittaDestinataria"].ToString(),
                        IndirizzoDestinatario = reader["IndirizzoDestinatario"].ToString(),
                        NomeDestinatario = reader["NomeDestinatario"].ToString(),
                        CognomeDestinatario = reader["CognomeDestinatario"].ToString(),
                        CostoSpedizione = (decimal)reader["CostoSpedizione"],
                        DataDiConsegna = reader.IsDBNull(reader.GetOrdinal("DataDiConsegna")) ? null : (DateTime?)reader["DataDiConsegna"]
                    };
                    spedizioni.Add(spedizione);

                    string citta = spedizione.CittaDestinataria;
                    if (!string.IsNullOrEmpty(citta))
                    {
                        if (conteggioPerCitta.ContainsKey(citta))
                        {
                            conteggioPerCitta[citta]++;
                        }
                        else
                        {
                            conteggioPerCitta[citta] = 1;
                        }
                    }
                }
            }
        }

        ViewBag.SpedizioniPerCitta = conteggioPerCitta;
        return View(spedizioni);
    }
}
