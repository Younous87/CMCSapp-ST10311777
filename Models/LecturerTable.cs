using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace CMCSapp_ST10311777.Models
{
    public class LecturerTable
    {
        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

        // Define a static connection string for the SQL database
        internal static string con_string =
            "Server=tcp:cloudev-sql-server.database.windows.net,1433;Initial Catalog=CLOUD-db;Persist Security Info=False;User ID=admin-youyou;Password=C'esttropcool87;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";

        // Define a static SqlConnection object
        public SqlConnection con = new SqlConnection(con_string);

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

        //Default constructor
        public LecturerTable()
        {
        }

        public int LecturerID { get; set; }
        public string LecturerFirstName { get; set; }
        public string LecturerLastName { get; set; }
        public string LecturerEmail { get; set; }
        public string LecturerPhone { get; set; }

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

        // Method to insert a claim into the database
        public int NewLecturer(LecturerTable p)
        {
            try
            {
                // SQL query to insert a new claim into the claimTable
                string sql = "INSERT INTO lecturerTable (lectFirstName, lectLastName, lectEmail, lectNumber) VALUES (@lectFirstName, @lectLastName, @lectEmail, @lectNumber)";
                SqlCommand cmd = new SqlCommand(sql, con);

                // Add parameters to the SqlCommand to prevent SQL injection
                cmd.Parameters.AddWithValue("@lectFirstName", p.LecturerFirstName);
                cmd.Parameters.AddWithValue("@lectLastName", p.LecturerLastName);
                cmd.Parameters.AddWithValue("@lectEmail", p.LecturerEmail);
                cmd.Parameters.AddWithValue("@lectNumber", p.LecturerPhone);

                // Open the connection and execute the insert command
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery(); // Returns the number of rows affected
                return rowsAffected; // Return the number of rows inserted
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL exceptions
                throw new ApplicationException("Database error occurred while creating the claim: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                throw new ApplicationException("An error occurred while creating the claim: " + ex.Message);
            }
            finally
            {
                // Ensure the connection is closed even if an exception occurs
                if (con != null && con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        ////°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

        // Method to get all claims from the database
        public List<LecturerTable> GetAllLecturers()
        {
            // List to hold all claims
            List<LecturerTable> claims = new List<LecturerTable>();

            try
            {
                using (SqlConnection con = new SqlConnection(con_string))
                {
                    // SQL query to select all claims from the claimTable
                    string sql = "SELECT * FROM lecturerTable";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    con.Open(); // Open the connection

                    SqlDataReader rdr = cmd.ExecuteReader(); // Execute the command and read the data
                    while (rdr.Read()) // Iterate through the results
                    {
                        // Create a ClaimTable object for each record found
                        LecturerTable claim = new LecturerTable()
                        {
                            LecturerID = (int)rdr["lectID"],
                            LecturerFirstName = (string)rdr["lectFirstName"],
                            LecturerLastName = (string)rdr["lectLastName"],
                            LecturerEmail = (string)rdr["lectEmail"],
                            LecturerPhone = (string)rdr["lectNumber"]
                        };

                        claims.Add(claim); // Add the claim to the list
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL exceptions, indicating a database-related issue
                throw new ApplicationException("Database error occurred while retrieving claims: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                throw new ApplicationException("An error occurred while retrieving claims: " + ex.Message);
            }

            return claims; // Return the list of claims
        }

    }
}
