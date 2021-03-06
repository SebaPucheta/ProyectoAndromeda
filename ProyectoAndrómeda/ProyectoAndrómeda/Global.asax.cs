﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Entidades;
using BaseDeDatos;

namespace ProyectoAndrómeda
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            //Carrito que se va guardando mientras consulto un catálogo
            Session["carrito"] = new List<ProductoCarrito>();

            //Guardo id de la factura para saber si se puede o no descargar un archivo
            Session["idFacturaPanel"] = null;

            //
            Session["objetoApunteEntidad"] = null;
            Session["objetoLibroEntidad"] = null;
            Session["idUsuario"] = "";
            Session["nombreUsuario"] = "";
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                var id = HttpContext.Current.User.Identity;
                HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(id, LoginDao.ObtenerRoles(id.Name));
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            Session["carrito"] = null;
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}