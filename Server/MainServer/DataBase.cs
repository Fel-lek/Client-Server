using MySql.Data.MySqlClient;

//public enum CommandType
//{
//    Select,
//    Insert,
//    Update,
//    Delete,
//    Where,
//    And,
//    Or,
//    Not,
//    All,
//}

public class DataBase
{
    public static MySqlConnection databaseConnection;

    public DataBase()
    {
        try
        {
            databaseConnection = new MySqlConnection("server=127.0.0.1;uid=root;pwd=rootpass;database=SocketTest");
            databaseConnection.Open();
            Console.WriteLine($"Database connection successful!");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Database connection failed: {e}");
        }
    }

    //public void SqlCommand(/*CommandType commandType,*/string query)
    //{
    //    //MySqlCommand command = new MySqlCommand();

    //    MySqlDataReader reader;

    //    //switch (commandType)
    //    //{
    //    //    case CommandType.Select:
    //    //        query += "SELECT";
    //    //        break;
    //    //    case CommandType.Insert:
    //    //        query += "INSERT INTO";
    //    //        break;
    //    //    case CommandType.Update:
    //    //        query += "UPDATE";
    //    //        break;
    //    //    case CommandType.Delete:
    //    //        query += "DELETE";
    //    //        break;
    //    //    case CommandType.Where:
    //    //        query += "WHERE";
    //    //        break;
    //    //    case CommandType.And:
    //    //        query += "AND";
    //    //        break;
    //    //    case CommandType.Or:
    //    //        query += "OR";
    //    //        break;
    //    //    case CommandType.Not:
    //    //        query += "NOT";
    //    //        break;
    //    //    case CommandType.All:
    //    //        query += "*";
    //    //        break;
    //    //    default:
    //    //        query = null;
    //    //        break;
    //    //}

    //    MySqlCommand command = new MySqlCommand(query, databaseConnection);
    //    reader = command.ExecuteReader();

    //    while (reader.Read())
    //    {
    //        Console.WriteLine($"{reader[""]}");
    //    }
    //}

    //public string SqlQuery(string query)
    //{
    //    //string sqlLine = $"SELECT {dataToSelect} FROM {destination}";
    //    //string sqlLine = $"SELECT {dataToSelect} FROM {destination} WHERE PlayerName = 'Fellek'";
    //    return query;
    //}

    public bool GetUser(string username, string password)
    {
        MySqlCommand command = new MySqlCommand($"SELECT UserName, UserPass FROM players WHERE UserName = @username", databaseConnection);
        command.Parameters.AddWithValue("@username", username);
        using MySqlDataReader reader = command.ExecuteReader();

        if (!reader.Read())
            return false;

        if (username != reader.GetString("UserName"))
            return false;

        if (CheckPass(password, reader))
        {
            reader.Close();
            return true;
        }

        return false;
        //MySqlDataReader reader = command.ExecuteReader();

        //while (reader.Read())
        //{
        //    if(username == reader.GetString("PlayerName"))
        //    {
        //        Console.WriteLine("Name Successful");
        //        CheckPass(password, reader);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Name Unsuccessful");
        //    }
        //}
    }

    private bool CheckPass(string password, MySqlDataReader reader)
    {
        //MySqlCommand command = new MySqlCommand($"SELECT PlayerPass FROM players WHERE PlayerPass = '{password}'", databaseConnection);
        if (password == reader.GetString("UserPass"))
        {
            Console.WriteLine("Login Successful");
            Token token = new Token();
            return true;
        }
        else
        {
            Console.WriteLine("Login Unsuccessful");
            //Decline request to join server
            return false;
        }
    }

    //private async void ReadDatabase(MySqlCommand cmd, string password, MySqlDataReader reader, string username)
    //{

    //}

    public string GrabEmail(string username)
    {
        MySqlCommand command = new MySqlCommand($"SELECT UserName, UserEmail FROM players WHERE UserName = '{username}'", databaseConnection);
        using MySqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            Random rnd = new Random();
            int codeGen = rnd.Next() + rnd.Next() + rnd.Next() + rnd.Next() + rnd.Next() + rnd.Next();
            //send email
            return reader.GetString("UserEmail");
        }
        else
            return null;
    }
}