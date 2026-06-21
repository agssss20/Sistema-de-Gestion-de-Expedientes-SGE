
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;

public record class ListarTramitesPorExpedienteResponse(IEnumerable<Tramite> Tramites);
