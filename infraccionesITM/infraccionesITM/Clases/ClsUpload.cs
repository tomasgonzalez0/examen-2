using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace infraccionesITM.Clases
{
    public class ClsUpload
    {

        public HttpRequestMessage request { get; set; }
        public string Datos { get; set; }
        public string Proceso { get; set; }
        public async Task<HttpResponseMessage> GrabarArchivo(bool Actualizar)
        {
            string RptaError = "";
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.UnsupportedMediaType);
            }
            string root = HttpContext.Current.Server.MapPath("~/Archivos");
            var provider = new MultipartFormDataStreamProvider(root);
            try
            {
                await request.Content.ReadAsMultipartAsync(provider);
                List<string> Archivos = new List<string>();
                foreach (MultipartFileData file in provider.FileData)
                {
                    string fileName = file.Headers.ContentDisposition.FileName;
                    if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                    {
                        fileName = fileName.Trim('"');
                    }
                    if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                    {
                        fileName = Path.GetFileName(fileName);
                    }
                    if (File.Exists(Path.Combine(root, fileName)))
                    {
                        if (Actualizar)
                        {
                            //Se borra el original
                            File.Delete(Path.Combine(root, fileName));
                            //Se crea el nuevo archivo con el mismo nombre
                            File.Move(file.LocalFileName, Path.Combine(root, fileName));
                        }
                        else
                        {
                            File.Delete(file.LocalFileName);
                            //Se da una respuesta de error
                            RptaError += "El archivo: " + fileName + " ya existe";
                        }
                    }
                    else
                    {
                        Archivos.Add(fileName);
                        //Se renombra el archivo
                        File.Move(file.LocalFileName, Path.Combine(root, fileName));
                    }
                }
                if (Archivos.Count > 0)
                {
                    //Envía a grabar la información de las imágenes
                    string Respuesta = ProcesarArchivos(Archivos);
                    //Se da una respuesta de éxito
                    return request.CreateResponse(HttpStatusCode.OK, "Archivo subido con éxito");
                }
                else
                {
                    if (Actualizar)
                    {
                        return request.CreateResponse(HttpStatusCode.OK, "Archivo actualizado con éxito");
                    }
                    else
                    {
                        return request.CreateErrorResponse(HttpStatusCode.Conflict, "El(los) archivo(s) ya existe(n)");
                    }
                }
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error al cargar el archivo: " + ex.Message);
            }
        }
        public HttpResponseMessage ConsultarArchivo(string NombreArchivo)
        {
            try
            {
                string Ruta = HttpContext.Current.Server.MapPath("~/Archivos");
                string Archivo = Path.Combine(Ruta, NombreArchivo);
                if (File.Exists(Archivo))
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    var stream = new FileStream(Archivo, FileMode.Open, FileAccess.Read);
                    response.Content = new StreamContent(stream);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = NombreArchivo;
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    return response;
                }
                else
                {
                    return request.CreateErrorResponse(HttpStatusCode.NotFound, "Archivo no encontrado");
                }
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error al consultar el archivo: " + ex.Message);
            }
        }
        public HttpResponseMessage EliminarArchivo(string NombreArchivo)
        {
            try
            {
                string Ruta = HttpContext.Current.Server.MapPath("~/Archivos");
                string Archivo = Path.Combine(Ruta, NombreArchivo);

                if (File.Exists(Archivo))
                {
                    File.Delete(Archivo);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("Archivo eliminado correctamente")
                    };
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("Archivo no encontrado")
                    };
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Error al eliminar el archivo: " + ex.Message)
                };
            }
        }
        private string ProcesarArchivos(List<string> Archivos)
        {
            switch (Proceso.ToUpper())
            {
                case "PRODUCTO":
                    clsImagenesInfraccion ImagenesProducto = new clsImagenesInfraccion();
                    ImagenesProducto.idProducto = Datos;//Debe venir la información que se procesa en la base de datos, para nuestro caso, el código del producto
                    ImagenesProducto.Archivos = Archivos;
                    return ImagenesProducto.GrabarImagenes();
                default:
                    return "Proceso no válido";
            }
        }
    }
}