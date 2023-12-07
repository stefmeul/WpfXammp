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
using MySql.Data.MySqlClient;

namespace WpfXammp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

            string MySQLConnectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=wpfxammp";

            MySqlConnection databaseConnection = new MySqlConnection(MySQLConnectionString);

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

            }
            catch (Exception e) 
            {
                MessageBox.Show("query error" + e.Message);
            }
        }
    }
}
