
using SGE.Dominio.Tramites;
namespace SGE.Aplicacion.Tramites;
// interfaz para aplicar metodos en la entidad tramite
public interface ITramiteRepository
{
    IEnumerable<Tramite> ListarTramitesPorExpediente(Guid idExpediente); //paso el id del expediente para asi armar todos los tramites de un expediente 
    IEnumerable<Tramite> ListarTodosLosTramites();
    void AgregarTramite(Tramite tramite);
    void TramiteBaja(Guid idBaja); 
    void ModificarTramite(Tramite tramModificacion);
    Tramite obtenerTramitePorId(Guid id);
    void EliminarPorExpedienteId(Guid expedienteId);
}
