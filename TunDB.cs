
using System.Data.SQLite;
using System.Data;

namespace TunaszUtils.DB
{
    public static class TunDB
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="selectItems"></param>
        /// <param name="whereItems">1st Name 2nd condition 3rd values</param>
        /// <returns></returns>
        public static List<List<object>> SelectFromTable(string db, string table, object[] selectItems, object[] whereItems)
        {
            List<List<object>> dataToRerurn = new List<List<object>>();
            string selectedItems = "";
            string whereStringItems = "";
            for (int i = 0; i < selectItems.Length; i++)
            {
                selectedItems += selectItems[i] + ",";
            }
            if (whereItems.Length > 0)
            {
                whereStringItems += "WHERE ";
            }
            for (int i = 0; i < whereItems.Length; i += 3)
            {
                string where1 = whereItems[i + 2].GetType() == typeof(string) ? $"'{whereItems[i + 2]}'" : $"{whereItems[i + 2]}";
                whereStringItems += $"{whereItems[i]}{whereItems[i + 1]}{where1},";
            }

            selectedItems = selectedItems.TrimEnd(',');
            whereStringItems = whereStringItems.TrimEnd(',');
            using (var con = new SQLiteConnection(db))
            {
                con.Open();

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $"SELECT {selectedItems} FROM {table} {whereStringItems}";

                    try
                    {
                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            int j = 0;
                            while (reader.Read())
                            {
                                string itemData = "";
                                dataToRerurn.Add(new List<object>());
                                for (int i = 0; i < selectItems.Length; i++)
                                {
                                    itemData += $"{selectItems[i]}: {reader[selectItems[i].ToString()]}\t";
                                    dataToRerurn[j].Add(reader[selectItems[i].ToString()]);
                                }
                                j++;
                            }
                            reader.Close();
                        }

                    }
                    catch (Exception)
                    {
                        return new List<List<object>>();
                    }
                }
                con.Close();
            }
            return dataToRerurn;

        }

        public static void ClearTable(string db,string table)
        {
            RunDataBaseCommand(db,$"DELETE FROM {table};");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="setValues">First is the name, second is value</param>
        /// <param name="conditions">First is name, second is value</param>
        public static bool UpdateTable(string db,string table, object[] setValues, object[] conditions)
        {

            string setValuesString = "";
            string conditionsString = "";
            for (int i = 0; i < setValues.Length; i++)
            {
                if (i % 2 == 0)
                {
                    setValuesString += $"{setValues[i]}=";
                }
                else
                {
                    if (setValues[i].GetType() == typeof(string)) setValuesString += $"'{setValues[i]}',";
                    else setValuesString += $"{setValues[i]},";
                }

            }
            for (int i = 0; i < conditions.Length; i++)
            {
                if (i % 2 == 0)
                {
                    conditionsString += $"{conditions[i]}=";
                }
                else
                {
                    if (conditions[i].GetType() == typeof(string)) conditionsString += $"'{conditions[i]}',";
                    else conditionsString += $"{conditions[i]},";
                }

            }
            setValuesString = setValuesString.TrimEnd(',');
            conditionsString = conditionsString.TrimEnd(',');
            return RunDataBaseCommand(db,$"Update {table} SET {setValuesString} WHERE {conditionsString}");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableItems">First is the name, second is the value</param>
        public static void InsertIntoTable(string db,string table, params object[] tableItems)
        {
            string itemNames = "";
            string itemValues = "";
            for (int i = 0; i < tableItems.Length; i++)
            {
                if (i % 2 == 0)
                {
                    itemNames += $"{tableItems[i]},";
                }
                else
                {
                    if (tableItems[i].GetType() == typeof(string)) itemValues += $"'{tableItems[i]}',";
                    else itemValues += $"{tableItems[i]},";
                }

            }
            itemNames = itemNames.TrimEnd(',');
            itemValues = itemValues.TrimEnd(',');
            RunDataBaseCommand(db,$"INSERT INTO {table} ({itemNames}) VALUES({itemValues})");

        }

        public static bool RunDataBaseCommand(string db,string commandText)
        {
            try
            {
                using (var con = new SQLiteConnection(db))
                {
                    con.Open();

                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = commandText;
                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public static void CreateTable(string db, string tableName, params string[] tableParams)
        {
            string everyTableParam = "";
            for (int i = 0; i < tableParams.Length; i++)
            {
                everyTableParam += tableParams[i] + ",";
            }
            everyTableParam = everyTableParam.TrimEnd(',');
            RunDataBaseCommand(db, $"CREATE TABLE IF NOT EXISTS {tableName} ({everyTableParam});");
        }

    }
}
