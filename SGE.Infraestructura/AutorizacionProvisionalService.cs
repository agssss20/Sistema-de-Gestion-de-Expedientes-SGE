
using SGE.Aplicacion.Autorizacion;

namespace SGE.Infraestructura;

public class AutorizacionProvisionalService: IAutorizacionService
{
    public bool TienePermiso(Guid userId, Permiso permiso)
    {
        return true;
    }
}

