using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;
// se encarga de buscar el ultimo tramite de un expediente dado, y a partir de su etiqueta llamar al metodo del expediente que evalua el nuevo estado 
// si esa llamada retorna true, se efectua el cambio de estado llamando a modificar expediente, sino, no se hace nada, porque el estado del expediente no cambia
// Para evitar repetir código nos basamos en el Principio DRY ("Don't Repeat yourself") y lo aplicamos en los tres casos de uso 
public class ActualizacionEstadoExpedienteService (ITramiteRepository tramiteRepository, IExpedienteRepository expedienteRepository)
{
    public void Ejecutar(Guid IdExp, Guid IdUser)
    {
        var expediente = expedienteRepository.obtenerExpedientePorId(IdExp); // busca el expediente por el id que le manda el usecase que instancie esto
        if(expediente == null){
            throw new ApplicationException("El expediente no existe");
        }
        IEnumerable<Tramite> tramites = tramiteRepository.ListarTramitesPorExpediente(IdExp); // busca una lista IEnumerable de los tramites de ese id de expediente
        IEnumerable<Tramite> tramitesOrdenados = tramites.OrderByDescending<Tramite, DateTime>(t => t.FechaCreacion); // ordnena el IEnumerable de tramites por fecha de creacion, de mas nuevo a mas viejo
        Tramite? t = tramitesOrdenados.FirstOrDefault(); // agarra el primer tramite, llama al metodo que actualiza el estado del expediente
        bool seActualiza = expediente.ActualizarEstado(t?.Etiqueta, IdUser); // no hace falta comprobar la condicion de etiqueta, si es null, en el mismo metodo se setea el estado en recienIniciado
        if(seActualiza)
        {
            expedienteRepository.ModificarExpediente(expediente); // si se actualiza el estado del expediente, se llama al metodo modificar expediente para que se guarde el cambio
        }
    }
}