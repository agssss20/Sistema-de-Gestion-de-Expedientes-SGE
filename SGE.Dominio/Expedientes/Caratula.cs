
using SGE.Dominio.Comun;

namespace SGE.Dominio.Expedientes;

//genero la record class para garantizar que el contenido de mi caratula no sea null
public record class Caratula
{
    public string Valor{get;}
    public Caratula(string valor)
    {
        if(string.IsNullOrWhiteSpace(valor))
            throw new DominioException(" La caratula no puede estar vacia.");

        Valor = valor.Trim();
    }

    public override string? ToString() => Valor;
}
