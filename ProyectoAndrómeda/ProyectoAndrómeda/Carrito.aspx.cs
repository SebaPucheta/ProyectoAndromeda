﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//Del form
using BaseDeDatos;
using System.Data;
using Entidades;
using Negocio;

//Para el mail
using System.Text;
using System.Net.Mail;

using mercadopago;
using System.Collections;

namespace ProyectoAndrómeda
{
    public partial class Carrito : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
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
                //Entrar a "Carrito" pero no desde el catálogo
                cargarGrilla();

                //Cargar el total del carrito
                calcularTotal();

            }

        }

        protected void cargarApunte(int id)
        {
            ApunteEntidad apunte = new ApunteEntidad();
            apunte = ApunteDao.ConsultarApunte(id);
            List<ProductoCarrito> lista = (List<ProductoCarrito>)Session["carrito"];

            //Verifico cada producto del carrito para no agregar el mismo dos veces
            foreach (ProductoCarrito dato in lista)
            {
                //Como aca es solo cargar apunte, verifico de apuntes solo
                if (dato.tipoItem == "Apunte")
                {
                    //Creo el objeto Apunte de la lista para verificar y lo comparo con el que cree afuera
                    ApunteEntidad apunteVerificacion = new ApunteEntidad();
                    apunteVerificacion = (ApunteEntidad)dato.item;

                    //Si son iguales, salgo del metodo
                    if (apunte.idApunte == apunteVerificacion.idApunte)
                    {
                        return;
                    }
                }
            }

            //Paso la verificación, no hay nada igual
            ProductoCarrito nuevoProducto = new ProductoCarrito();
            nuevoProducto.idProductoCarrito = lista.Count + 1;
            nuevoProducto.item = apunte;
            nuevoProducto.tipoItem = "Apunte";
            nuevoProducto.cantidad = 1;
            nuevoProducto.subtotal = nuevoProducto.cantidad * apunte.precioApunte;
            lista.Add(nuevoProducto);
            Session["carrito"] = lista;
            cargarGrilla();

        }

        protected void cargarLibro(int id)
        {
            LibroEntidad libro = new LibroEntidad();
            libro = LibroDao.ConsultarLibro(id);
            List<ProductoCarrito> lista = (List<ProductoCarrito>)Session["carrito"];

            //Verifico cada producto del carrito para no agregar el mismo dos veces
            foreach (ProductoCarrito dato in lista)
            {
                //Como aca es solo cargar libros, verifico libros solo
                if (dato.tipoItem == "Libro")
                {
                    //Creo el objeto Apunte de la lista para verificar y lo comparo con el que cree afuera
                    LibroEntidad libroVerificacion = new LibroEntidad();
                    libroVerificacion = (LibroEntidad)dato.item;

                    //Si son iguales, salgo del metodo
                    if (libro.idLibro == libroVerificacion.idLibro)
                    {
                        return;
                    }
                }
            }

            //Paso la verificación, no hay nada igual
            ProductoCarrito nuevoProducto = new ProductoCarrito();
            nuevoProducto.idProductoCarrito = lista.Count + 1;
            nuevoProducto.item = libro;
            nuevoProducto.tipoItem = "Libro";
            nuevoProducto.cantidad = 1;
            nuevoProducto.subtotal = nuevoProducto.cantidad * libro.precioLibro;
            lista.Add(nuevoProducto);
            Session["carrito"] = lista;
            cargarGrilla();
        }

        //Cargar toda la grilla, TODA
        protected void cargarGrilla()
        {
            List<ProductoCarrito> lista = (List<ProductoCarrito>)Session["carrito"];
            DataTable tabla = new DataTable();
            DataRow fila;

            //Creo las columnas de la tabla
            tabla.Columns.Add("img", typeof(string));
            tabla.Columns.Add("idProductoCarrito", typeof(int));
            tabla.Columns.Add("idProducto", typeof(int));
            tabla.Columns.Add("nombreProducto", typeof(string));
            tabla.Columns.Add("tipoProducto", typeof(string));
            tabla.Columns.Add("precioUnitario", typeof(float));
            tabla.Columns.Add("cantidad", typeof(int));
            tabla.Columns.Add("subtotal", typeof(float));


            foreach (ProductoCarrito dato in lista)
            {
                if (dato.tipoItem == "Apunte")
                {
                    fila = tabla.NewRow();

                    ApunteEntidad apunte = new ApunteEntidad();
                    apunte = (ApunteEntidad)dato.item;
                    //fila[0] = "~/imagenes/PortadaApunte.png";
                    fila[1] = dato.idProductoCarrito;
                    fila[2] = apunte.idApunte;
                    fila[3] = apunte.nombreApunte;
                    fila[4] = dato.tipoItem;
                    fila[5] = apunte.precioApunte;
                    fila[6] = dato.cantidad;
                    fila[7] = dato.subtotal;

                    tabla.Rows.Add(fila);
                }

                if (dato.tipoItem == "Libro")
                {
                    fila = tabla.NewRow();

                    LibroEntidad libro = new LibroEntidad();
                    libro = (LibroEntidad)dato.item;
                    //fila[0] = "~/imagenes/PortadaApunte.png";
                    fila[1] = dato.idProductoCarrito;
                    fila[2] = libro.idLibro;
                    fila[3] = libro.nombreLibro;
                    fila[4] = dato.tipoItem;
                    fila[5] = libro.precioLibro;
                    fila[6] = dato.cantidad;
                    fila[7] = dato.subtotal;

                    tabla.Rows.Add(fila);
                }
            }

            DataView dataView = new DataView(tabla);

            dgv_carrito.DataKeyNames = new string[] { "idProductoCarrito" };
            dgv_carrito.DataSource = dataView;
            dgv_carrito.DataBind();

            //Actualizar cantidad
            foreach (GridViewRow row in dgv_carrito.Rows)
            {
                //Si es digital no hay cantidad
                if (row.Cells[5].Text == "Apunte" && ApunteDao.ConsultarTipoApunte(int.Parse(row.Cells[2].Text)) == "Digital")
                    ((TextBox)row.FindControl("txt_cantidad")).Enabled = false;
                else
                    ((TextBox)row.FindControl("txt_cantidad")).Text = row.Cells[7].Text;
            }

            //Cargar imagenes de producto digital
            foreach (GridViewRow row in dgv_carrito.Rows)
            {
                if (row.Cells[5].Text == "Apunte")
                {
                    if (ApunteDao.ConsultarTipoApunte(int.Parse(row.Cells[2].Text)) == "Digital")
                        row.FindControl("img_digital").Visible = true;
                }
            }

            cargarPortadas();

        }


        protected void calcularTotal()
        {
            float acumulador = 0;
            List<ProductoCarrito> lista = (List<ProductoCarrito>)Session["carrito"];
            foreach (ProductoCarrito dato in lista)
            {
                acumulador = acumulador + dato.subtotal;
            }
            lbl_total.Text = acumulador.ToString("0.00");
        }

        //Creo facutura para la trasacción - VER NUMERO DE ESTADO
        protected FacturaEntidad crearFacturaEntidad(string idMP)
        {
            FacturaEntidad factura = new FacturaEntidad();
            factura.total = float.Parse(lbl_total.Text);
            factura.idUsuario = UsuarioDao.ConsultarIdUsuario(HttpContext.Current.User.Identity.Name);
            //Ponele que el 2 es pendiente
            factura.idEstadoPago = 2;
            //Id facuta MercadoPago
            factura.idFacturaMP = idMP;
            //Carrito con todos los objetos
            factura.listaProductoCarrito = (List<ProductoCarrito>)Session["carrito"];

            return factura;
        }

        //Validar stock de un apunte impreso
        protected bool faltaStock()
        {
            bool faltaStock = false;
            List<ProductoCarrito> lista = (List<ProductoCarrito>)Session["carrito"];
            foreach (ProductoCarrito dato in lista)
            {
                if (dato.tipoItem == "Apunte")
                {
                    //Restar la cantidad de stock al apunte SI ES IMPRESO
                    if (ApunteDao.ConsultarTipoApunte(((ApunteEntidad)(dato.item)).idApunte) == "Impreso")
                    {
                        int cantidad = dato.cantidad;
                        int stock = ((ApunteEntidad)dato.item).stock;
                        if (stock < cantidad)
                            faltaStock = true;
                    }
                }

                if (dato.tipoItem == "Libro")
                {
                    int cantidad = dato.cantidad;
                    int stock = ((LibroEntidad)dato.item).stock;
                    if (stock < cantidad)
                        faltaStock = true;
                }
            }
            return faltaStock;
        }

        //[0] asp:image
        //[1] idProdCarrito
        //[2] idProd
        //[3] img_digital
        //[4] nombre
        //[5] tipoProducto
        //[6] precio
        //[7] cantidad
        //[8] textbox cantidad
        //[9] subtotal

        private string CrearCorreo()
        {
            float total = 0;
            string correo = "Se han realizado las siguientes ventas:";
            foreach (GridViewRow fila in dgv_carrito.Rows)
            {
                //Replace("$", "")
                string prueba = fila.Cells[9].Text.Replace("$", "").Replace("€", "").Replace(",", ".").Trim();
                total += float.Parse(prueba);
                if (fila.Cells[5].Text.Equals("Apunte"))
                {
                    if (int.Parse(fila.Cells[7].Text) == 1)
                    {
                        correo = correo + "\n Se vendio 1 unidad del apunte " + fila.Cells[4].Text + " en formato " + ApunteDao.ConsultarTipoApunte(int.Parse(fila.Cells[2].Text)).ToLower() + " por un total de " + fila.Cells[9].Text;
                    }
                    else
                    {
                        correo = correo + "\n Se vendio " + fila.Cells[7].Text + " unidades de los apuntes " + fila.Cells[4].Text + " en formato " + ApunteDao.ConsultarTipoApunte(int.Parse(fila.Cells[2].Text)).ToLower() + " por un total de " + fila.Cells[9].Text;
                    }
                }
                else
                {
                    if (int.Parse(fila.Cells[7].Text) == 1)
                    {
                        correo = correo + "\n Se vendio 1 unidad del libro " + fila.Cells[4].Text + " por un total de " + fila.Cells[9].Text;
                    }
                    else
                    {
                        correo = correo + "\n Se vendio " + fila.Cells[7].Text + " unidades de los libros " + fila.Cells[4].Text + " por un total de " + fila.Cells[9].Text;
                    }
                }

            }
            correo = correo + "\n El total de la venta es de $" + total;
            return correo;
        }


        private Boolean SendMail(string correo, string body)
        {
            try
            {
                //Configuración del Mensaje
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                //Especificamos el correo desde el que se enviará el Email y el nombre de la persona que lo envía
                mail.From = new MailAddress("seba.pucheta.17@gmail.com", "EDUCOM", Encoding.UTF8);
                //Aquí ponemos el asunto del correo
                mail.Subject = "Venta realizada";
                //Aquí ponemos el mensaje que incluirá el correo
                mail.Body = body;
                //Especificamos a quien enviaremos el Email, no es necesario que sea Gmail, puede ser cualquier otro proveedor
                mail.To.Add(correo);
                //Si queremos enviar archivos adjuntos tenemos que especificar la ruta en donde se encuentran
                //mail.Attachments.Add(new Attachment(@"C:\Documentos\carta.docx"));

                //Configuracion del SMTP
                SmtpServer.Port = 587; //Puerto que utiliza Gmail para sus servicios
                //Especificamos las credenciales con las que enviaremos el mail
                SmtpServer.Credentials = new System.Net.NetworkCredential("seba.pucheta.17@gmail.com", "43614315616628");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        protected string GenerarPago()
        {
            string idMP = "";
            try
            {
                //Pongo CLIENT_ID y CLIENT_SECRET -- Aplicacion: 233620557 -- mp-app-233620557
                MP mp = new MP("4180047074500934", "oTJvHcjBcmbO73LsYTNseZhUmzaNYkx1");
                mp.sandboxMode(true);

                //Reemplazo comas por puntos
                string total = (lbl_total.Text).Replace(",", ".").Trim();

                //Creo la preferencia de la boleta como si fuera un objeto json, en un string
                string preferenceData = "{\"items\":" +
                                            "[{" +
                                                "\"title\":\"" + generarStringProductos() + "\"," +
                                                "\"quantity\":1," +
                                                "\"currency_id\":\"ARS\"," +
                                                "\"unit_price\":" + total +
                                            "}]" +
                                         "}";

                //Creo la preferencia en una hashtable
                Hashtable preference = mp.createPreference(preferenceData);

                //Para obtener el ID del pago
                idMP = (((Hashtable)preference["response"])["id"]).ToString();

            }
            catch (Exception ex)
            {
                Response.Write("<script>window.alert('Hubo un problema con el acceso a internet')</script>");
            }
            return idMP;

        }

        protected string generarStringProductos()
        {
            string cadena = "";
            List<ProductoCarrito> lista = (List<ProductoCarrito>)Session["carrito"];
            foreach (ProductoCarrito dato in lista)
            {
                cadena = cadena + "|";
                if (dato.tipoItem == "Apunte")
                {
                    cadena = cadena + ((ApunteEntidad)dato.item).nombreApunte + " x" + dato.cantidad + " ";
                }

                if (dato.tipoItem == "Libro")
                {
                    cadena = cadena + ((LibroEntidad)dato.item).nombreLibro + " x" + dato.cantidad + " ";
                }
            }
            return cadena;
        }

        //Cargar las portadas en el carrito de compras a la izquierda
        protected void cargarPortadas()
        {
            foreach (GridViewRow fila in dgv_carrito.Rows)
            {
                if (fila.Cells[5].Text.Equals("Apunte"))
                {
                    ApunteEntidad apunte = ApunteDao.ConsultarApunte(int.Parse(fila.Cells[2].Text));
                    if (apunte.imagenApunte == "")
                        ((Image)fila.FindControl("img")).ImageUrl = "~/imagenes/PortadaApunte.png";
                    else
                        ((Image)fila.FindControl("img")).ImageUrl = apunte.imagenApunte;
                }
                else
                {
                    LibroEntidad libro = LibroDao.ConsultarLibro(int.Parse(fila.Cells[2].Text));
                    if (libro.imagenLibro == "")
                        ((Image)fila.FindControl("img")).ImageUrl = "~/imagenes/PortadaApunte.png";
                    else
                        ((Image)fila.FindControl("img")).ImageUrl = libro.imagenLibro;
                }
            }
        }




        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////EVENTOS////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////

        //[0] asp:image
        //[1] idProdCarrito
        //[2] idProd
        //[3] img_digital
        //[4] nombre
        //[5] tipoProducto
        //[6] precio
        //[7] cantidad
        //[8] textbox cantidad
        //[9] subtotal

        protected void btn_actualizar_Click(object sender, EventArgs e)
        {
            List<ProductoCarrito> lista = (List<ProductoCarrito>)Session["carrito"];
            foreach (GridViewRow row in dgv_carrito.Rows)
            {
                //Veo si tiene el txtbox le pusieron una cantidad sino salteo al otro item
                if (((TextBox)row.Cells[7].FindControl("txt_cantidad")).Text == "")
                    continue;

                //Si le ingresaron una nueva cantidad, procedo
                int nuevaCantidad = int.Parse(((TextBox)row.Cells[8].FindControl("txt_cantidad")).Text);

                //Verifico si la cantidad es mayor a 0
                if (nuevaCantidad < 1)
                {
                    Response.Write("<script>window.alert('Se ha ingresado una cantidad incorrecta')</script>");
                    continue;
                }

                int idActual = int.Parse(dgv_carrito.DataKeys[row.RowIndex].Value.ToString());

                foreach (ProductoCarrito dato in lista)
                {
                    if (dato.idProductoCarrito == idActual)
                    {
                        if (dato.tipoItem == "Apunte")
                        {
                            ApunteEntidad apunte = new ApunteEntidad();
                            apunte = (ApunteEntidad)dato.item;
                            dato.cantidad = nuevaCantidad;
                            dato.subtotal = dato.cantidad * apunte.precioApunte;
                        }

                        if (dato.tipoItem == "Libro")
                        {
                            LibroEntidad libro = new LibroEntidad();
                            libro = (LibroEntidad)dato.item;
                            dato.cantidad = nuevaCantidad;
                            dato.subtotal = dato.cantidad * libro.precioLibro;
                        }
                    }
                }
            }
            cargarGrilla();
            calcularTotal();
        }


        protected void dgv_carrito_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            ((List<ProductoCarrito>)Session["carrito"]).RemoveAt(e.RowIndex);
            cargarGrilla();
            calcularTotal();
        }



        protected void dgv_carrito_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idProducto = (int)dgv_carrito.SelectedDataKey.Value;
            int indice = (int)dgv_carrito.SelectedIndex;
            string tipo = dgv_carrito.Rows[indice].Cells[5].Text;

            List<ProductoCarrito> lista = ((List<ProductoCarrito>)Session["carrito"]);
            foreach (ProductoCarrito dato in lista)
            {
                if (idProducto == dato.idProductoCarrito)
                {
                    if (tipo == "Apunte")
                    {
                        ApunteEntidad apunte = new ApunteEntidad();
                        apunte = (ApunteEntidad)dato.item;
                        int id = apunte.idApunte;
                        string dir = "DetalleItem.aspx?idLibro=0&idApunte=" + id.ToString();
                        Response.Redirect(dir);
                        break;
                    }

                    if (tipo == "Libro")
                    {
                        LibroEntidad libro = new LibroEntidad();
                        libro = (LibroEntidad)dato.item;
                        int id = libro.idLibro;
                        string dir = "DetalleItem.aspx?idApunte=0&idLibro=" + id.ToString();
                        Response.Redirect(dir);
                        break;
                    }
                }
            }

        }


        //Confirmar la transaccion
        protected void btn_confirmar_Click(object sender, EventArgs e)
        {
            //Hago la transacción
            if (faltaStock())
            {
                Response.Write("<script>window.alert('No hay stock disponible de alguno de los productos')</script>");
                return;
            }

            //Si no esta logueado y toco el boton de confirmar
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                Response.Write("<script>window.alert('Primero debe iniciar sesión')</script>");
                return;
            }

            //Enviar mail
            SendMail("marvinien1.0@gmail.com", CrearCorreo());

            //Generar id factura mercado pago
            string idMP = GenerarPago();

            if (idMP != "")
            {
                int idFactura = FacturaDao.RegistrarFactura(crearFacturaEntidad(idMP));
                Response.Redirect("Pago.aspx?fact=" + idFactura.ToString());
            }else
            {
                Response.Write("<script>window.alert('No se ha podido generar el pedido')</script>");
            }
        }


        protected void btn_cancelar_Click(object sender, EventArgs e)
        {
            Session["carrito"] = new List<ProductoCarrito>();
            Response.Redirect("Home.aspx");
        }
    }


}
