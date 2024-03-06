using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using HenriSpediziona.Models;

public class SpedizioniController : Controller
{
    // GET: Spedizioni/Create
    public ActionResult Create()
    {
        if (Session["AdminLogged"] == null || !(bool)Session["AdminLogged"])
        {
            return RedirectToAction("Login", "Admin");
        }

        // Prepara la vista con la lista degli utenti
        ViewBag.IdUtente = new SelectList(GetUtenti(), "Value", "Text");
        return View(new Spedizione { DataSpedizione = DateTime.Now });  // Imposta la data di spedizione predefinita
    }

    // POST: Spedizioni/Create
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
                    cmd.Parameters.AddWithValue("@NomeDestinatario", spedizione.NomeDestinatario ?? (object)DBNull.Value); // Gestisci i campi nullable
                    cmd.Parameters.AddWithValue("@CognomeDestinatario", spedizione.CognomeDestinatario ?? (object)DBNull.Value); // Gestisci i campi nullable
                    cmd.Parameters.AddWithValue("@CostoSpedizione", spedizione.CostoSpedizione);
                    cmd.Parameters.AddWithValue("@DataDiConsegna", spedizione.DataDiConsegna ?? (object)DBNull.Value); // Gestisci i campi nullable

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");  
        }

        // Se il modello non è valido, ricarica la pagina con le informazioni attuali
        ViewBag.IdUtente = new SelectList(GetUtenti(), "Value", "Text", spedizione.IdUtente);
        return View(spedizione);
    }

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
                    string fullName = reader["Nome"].ToString() + " " + reader["Cognome"].ToString();

                    utenti.Add(new SelectListItem
                    {
                        Value = reader["IdUtente"].ToString(), // ID dell'utente
                        Text = fullName // Nome e Cognome concatenati
                    });
                }
            }
        }
        return utenti;
    }
    public ActionResult Index()
    {
        if (Session["AdminLogged"] == null || !(bool)Session["AdminLogged"])
        {
            return RedirectToAction("Login", "Admin");
        }

        List<Spedizione> spedizioni = new List<Spedizione>();
        string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Spedizioni";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
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
                        DataDiConsegna = reader["DataDiConsegna"] as DateTime?
                    };
                    spedizioni.Add(spedizione);
                }
            }
        }
        return View(spedizioni);
    }

}
