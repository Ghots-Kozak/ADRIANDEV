using Context;
using System.Text.Json;

namespace Control_Piso.Models
{
    public partial class ApplicationDbContext : ContextDB
    {
        public ApplicationDbContext()
        {

        }

        public static bool EsJsonValido(string cadena)
        {
            try
            {
                using (JsonDocument.Parse(cadena))
                {
                    return true;
                }
            }
            catch (JsonException)
            {
                return false;
            }
        }

    }
}
