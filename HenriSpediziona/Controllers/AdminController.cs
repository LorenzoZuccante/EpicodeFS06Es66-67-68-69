﻿using HenriSpediziona.Models;
using System.Web.Mvc;

public class AdminController : Controller
{
    // GET: Admin/Login
    public ActionResult Login()
    {
        return View();
    }

    // POST: Admin/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(AdminLoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        
        if (model.Email == "pippo@franco" && model.Password == "LagunaRio2322")
        {
           
            Session["AdminLogged"] = true;
            return RedirectToAction("Index", "Home"); 
        }
        else
        {
            ModelState.AddModelError("", "Le credenziali non sono valide.");
            return View(model);
        }
    }
    public ActionResult Logout()
    {
        Session["AdminLogged"] = null;
        return RedirectToAction("Index", "Home"); 
    }

}
