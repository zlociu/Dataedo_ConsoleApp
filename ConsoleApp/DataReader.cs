namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DataReader
    {
        private readonly IList<DbObject> _dbObjectList;
        private readonly string _fileName;

        public DataReader(string fileName)
        {
            _fileName = fileName;
            _dbObjectList = new List<DbObject>();
        }

        private void ImportData()
        {   
            // skip first row because it contains column names
            var lines = File.ReadAllLines(_fileName).Skip(1);

            foreach(var line in lines)
            {
                // if line is empty, just skip it
                if (! string.IsNullOrWhiteSpace(line)) 
                {
                    try
                    {
                        var obj = DbObject.FromString(line);
                        _dbObjectList.Add(obj);
                    }
                    catch(Exception ex)
                    {
                        Console.Error.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
        }

        private void AssingChildren()
        {
            for(int i = 0; i < _dbObjectList.Count; i++) 
            {
                var parent = _dbObjectList[i];
                parent.NumberOfChildren = _dbObjectList
                    .Where(child => child.ParentType == parent.Type && child.ParentName == parent.Name)
                    .Count();
            }
        }
        
        private void PrintData()
        {
            var databases = _dbObjectList.Where(x => x.Type == DbObjectType.DATABASE);
            foreach (var database in databases)
            {
                Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");
                var tables = _dbObjectList
                    .Where(x => x.Type == DbObjectType.TABLE && 
                        x.ParentType == DbObjectType.DATABASE && 
                        x.ParentName == database.Name);

                // print all database's tables
                foreach (var table in tables)
                {
                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");
                    var columns = _dbObjectList
                        .Where(x => x.Type == DbObjectType.COLUMN && 
                            x.ParentType == DbObjectType.TABLE && 
                            x.ParentName == table.Name);

                    // print all table's columns
                    foreach (var column in columns)
                    {
                        Console.WriteLine(
                            $"\t\tColumn '{column.Name}' with {column.DataType} data type " +
                            $"{(column.IsNullable == true ? "accepts nulls" : "with no nulls")}");
                    }
                }
            }
        }

        public void PrepareAndPrintData()
        {
            ImportData();
            AssingChildren();
            PrintData();
            Console.ReadLine();
        }
    }
}