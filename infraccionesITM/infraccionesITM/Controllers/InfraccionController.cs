using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Data.Entity;
using infraccionesITM.Models;

namespace infraccionesITM.Controllers
{
    public class InfraccionController : ApiController
    {
        private readonly DBExamenEntities3 _db = new DBExamenEntities3();

        // Clase para recibir la fotomulta y el vehículo en un solo objeto
        public class FotomultaRequest
        {
            public Infraccion Infraccion { get; set; }
            public Vehiculo Vehiculo { get; set; }
        }

        // POST: api/Infraccion
        [HttpPost]
        public IHttpActionResult RegistrarFotomulta([FromBody] FotomultaRequest request)
        {
            if (request == null || request.Infraccion == null || request.Vehiculo == null)
                return Content(HttpStatusCode.BadRequest, "El objeto enviado es inválido.");

            try
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    // Validar si el vehículo ya existe
                    var vehiculoExistente = _db.Vehiculoes.FirstOrDefault(v => v.Placa == request.Vehiculo.Placa);
                    if (vehiculoExistente == null)
                    {
                        _db.Vehiculoes.Add(request.Vehiculo);
                        _db.SaveChanges();
                    }

                    // Guardar la infracción
                    request.Infraccion.PlacaVehiculo = request.Vehiculo.Placa;
                    _db.Infraccions.Add(request.Infraccion);
                    _db.SaveChanges();

                    transaction.Commit();
                    return Content(HttpStatusCode.Created, "Fotomulta registrada correctamente.");
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, "Error: " + ex.Message);
            }
        }

        // GET: api/Infraccion/{placa}
        [HttpGet]
        [Route("api/Infraccion/{placa}")]
        public IHttpActionResult ObtenerFotomultasPorPlaca(string placa)
        {
            try
            {
                var resultado = _db.Infraccions
                    .Where(i => i.PlacaVehiculo == placa)
                    .Select(i => new
                    {
                        i.PlacaVehiculo,
                        Vehiculo = _db.Vehiculoes
                            .Where(v => v.Placa == i.PlacaVehiculo)
                            .Select(v => new { v.TipoVehiculo, v.Marca, v.Color })
                            .FirstOrDefault(),
                        i.FechaInfraccion,
                        i.TipoInfraccion,
                        Fotos = _db.FotoInfraccions
                            .Where(f => f.idInfraccion == i.idFotoMulta)
                            .Select(f => f.NombreFoto)
                            .ToList()
                    })
                    .ToList();

                if (resultado.Count == 0)
                    return Content(HttpStatusCode.NotFound, "No se encontraron fotomultas para esta placa.");

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, "Error: " + ex.Message);
            }
        }
    }
}
