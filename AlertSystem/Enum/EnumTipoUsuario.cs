using System;
using System.Text.Json.Serialization;

namespace AlertSystem.Enum
{
    [Serializable]
    public enum EnumTipoUsuario
    {
        Padrao = 0,
        Tecnico = 1,
        Administrador = 2,
    }
}
