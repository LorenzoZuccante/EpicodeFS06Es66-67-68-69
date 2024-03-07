using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using HenriSpediziona.Models;

public class StatiSpedizioniController : Controller
{
    public ActionResult Create()
    {
        if (Session["AdminLogged"] == null || !(bool)Session["AdminLogged"])
        {
            return RedirectToAction("Login", "Admin");
        }

        ViewBag.IdSpedizione = new SelectList(GetSpedizioni(), "Value", "Text");
        ViewBag.StatoConsegna = new SelectList(new List<string> { "In transito", "In consegna", "Consegnato", "Non consegnato" });
        return View(new StatoSpedizione { DataOraAggiornamento = DateTime.Now });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(StatoSpedizione statoSpedizione)
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
                string query = "INSERT INTO StatoSpedizioni (IdSpedizione, StatoConsegna, PosizionePacco, Descrizione, DataOraAggiornamento) VALUES (@IdSpedizione, @StatoConsegna, @PosizionePacco, @Descrizione, @DataOraAggiornamento)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdSpedizione", statoSpedizione.IdSpedizione);
                    cmd.Parameters.AddWithValue("@StatoConsegna", statoSpedizione.StatoConsegna);
                    cmd.Parameters.AddWithValue("@PosizionePacco", statoSpedizione.PosizionePacco ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descrizione", statoSpedizione.Descrizione ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DataOraAggiornamento", statoSpedizione.DataOraAggiornamento);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        ViewBag.IdSpedizione = new SelectList(GetSpedizioni(), "Value", "Text", statoSpedizione.IdSpedizione);
        ViewBag.StatoConsegna = new SelectList(new List<string> { "In transito", "In consegna", "Consegnato", "Non consegnato" }, statoSpedizione.StatoConsegna);
        return View(statoSpedizione);
    }

    private IEnumerable<SelectListItem> GetSpedizioni()
    {
        List<SelectListItem> spedizioni = new List<SelectListItem>();
        string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string query = "SELECT IdSpedizione, DataSpedizione, NomeDestinatario, CognomeDestinatario, CittaDestinataria, IndirizzoDestinatario, DataDiConsegna FROM Spedizioni";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string displayValue = $"ID: {reader["IdSpedizione"]} - {reader["DataSpedizione"]:yyyy-MM-dd} Destinatario: {reader["NomeDestinatario"]} {reader["CognomeDestinatario"]} ({reader["CittaDestinataria"]} {reader["IndirizzoDestinatario"]} {reader["DataDiConsegna"]:yyyy-MM-dd})";
                    spedizioni.Add(new SelectListItem
                    {
                        Value = reader["IdSpedizione"].ToString(),
                        Text = displayValue
                    });
                }
            }
        }
        return spedizioni;
    }

    public ActionResult Index()
    {
        if (Session["AdminLogged"] == null || !(bool)Session["AdminLogged"])
        {
            return RedirectToAction("Login", "Admin");
        }

        List<StatoSpedizione> statiSpedizioni = GetAllStatiSpedizioni();
        return View(statiSpedizioni);
    }

    public ActionResult FiltraInConsegna()
    {
        if (Session["AdminLogged"] == null || !(bool)Session["AdminLogged"])
        {
            return RedirectToAction("Login", "Admin");
        }

        List<StatoSpedizione> statiSpedizioni = GetAllStatiSpedizioni();
        ViewBag.CountInConsegna = statiSpedizioni.Count(s => s.StatoConsegna == "In Consegna");
        return View("Index", statiSpedizioni.Where(s => s.StatoConsegna == "In Consegna").ToList());
    }

    private List<StatoSpedizione> GetAllStatiSpedizioni()
    {
        List<StatoSpedizione> statiSpedizioni = new List<StatoSpedizione>();
        string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string query = "SELECT * FROM StatoSpedizioni";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    statiSpedizioni.Add(new StatoSpedizione
                    {
                        IdStato = (int)reader["IdStato"],
                        IdSpedizione = (int)reader["IdSpedizione"],
                        StatoConsegna = reader["StatoConsegna"].ToString(),
                        PosizionePacco = reader["PosizionePacco"].ToString(),
                        Descrizione = reader["Descrizione"] != DBNull.Value ? reader["Descrizione"].ToString() : "",
                        DataOraAggiornamento = (DateTime)reader["DataOraAggiornamento"]
                    });
                }
            }
        }
        return statiSpedizioni;
    }
}
