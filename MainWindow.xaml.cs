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
using Npgsql;

namespace Milestone1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class business
        {
            public string bid { get; set; }
            public string name { get; set; }
            public string state { get; set; }
            public string city { get; set; }



        }
        public MainWindow()
        {
            InitializeComponent();
            addState();
            addColumns2Grid();

        }

        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = milestone1db; password=gabespc1";
        }

        private void addState()
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT distinct state FROM business ORDER BY state";
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            statesList.Items.Add(reader.GetString(0));
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                        System.Windows.MessageBox.Show("SQL Error - " + ex.Message.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        private void addColumns2Grid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("name");
            col1.Header = "BusinessName";
            col1.Width = 255;
            BusinessGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("State");
            col2.Header = "State";
            col2.Width = 60;
            BusinessGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("City");
            col3.Header = "City";
            col3.Width = 150;
            BusinessGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("bid");
            col4.Header = "";
            col4.Width = 0;
            BusinessGrid.Columns.Add(col4);
        }

        private void statesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CityList.Items.Clear();
            if (statesList.SelectedIndex >= -1)
            {
                using (var connection = new NpgsqlConnection(buildConnectionString()))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand())
                    {

                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT distinct city FROM business WHERE state = '" + statesList.SelectedItem.ToString().ToUpper() + "' ORDER BY city";
                        try
                        {
                            var reader = cmd.ExecuteReader();
                            while (reader.Read())
                                CityList.Items.Add(reader.GetString(0));
                        }
                        catch (NpgsqlException ex)
                        {
                            Console.WriteLine(ex.Message.ToString());
                            System.Windows.MessageBox.Show("SQL Error - " + ex.Message.ToString());
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void CityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BusinessGrid.Items.Clear();
            if (CityList.SelectedIndex >= -1)
            {
                using (var connection = new NpgsqlConnection(buildConnectionString()))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand())
                    {

                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT name, state, city, business_id FROM business WHERE state = '" + statesList.SelectedItem.ToString().ToUpper() + "' AND city = '" + CityList.SelectedItem.ToString() + "' ORDER BY city;";
                        try
                        {
                            var reader = cmd.ExecuteReader();
                            while (reader.Read())
                                BusinessGrid.Items.Add(new business() { name = reader.GetString(0), state = reader.GetString(1), city = reader.GetString(2), bid = reader.GetString(3) });
                        }
                        catch (NpgsqlException ex)
                        {
                            Console.WriteLine(ex.Message.ToString());
                            System.Windows.MessageBox.Show("SQL Error - " + ex.Message.ToString());
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void BusinessGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BusinessGrid.SelectedIndex >= 0)
            {
                business B = BusinessGrid.Items[BusinessGrid.SelectedIndex] as business;
                if ((B.bid != null) && (B.bid.ToString().CompareTo("")!=0))
                {
                    BusinessDetails businessWindow = new BusinessDetails(B.bid.ToString());
                    businessWindow.Show();
                }
            }
        }
    }
}
