using infraccionesITM.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace infraccionesITM.Controllers
{
    [RoutePrefix("UploadFiles")]
    public class UploadFilesController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> GrabarArchivo(HttpRequestMessage Request, string Datos, string Proceso)
        {
            ClsUpload UploadFiles = new ClsUpload();
            UploadFiles.request = Request;
            UploadFiles.Datos = Datos;
            UploadFiles.Proceso = Proceso;
            return await UploadFiles.GrabarArchivo(false);
        }
        [HttpGet]
        public HttpResponseMessage Get(string NombreImagen)
        {
            ClsUpload upload = new ClsUpload();
            return upload.ConsultarArchivo(NombreImagen);
        }
        [HttpPut]
        public async Task<HttpResponseMessage> ActualizarArchivo(HttpRequestMessage Request, string Datos, string Proceso)
        {
            ClsUpload UploadFiles = new ClsUpload();
            UploadFiles.request = Request;
            UploadFiles.Datos = Datos;
            UploadFiles.Proceso = Proceso;
            return await UploadFiles.GrabarArchivo(true);
        }
        [HttpDelete]
        public HttpResponseMessage EliminarArchivo(string NombreImagen)
        {
            ClsUpload upload = new ClsUpload();
            return upload.EliminarArchivo(NombreImagen);
        }
    }
}