using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class DbObject
    {
        public DbObjectType Type { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public string ParentName { get; set; }
        public DbObjectType ParentType { get; set; }
        public string DataType { get; set; }
        public bool IsNullable { get; set; }
        public int NumberOfChildren { get; set; }

        private DbObject() { }

        public static DbObject FromString(string line)
        {
            var values = line.Split(';');
            if (values.Length != 7) throw new ArgumentException($"Incorrect line format: line has {values.Length} values");

            return new DbObject
            {
                Type = DbObjectTypeConverter.FromString(values[0]),
                Name = values[1].Trim().Replace(" ", string.Empty),
                Schema = values[2].Trim(),
                ParentName = values[3].Trim().Replace(" ", string.Empty),
                ParentType = DbObjectTypeConverter.FromString(values[4]),
                DataType = values[5].Trim() == string.Empty ? "NULL" : values[5].Trim(),
                IsNullable = values[6] == "1",
                NumberOfChildren = 0
            };
        }
    }
}
