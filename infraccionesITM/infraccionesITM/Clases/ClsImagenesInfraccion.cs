using infraccionesITM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace infraccionesITM.Clases
{
    public class clsImagenesInfraccion
    {
        private readonly DBExamenEntities3 _db = new DBExamenEntities3();
        public Infraccion infraccion { get; set; }
        public string idProducto { get; set; }
        public List<string> Archivos { get; set; }
        public string GrabarImagenes()
        {
            try
            {
                if (Archivos.Count > 0)
                {
                    foreach (string Archivo in Archivos)
                    {
                        FotoInfraccion Imagen = new FotoInfraccion();
                        Imagen.idFoto = Convert.ToInt32(idProducto);
                        Imagen.NombreFoto = Archivo;
                        _db.FotoInfraccions.Add(Imagen);
                        _db.SaveChanges();
                    }
                    return "Imagenes guardadas correctamente";
                }
                else
                {
                    return "No se enviaron archivos para guardar";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}