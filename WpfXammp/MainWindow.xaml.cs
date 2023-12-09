using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;

namespace WpfXammp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static private string MySQLConnectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=wpfxammp";
        static private MySqlConnection databaseConnection = new MySqlConnection(MySQLConnectionString);
        private string UserId;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnExecQuery_Click(object sender, RoutedEventArgs e)
        {
            runQuery();
        }




        private void runQuery()
        {
            string query = txbInputQuery.Text;
             
            if(query == "")
            {
                MessageBox.Show("Please input sql query");
                return;
            }

            //MySQLConnectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=wpfxammp";
            //MySqlConnection databaseConnection = new MySqlConnection(MySQLConnectionString);

            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;

            try
            {
                databaseConnection.Open();

                MySqlDataReader myReader = commandDatabase.ExecuteReader();

                if (myReader.HasRows) 
                {
                    MessageBox.Show("query results in console");

                    while (myReader.Read())
                    {                                   // id                          // name              // email                        // balans
                        Console.WriteLine(myReader.GetString(0) + " - " + myReader.GetString(1) + " - " + myReader.GetString(2) + " - " + myReader.GetString(3) + " - ");
                    }
                }
                else
                {
                    MessageBox.Show("query succesful");
                }
                databaseConnection.Close();

            }
            catch (Exception e) 
            {
                MessageBox.Show("query error" + e.Message);
                databaseConnection.Close();

            }
        }



        private void InsertUser()
        {
            if (txbEmail.Text == "" || txbNaam.Text == "") // add if contains char ','
            {
                MessageBox.Show("Please fill in Name and Email.");
            } 
            
            else
            {

                string query = "INSERT INTO personen(Id, Name, Email, Balans) VALUES ('NULL', '" + txbNaam.Text + "', '" + txbEmail.Text + "', '50')";

                databaseConnection = new MySqlConnection(MySQLConnectionString);
                MySqlCommand CommandDb = new MySqlCommand(query, databaseConnection);
                CommandDb.CommandTimeout = 60;

                try
                {
                    databaseConnection.Open();
                    MySqlDataReader myReader = CommandDb.ExecuteReader();
                
                        MessageBox.Show("Registration succesful.");
             
                    databaseConnection.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void listUsers() 
        {
            lsbDataBase.Items.Clear();

            string query = "SELECT * FROM personen";

            databaseConnection = new MySqlConnection(MySQLConnectionString);
            MySqlCommand CommandDb = new MySqlCommand(query, databaseConnection);
            CommandDb.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {
                databaseConnection.Open();
                reader = CommandDb.ExecuteReader();
                
                if (reader.HasRows)
                {
                    while (reader.Read()) 
                    {
                                            // Id               // Name             // Email            // Balans
                        string[] row = { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3) };
                        ListBoxItem DbItem = new ListBoxItem();
                       
                        for (int i = 0; i < row.Length; i++)
                        {
                            string temp = row[i].ToString();
                            DbItem.Content += temp + ", ";
                        }
                        lsbDataBase.Items.Add(DbItem);
                    }

                }
               else
                {
                    MessageBox.Show("No users found");
                }

                databaseConnection.Close(); 
            }
            catch (Exception e)
            { 
                MessageBox.Show(e.Message); 
            }

            txbBalans.Text = "balans";
            txbEmail.Text = "email";
            txbNaam.Text = "name";

        }


        private void updateUser()
        {
            string query = "UPDATE personen SET Name = '" + txbNaam.Text + "', Email = '" + txbEmail.Text + "', Balans = '" + txbBalans.Text + "' WHERE Id = ' " + UserId + " '";
           
            databaseConnection = new MySqlConnection(MySQLConnectionString);
            MySqlCommand CommandDb = new MySqlCommand(query, databaseConnection);
            CommandDb.CommandTimeout = 60;

            try
            {
                databaseConnection.Open();
                MySqlDataReader myReader = CommandDb.ExecuteReader();

                MessageBox.Show("User update succesful.");

                databaseConnection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


        }


        private void deleteUser()
        {
            string query = "DELETE FROM personen WHERE Id = ' " + UserId + " '";

            databaseConnection = new MySqlConnection(MySQLConnectionString);
            MySqlCommand CommandDb = new MySqlCommand(query, databaseConnection);
            CommandDb.CommandTimeout = 60;

            try
            {
                databaseConnection.Open();
                MySqlDataReader myReader = CommandDb.ExecuteReader();

                MessageBox.Show("User deleted ");

                databaseConnection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    


// Event Handlers

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            InsertUser();
            listUsers();

        }

        private void btnShowUsers_Click(object sender, RoutedEventArgs e)
        {
            listUsers();
        }

        private void lsbDataBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selected = (ListBoxItem)lsbDataBase.SelectedItem;
            if ( selected != null)
            {

            List<string> waarden = new List<string>();

            foreach (ListBoxItem item in lsbDataBase.SelectedItems)
            {
                waarden.Add(item.Content.ToString());
            }
            string temp = String.Join(", ", waarden);

            string[] substrings = temp.Split(',');

            UserId = substrings[0];
            string name = substrings[1];
            string email = substrings[2];
            string balans = substrings[3];

            name = name.Trim();
            email = email.Trim();
            balans = balans.Trim();

            txbNaam.Text = name;
            txbEmail.Text = email;
            txbBalans.Text = balans;

            lblView.Content = temp + "userId " + UserId;
            }

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            updateUser();
            listUsers();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteUser();
            listUsers();
        }
    }
}
