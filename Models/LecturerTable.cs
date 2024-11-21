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

        // Define properties for the LecturerTable class
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
                using (SqlConnection con = new SqlConnection(con_string))
                {
                    // SQL query to insert a new lecturer into the lecturerTable
                    string sql = "INSERT INTO lecturerTable (lectFirstName, lectLastName, lectEmail, lectNumber) VALUES (@lectFirstName, @lectLastName, @lectEmail, @lectNumber)";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
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
                }
            }
            catch (SqlException sqlEx)
            {
                throw new ApplicationException($"Database error occurred while inserting a new lecturer: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while inserting a new lecturer: {ex.Message}");
            }
        }

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

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

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

        // Method to update a specific lecturer's information using their ID
        public int UpdateLecturerById(int lecturerId, string firstName, string lastName, string email, string phone)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(con_string))
                {
                    // Start building the SQL query dynamically
                    string sql = "UPDATE lecturerTable SET ";
                    List<string> updates = new List<string>();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        // Check each parameter and add it to the query only if it is not null or empty
                        if (!string.IsNullOrEmpty(firstName))
                        {
                            updates.Add("lectFirstName = @lectFirstName");
                            cmd.Parameters.AddWithValue("@lectFirstName", firstName);
                        }
                        if (!string.IsNullOrEmpty(lastName))
                        {
                            updates.Add("lectLastName = @lectLastName");
                            cmd.Parameters.AddWithValue("@lectLastName", lastName);
                        }
                        if (!string.IsNullOrEmpty(email))
                        {
                            updates.Add("lectEmail = @lectEmail");
                            cmd.Parameters.AddWithValue("@lectEmail", email);
                        }
                        if (!string.IsNullOrEmpty(phone))
                        {
                            updates.Add("lectNumber = @lectNumber");
                            cmd.Parameters.AddWithValue("@lectNumber", phone);
                        }

                        // If no updates are specified, return without executing the query
                        if (updates.Count == 0)
                        {
                            throw new ApplicationException("No fields to update were provided.");
                        }

                        // Join all update statements and complete the query
                        sql += string.Join(", ", updates) + " WHERE lectID = @lectID";
                        cmd.CommandText = sql;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@lectID", lecturerId);

                        // Open the connection and execute the update command
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery(); // Returns the number of rows affected
                        return rowsAffected; // Return the number of rows updated
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new ApplicationException($"Database error occurred while updating lecturer information: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating lecturer information: {ex.Message}");
            }
        }

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

        // Method to get a lecturer by their ID
        public LecturerTable GetLecturerById(int lecturerId)
        {
            try
            {
                // SQL query to select a lecturer by ID
                string sql = "SELECT lectID, lectFirstName, lectLastName, lectEmail, lectNumber FROM lecturerTable WHERE lectID = @lectID";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    // Add parameter to prevent SQL injection
                    cmd.Parameters.AddWithValue("@lectID", lecturerId);

                    // Open the connection
                    con.Open();

                    // Execute the query and read the result
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Map the result to a LecturerTable object
                            return new LecturerTable
                            {
                                LecturerID = reader.GetInt32(reader.GetOrdinal("lectID")),
                                LecturerFirstName = reader.GetString(reader.GetOrdinal("lectFirstName")),
                                LecturerLastName = reader.GetString(reader.GetOrdinal("lectLastName")),
                                LecturerEmail = reader.GetString(reader.GetOrdinal("lectEmail")),
                                LecturerPhone = reader.GetString(reader.GetOrdinal("lectNumber"))
                            };
                        }
                    }
                }

                // Return null if no lecturer is found
                return null;
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL exceptions
                throw new ApplicationException("Database error occurred while retrieving the lecturer: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                throw new ApplicationException("An error occurred while retrieving the lecturer: " + ex.Message);
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

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

    }
}//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°...ooo000 END OF FILE 000ooo...°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//
