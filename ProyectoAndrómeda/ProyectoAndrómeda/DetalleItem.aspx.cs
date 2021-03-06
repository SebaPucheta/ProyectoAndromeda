﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseDeDatos;
using Entidades;

namespace ProyectoAndrómeda
{
    public partial class DetalleItem : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int id;
            if (Request.QueryString.Count > 0)
            {
                if (int.Parse(Request.QueryString["idApunte"]) != 0)
                {
                    id = int.Parse(Request.QueryString["idApunte"]);
                    cargarApunte(id);
                }
                if (int.Parse(Request.QueryString["idLibro"]) != 0)
                {
                    id = int.Parse(Request.QueryString["idLibro"]);
                    cargarLibro(id);
                }
            }
            //Entrar a "DetalleItem" pero no desde el catálogo
        }

        protected void cargarApunte(int id)
        {
            ApunteEntidad apunte = new ApunteEntidad();
            apunte = ApunteDao.ConsultarApunte(id);
            lbl_titulo.Text = apunte.nombreApunte;
            lbl_precio.Text = apunte.precioApunte.ToString();
            lbl_stock.Text = apunte.stock.ToString();
            lbl_codigo.Text = apunte.codigoBarraApunte.ToString();
            lbl_descripcion.Text = apunte.descripcionApunte;
            img_portada.ImageUrl = "imagenes/apunte/" + apunte.idApunte.ToString() + ".jpg";

                if (ApunteDao.ConsultarTipoApunte(id) == "Digital")
                    img_digital.Visible = true;
        }

        protected void cargarLibro(int id)
        {
            LibroEntidad libro = new LibroEntidad();
            libro = LibroDao.ConsultarLibro(id);
            lbl_titulo.Text = libro.nombreLibro;
            lbl_precio.Text = libro.precioLibro.ToString();
            lbl_stock.Text = libro.stock.ToString();
            lbl_codigo.Text = libro.codigoBarraLibro.ToString();
            lbl_descripcion.Text = libro.descripcionLibro;

            img_portada.ImageUrl = "imagenes/libro/" + libro.idLibro.ToString() + ".jpg";
        }



        protected void btn_carrito_Click(object sender, EventArgs e)
        {
            int id;
            string dirCarrito;
            if (int.Parse(Request.QueryString["idApunte"]) != 0)
            {
                id = int.Parse(Request.QueryString["idApunte"]);
                dirCarrito = "Carrito.aspx?idLibro=0&idApunte=" + id.ToString();
                Response.Redirect(dirCarrito);
            }
            if (int.Parse(Request.QueryString["idLibro"]) != 0)
            {
                id = int.Parse(Request.QueryString["idLibro"]);
                dirCarrito = "Carrito.aspx?idLibro=0&idApunte=" + id.ToString();
                Response.Redirect(dirCarrito);
            }

        
        }
    }
}