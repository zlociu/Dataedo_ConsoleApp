namespace ConsoleApp
{
    public enum DbObjectType
    {
        DATABASE,
        TABLE,
        COLUMN,
        UNKNOWN = 99
    }

    public static class DbObjectTypeConverter
    {
        public static DbObjectType FromString(string type)
        {
            var type_unified = type.Trim().Replace(" ", string.Empty).ToUpper();
            switch (type_unified)
            {
                case "DATABASE": return DbObjectType.DATABASE;
                case "TABLE": return DbObjectType.TABLE;
                case "COLUMN": return DbObjectType.COLUMN;
                default: return DbObjectType.UNKNOWN;
            }
        }
    }
}

