public class Logro
{
    public string idInterno;
    public string descripcion;
    public System.Func<bool> condicion; // Una función que verifica si el logro se cumple
    public bool desbloqueado = false;

    public Logro(string id, string desc, System.Func<bool> cond)
    {
        idInterno = id;
        descripcion = desc;
        condicion = cond;
    }
}
