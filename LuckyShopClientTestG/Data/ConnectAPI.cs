using LuckyShopClientTestG.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

public class ConnectAPI
{
    public ConnectAPI(Auth auth)
    {
        this.auth = auth;
        SetConnectionString();
        connection = new SqlConnection(connectionString);
        connection.StateChange += Connection_StateChange;

    }


    #region Class Attributes
    public bool isConnected = false;
    public string statusMessage = string.Empty;
    public SqlConnection connection;
    private ObservableCollection<Produkt> produktListe = new ObservableCollection<Produkt>();
    public ObservableCollection<Produkt> ProduktListe
    {
        get { return produktListe; }
        set { produktListe = value; }
    }



    private string productTableName = "Produkte";
    private string orderTableName = "Bestellungen";

    private string connectionString;
    private Auth auth;
    #endregion

    #region Public Methods
    public void Connect()
    {

        try
        {
            if (!isConnected)
            {
                connection.Open();
            }

            FetchList();

        }
        catch (Exception)
        {
            statusMessage = $"Can't establish connection to table: {productTableName}.";

            throw;
        }
    }

    public void SetConnectionString()
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        //builder.DataSource = auth.serverName + "," + auth.port;
        //builder.UserID = auth.userName;
        //builder.Password = auth.password;
        //builder.InitialCatalog = auth.databaseName;
        //builder.TrustServerCertificate = true;
        //builder.Encrypt = true;

        connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
            "Initial Catalog=Shop;Integrated Security=True;" +
            "Pooling=False;" +
            "Encrypt=True;" +
            "Trust Server Certificate=False";
     }
    public void FetchList()
    {
        string sqlQuery = BuildSelectionQuery();
        if (!isConnected)
        {
            return;
        }
        ProduktListe?.Clear();
        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {

                    string produktID = reader.GetString(0);
                    decimal preis = reader.GetDecimal(1);
                    string bez = reader.GetString(2);
                    Produkt data = new Produkt(
                        ProduktID: produktID,
                        Preis: preis,
                        Bezeichnung: bez);


                    ProduktListe.Add(data);
                }
                reader.Close();
            }

            command.Dispose();
        }
    }

  

    public void Close()
    {
        if (connection.State != ConnectionState.Closed)
        {
            statusMessage = $"Disconnected from table: {productTableName}. ";
            ProduktListe.Clear();
            connection?.Close();
        }
    }

    public void UpdateList(Produkt updated)
    {
        if (!isConnected)
        {
            return;
        }

        ProduktListe?.Clear();

        FetchList();
    }

    public void BestellungAufnehmen(Kunde kunde, Bestellung bestellung)
    {

        if (!isConnected)
        {
            try
            {
                connection.Open();
                isConnected = true;
            }
            catch (Exception)
            {
                isConnected = false;
                throw;
            }
            
        }


        using (SqlCommand command = BuildKundenUpdateQuery(kunde))
        {
            _ = command.ExecuteNonQuery();
        }

        // Create or update the customer table. ToDo: Add proper login for existing customer and create new entry for customer.

        using (SqlCommand command = BuildBestellungenInsertQuery(bestellung))
        {
            _ = command.ExecuteNonQuery();
        }


    }
    #endregion

    #region Private Methods
    private void Connection_StateChange(object sender, StateChangeEventArgs e)
    {

        isConnected = e.CurrentState == ConnectionState.Open;
        if (isConnected)
        {
            statusMessage = $"Connected to Table: {productTableName}";
        }

        statusMessage += ($"Connection state changed from {e.OriginalState} to {e.CurrentState}.");
    }


    private string BuildSelectionQuery()
    {
        List<string> attributesSelections = new List<string>
        {
            "ProduktID",
            "Preis",
            "Bezeichnung"
        };

        string selections = string.Join(",", attributesSelections);

        return "Select " + selections + " from " + productTableName;
    }



    private SqlCommand BuildBestellungenInsertQuery(Bestellung bestellung)
    {
        SqlCommand command = new SqlCommand(
            $"INSERT INTO {orderTableName} " +
            $"(BestellID,Produkte,Gesamtsumme,KundenID) " +
            $"SELECT @BestellID,@Produkte,@Gesamtsumme,@KundenID");

        command.Parameters.AddWithValue("@BestellID", bestellung.BestellID);
        command.Parameters.AddWithValue("@Produkte", String.Join(";", bestellung.Produkte));
        command.Parameters.AddWithValue("@Gesamtsumme", bestellung.Gesamtsumme);
        command.Parameters.AddWithValue("@KundenID", bestellung.KundenID);

        command.Connection = connection;

        return command;

    }

    private SqlCommand BuildKundenUpdateQuery(Kunde entry)
    {

        string k_tableName = "Kunden";
        SqlCommand command = new SqlCommand($"UPDATE {k_tableName} " +
            "SET KundenID= @KundenID , " +
            "Adresse= @Adresse , " +
            "Name= @Name " +
            "WHERE KundenID= @KundenID;" +
            $"IF NOT EXISTS(SELECT 1 FROM {k_tableName} WHERE KundenID = @KundenID) " +
            $"BEGIN " +
            $"INSERT INTO {k_tableName} (KundenID,Adresse,Name) " +
            "SELECT @KundenID,@Adresse,@Name " +
            "END", connection);

        command.Parameters.AddWithValue("@KundenID", entry.KundenID);
        command.Parameters.AddWithValue("@Adresse", entry.Adresse);
        command.Parameters.AddWithValue("@Name", entry.Name);


        return command;

    }

    //private SqlCommand BuildDeleteQuery(Produkt item)
    //{

    //    SqlCommand command = new SqlCommand($"DELETE FROM {tableName} WHERE id=@id", connection);
    //    command.Parameters.AddWithValue("@id", item.ProduktID);

    //    return command;
    //}

    #endregion
}
