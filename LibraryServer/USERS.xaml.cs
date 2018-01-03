using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace LibraryServer
{
    /// <summary>
    /// Interaction logic for USERS.xaml
    /// </summary>
    public partial class USERS : UserControl
    {
        public USERS()
        {
            InitializeComponent();
            int ancurrent = DateTime.Now.Year;
            for (int i = ancurrent; i < ancurrent+7; i++)
            {
                ComboBoxAn.Items.Add(i);
            }
        }

        private String oldProfession;


        //FIELDS CAN NOT BE EMPTY!!!

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if(!CheckEmptyFields())
            {
                var message = "Please make sure all the fields are completed";
                SnackBarDisplay1(message, 2000);
                e.Handled = true;
                return;
            }
            if(ButtonSave.Content as string =="ADD")
            {
                AddUser();
            }
            else
            {
                UpdateUser();
            }
        }

        /// <summary>
        /// Checks all the fields have been completed before Saving
        /// </summary>
        /// <returns></returns>
        private bool CheckEmptyFields()
        {
            if (TextBoxCNP.Text == "") return false;
            if (TextBoxNume.Text == "") return false;
            if (TextBoxPreNume.Text == "") return false;
            if (TextBoxProfilePic.Text == "") return false;
            if (ComboBoxProfession.SelectedIndex == -1) return false;
            if(ComboBoxProfession.SelectionBoxItem.ToString()=="Student")
            {
                if (ComboBoxAn.SelectedIndex == -1) return false;
            }

            return true;

        }

        //check if userprofesion changed to delete

        private void UpdateUser()
        {
            //check if userprofesion changed, if so, delete entry and create one in the other database
            String profesie = ComboBoxProfession.SelectionBoxItem.ToString();
            if(oldProfession!=profesie)
            {
                DeleteEntry();
                return;
            }
            try
            {
                Console.WriteLine("Updating User");
                using (SqlConnection con = new SqlConnection(new BOOKS().SQL_ConnectionString()))
                {
                    con.Open();
                    String sql = "update " + profesie + "s set Nume=@nume, Prenume=@prenume, Profilepic=@profilepic";
                    if(profesie=="Student")
                    {
                        sql += ", An_absolvire=@anabs";
                    }
                    sql += " where CNP=" + TextBoxCNP.Text;
                    Console.WriteLine("Da Sql update is " + sql);
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@nume", TextBoxNume.Text);
                        cmd.Parameters.AddWithValue("@prenume", TextBoxPreNume.Text);
                        cmd.Parameters.AddWithValue("@profilepic", TextBoxProfilePic.Text);
                        if(profesie=="Student")
                        {
                            cmd.Parameters.AddWithValue("@anabs", ComboBoxAn.SelectionBoxItem.ToString());
                        }
                        cmd.ExecuteNonQuery();

                        Console.WriteLine("User modified successfully");
                        var message = "User Data modified successfully";
                        SnackBarDisplay1(message, 2000);
                        ResetFields();

                    }
                }
            }
            catch(SqlException e)
            {
                MessageBox.Show(e.ToString());
            }
        }


        private void DeleteEntry()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(new BOOKS().SQL_ConnectionString()))
                {
                    Console.WriteLine("Deleting entry from old table");
                    con.Open();
                    string sql = "delete from ";
                    sql += oldProfession + "s where CNP=@cnp";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@cnp", TextBoxCNP.Text);
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Entry deleted successfully..");
                        AddUser();
                    }
                }
            }
            catch (SqlException e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private void AddUser()
        {
            try
            {
                Console.WriteLine("Registering new user");
                String profesia = ComboBoxProfession.SelectionBoxItem.ToString();
                String sql = "insert into " + profesia;
                sql += "s (CNP,Nume,Prenume,";
                if(profesia=="Student")
                {
                    sql += "An_absolvire,";
                }
                sql += "Profilepic) values(@cnp,@nume,@prenume,";
                if(profesia=="Student")
                {
                    sql += "@anabs,";
                }
                sql += "@profilepic)";
                using (SqlConnection con = new SqlConnection(new BOOKS().SQL_ConnectionString()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@cnp", TextBoxCNP.Text);
                        cmd.Parameters.AddWithValue("@nume", TextBoxNume.Text);
                        cmd.Parameters.AddWithValue("@prenume", TextBoxPreNume.Text);
                        cmd.Parameters.AddWithValue("@profilepic", TextBoxProfilePic.Text);
                        if (profesia == "Student")
                        {
                            cmd.Parameters.AddWithValue("@anabs", ComboBoxAn.SelectionBoxItem.ToString());
                        }
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("User Registered successfully");
                        var message = "User Registered Successfully";
                        SnackBarDisplay1(message, 2000);
                        ResetFields();
                    }
                }
            }
            catch(SqlException e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private void SnackBarDisplay1(string message, int timeSpan)
        {
            if (MainWindow.tickCount < 1 + timeSpan / 1000) return;
            SnackbarDisplay.MessageQueue = new MaterialDesignThemes.Wpf.SnackbarMessageQueue(TimeSpan.FromMilliseconds(timeSpan));
            SnackbarDisplay.MessageQueue.Enqueue(message);
            MainWindow.tickCount = 0;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            ResetAnABS();
            ResetFields();
        }

        #region SnackBar
        /// <summary>
        /// Display various messages on Snackbar
        /// </summary>
        /// <param name="message">What message to be displayed</param>
        /// <param name="timeSpan">the time to be displayed in milliseconds</param>
        private void SnackBarDisplay(string message, int timeSpan)
        {
            if (MainWindow.tickCount < 1 + timeSpan / 1000) return;
            SnackbarDisplay.MessageQueue = new MaterialDesignThemes.Wpf.SnackbarMessageQueue(TimeSpan.FromMilliseconds(timeSpan));
            SnackbarDisplay.MessageQueue.Enqueue(message);
            MainWindow.tickCount = 0;
        }
        #endregion

        private void TextBoxCNP_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9) && !(new Home().GetPressedKey(e)))
            {
                //TextBoxCNP.Text = "";
                var message = "CNP must contain only numbers!";
                SnackBarDisplay(message, 1000);
                e.Handled = true;
            }
            if (e.Key == Key.Enter||TextBoxCNP.Text.Length==13)
            {
                if (TextBoxCNP.Text.Length != 13)
                {
                    var message = "Please enter a valid CNP";
                    SnackBarDisplay1(message, 1000);
                    e.Handled = true;
                    return;
                }
                if (!CheckStringOnlyNumbers(TextBoxCNP.Text))
                {
                    TextBoxCNP.Text = "";
                    var message = "CNP must contain only numbers!";
                    SnackBarDisplay(message, 1000);
                    e.Handled = true;
                }
                else
                {
                    if(TextBoxCNP.Text.Length<13)
                    {
                        Console.WriteLine("Attempted to querry a invalid CNP...Aborting");
                        var message = "CNP should be no more or less than 13 digits. Are you sure the CNP is right?";
                        SnackBarDisplay(message, 1000);
                        e.Handled = true;
                        return;
                    }
                    if(SQLSEARCH())
                    {
                        ButtonSave.Content = "Modify";
                        TextBoxCNP.IsEnabled = false;
                    }
                    else
                    {
                        ButtonSave.Content = "ADD";
                    }

                 
                }
            }
        }

        /// <summary>
        /// Checks if the Text is made only out of numers
        /// </summary>
        /// <param name="text">A string must be passed for checking</param>
        /// <returns>True for numbers, false if it contains other characters</returns>
        private bool CheckStringOnlyNumbers(string text)
        {
            if (text == "") return false;
            //if password contains characters reset password, and send error notification
            if (text.All(char.IsDigit)) return true;
            return false;
        }


        private bool SQLSEARCH()
        {
            Console.WriteLine("Searching for Students...");
            try
            {
                using (SqlConnection con = new SqlConnection(new BOOKS().SQL_ConnectionString()))
                {
                    con.Open();
                    String sql = "Select * from Students where CNP=@cnp";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@cnp", TextBoxCNP.Text);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if(reader.Read())
                            {
                                TextBoxNume.Text = reader["Nume"] as string;
                                TextBoxPreNume.Text = reader["Prenume"] as string;
                                TextBoxProfilePic.Text = reader["Profilepic"] as string;
                                ComboBoxAn.SelectedItem = reader.GetInt32(reader.GetOrdinal("An_absolvire"));
                                ComboBoxProfession.SelectedIndex = 0;
                                ResetAnABS();
                                Console.WriteLine("Student found");
                                oldProfession = "Student";
                                return true;
                            }
                            else
                            {
                                return SQLSEARCHLIBRARIAN();
                            }
                        }
                    }
                }
            }
            catch(SqlException e)
            {
                MessageBox.Show(e.ToString());
            }


            return false;
        }
        private bool SQLSEARCHLIBRARIAN()
        {
            Console.WriteLine("Student not found.... atempting finding Librarian");
            try
            {
                using (SqlConnection con = new SqlConnection(new BOOKS().SQL_ConnectionString()))
                {
                    con.Open();
                    String sql = "Select * from Librarians where CNP=@cnp";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@cnp", TextBoxCNP.Text);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TextBoxNume.Text = reader["Nume"] as string;
                                TextBoxPreNume.Text = reader["Prenume"] as string;
                                TextBoxProfilePic.Text = reader["Profilepic"] as string;
                                ComboBoxProfession.SelectedIndex = 1;
                                ComboBoxAn.Height = 0;
                                ComboBoxAn.Margin = new Thickness(0);
                                Console.WriteLine("Librarian found");
                                oldProfession = "Librarian";
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("Entry not found... it's a new user..");
                                oldProfession = "";
                                return false;
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                MessageBox.Show(e.ToString());
            }

            return false;
        }

        private void ResetAnABS()
        {
            ComboBoxAn.Height = 40.5f;
            ComboBoxAn.Margin = new Thickness(0, 10, 0, 0);
        }


        private void ResetFields()
        {
            TextBoxCNP.IsEnabled = true;
            TextBoxCNP.Text = "";
            TextBoxNume.Text = "";
            TextBoxPreNume.Text = "";
            TextBoxProfilePic.Text = "";
            ComboBoxAn.SelectedIndex = -1;
            ComboBoxProfession.SelectedIndex = -1;

        }

        private void ComboBoxProfession_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //retains what it was when the event fired, not the new value
            if(ComboBoxProfession.SelectionBoxItem.ToString()!="Student")
            {
                ResetAnABS();
            }
            else
            {
                ComboBoxAn.Height = 0;
                ComboBoxAn.Margin = new Thickness(0);
            }
        }

        private void TextBoxCNP_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxCNP.Text.Length == 13)
            {
                if (!CheckStringOnlyNumbers(TextBoxCNP.Text))
                {
                    TextBoxCNP.Text = "";
                    var message = "CNP must contain only numbers!";
                    SnackBarDisplay(message, 1000);
                    e.Handled = true;
                }
                else
                {
                    if (TextBoxCNP.Text.Length < 13)
                    {
                        Console.WriteLine("Attempted to querry a invalid CNP...Aborting");
                        var message = "CNP should be no more or less than 13 digits. Are you sure the CNP is right?";
                        SnackBarDisplay(message, 1000);
                        e.Handled = true;
                        return;
                    }
                    if (SQLSEARCH())
                    {
                        ButtonSave.Content = "Modify";
                        TextBoxCNP.IsEnabled = false;
                    }
                    else
                    {
                        ButtonSave.Content = "ADD";
                    }
                }
            }
        }
    }


    
}
