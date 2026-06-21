
using SGE.Dominio.Tramites;
namespace SGE.Aplicacion.Tramites;

public record class TramiteDTO(Guid expedienteId, EtiquetaTramite Etiqueta, ContenidoTramite? Contenido); 
// el DTO del tramite, con los datos necesarios para mostrarlo en la vista, como el id del expediente al que pertenece, 
// la etiqueta y el contenido del tramite, que puede ser nulo si el tramite no tiene contenido (por ejemplo, si es un tramite de baja)
