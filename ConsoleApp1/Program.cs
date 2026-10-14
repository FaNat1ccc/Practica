using System;
using System.Data;
using System.Data.Common;

namespace DataTableStates
{
    class Program
    {
        static void Main()
        {
            DataTable table = new DataTable("Students");
            table.Columns.Add("student_id", typeof(int));
            table.Columns.Add("first_name", typeof(string));
            table.Columns.Add("last_name", typeof(string));
            table.Columns.Add("group_number", typeof(string));

            try
            {
                string connString = "Host=localhost;Port=5432;Database=university;Username=postgres;Password=postgres";
                DbProviderFactory factory = DbProviderFactories.GetFactory("Npgsql");

                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = connString;
                    connection.Open();

                    DbCommand command = factory.CreateCommand();
                    command.Connection = connection;
                    command.CommandText = "SELECT student_id, first_name, last_name, group_number FROM students";

                    DbDataAdapter adapter = factory.CreateDataAdapter();
                    adapter.SelectCommand = command;

                    adapter.Fill(table);

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось подключиться к БД: {ex.Message}");
                Console.WriteLine("Используются тестовые данные.\n");

                table.Rows.Add(1, "Алексей", "Иванов", "ИТ-101");
                table.Rows.Add(2, "Никита", "Смирнов", "ИТ-102");
                table.Rows.Add(3, "Виктор", "Сидоров", "ИТ-103");
            }

            table.AcceptChanges();

            Console.WriteLine(" После загрузки ");
            PrintRowCount(table);
            PrintRowStates(table);

            Console.WriteLine("\n Создание строки через NewRow() ");
            DataRow newRow = table.NewRow();
            newRow["student_id"] = 999;
            newRow["first_name"] = "Иван";
            newRow["last_name"] = "Петров";
            newRow["group_number"] = "ИТ-201";
            Console.WriteLine($"Состояние новой строки до добавления: {newRow.RowState}");

            Console.WriteLine("\n Добавление строки в таблицу ");
            table.Rows.Add(newRow);
            PrintRowCount(table);
            PrintRowStates(table);

            Console.WriteLine("\n Изменение имени существующего студента ");
            DataRow rowToModify = table.Rows[0];
            rowToModify["first_name"] = "Александр";
            PrintRowStates(table);

            Console.WriteLine("\n Удаление другого студента ");
            DataRow rowToDelete = table.Rows[1];
            rowToDelete.Delete();
            PrintRowStates(table);

            Console.WriteLine("\n Вызов RejectChanges() ");
            table.RejectChanges();
            PrintRowCount(table);
            PrintRowStates(table);

            Console.WriteLine("\n Повторение изменений ");
            DataRow newRow2 = table.NewRow();
            newRow2["student_id"] = 998;
            newRow2["first_name"] = "Мария";
            newRow2["last_name"] = "Сидорова";
            newRow2["group_number"] = "ИТ-202";
            table.Rows.Add(newRow2);

            table.Rows[0]["first_name"] = "Дмитрий";
            table.Rows[1].Delete();

            Console.WriteLine("Перед AcceptChanges():");
            PrintRowCount(table);
            PrintRowStates(table);

            Console.WriteLine("\n Вызов AcceptChanges() ");
            table.AcceptChanges();
            PrintRowCount(table);
            PrintRowStates(table);
        }

        static void PrintRowCount(DataTable table)
        {
            Console.WriteLine($"Количество строк в DataTable: {table.Rows.Count}");
        }

        static void PrintRowStates(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                {
                    Console.WriteLine($"ID: {row["student_id"]}, Имя: {row["first_name"]}, Состояние: {row.RowState}");
                }
                else
                {
                    Console.WriteLine($"ID: {row["student_id", DataRowVersion.Original]}, Имя: {row["first_name", DataRowVersion.Original]}, Состояние: {row.RowState}");
                }
            }
        }
    }
}