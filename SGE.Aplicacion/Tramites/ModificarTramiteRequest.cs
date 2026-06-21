
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;

public record class ModificarTramiteRequest // Sirve para que el servidor entienda qué quiere el usuario y con qué datos.
(
    Guid ExpId,
    Guid UsuarioUltModificacion,
    ContenidoTramite ContenidoTramite,
    EtiquetaTramite Etiqueta 
);
