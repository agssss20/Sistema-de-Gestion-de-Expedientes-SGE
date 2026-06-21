
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public record class ListarTodosLosExpedientesResponse(IEnumerable<Expediente> Expedientes);
