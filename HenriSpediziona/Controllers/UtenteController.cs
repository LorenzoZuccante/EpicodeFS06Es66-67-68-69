using System;
using System.Configuration;
using System.Data.SqlClient;
using HenriSpediziona.Models;
using System.Web.Mvc;
using System.Collections.Generic;

namespace HenriSpediziona.Controllers
{
    public class UtenteController : Controller
    {
        // GET: Utente/New
        public ActionResult New()
        {
            if (Session["AdminLogged"] == null || !(bool)Session["AdminLogged"])
            {
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(Utente utente)
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
                    string sqlQuery = "INSERT INTO Utenti (Email, Password, Azienda, Nome, Cognome, Cod_Fisc, P_iva, IsAdmin) VALUES (@Email, @Password, @Azienda, @Nome, @Cognome, @Cod_Fisc, @P_iva, @IsAdmin)";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", utente.Email);
                        cmd.Parameters.AddWithValue("@Password", utente.Password);
                        cmd.Parameters.AddWithValue("@Azienda", utente.Azienda);
                        cmd.Parameters.AddWithValue("@Nome", (object)utente.Nome ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Cognome", (object)utente.Cognome ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Cod_Fisc", (object)utente.Cod_Fisc ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@P_iva", (object)utente.P_iva ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsAdmin", false); 

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");  
            }

            return View(utente);
        }
        public ActionResult Index()
        {
            if (Session["AdminLogged"] == null || !(bool)Session["AdminLogged"])
            {
                return RedirectToAction("Login", "Admin");
            }

            List<Utente> listaUtenti = new List<Utente>();
            string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Utenti";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Utente utente = new Utente()
                        {
                            IdUtente = Convert.ToInt32(reader["IdUtente"]),
                            Email = reader["Email"].ToString(),
                            Azienda = Convert.ToBoolean(reader["Azienda"]),
                            Nome = reader["Nome"].ToString(),
                            Cognome = reader.IsDBNull(reader.GetOrdinal("Cognome")) ? null : reader["Cognome"].ToString(),
                            Cod_Fisc = reader.IsDBNull(reader.GetOrdinal("Cod_Fisc")) ? null : reader["Cod_Fisc"].ToString(),
                            P_iva = reader.IsDBNull(reader.GetOrdinal("P_iva")) ? (int?)null : Convert.ToInt32(reader["P_iva"])
                        };
                        listaUtenti.Add(utente);
                    }
                }
            }

            return View(listaUtenti);
        }

    }

}