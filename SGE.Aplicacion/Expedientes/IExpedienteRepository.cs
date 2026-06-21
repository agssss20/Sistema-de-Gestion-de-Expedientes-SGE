
using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Expedientes;

public interface IExpedienteRepository
{
    IEnumerable<Expediente> ListarTodosLosExpedientes();
    Expediente? obtenerExpedientePorId(Guid id);
    void AgregarExpediente(Expediente expediente);
    void ExpedienteBaja(Guid idBaja); 
    void ModificarExpediente(Expediente expModificacion);
}
