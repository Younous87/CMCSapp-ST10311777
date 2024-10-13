using System.Data.SqlClient;
using System.Security.Claims;

namespace CMCSapp_ST10311777.Models
{
    public class ClaimTable
    {
        // Define a static connection string for the SQL database
        internal static string con_string =
            "Server=tcp:cloudev-sql-server.database.windows.net,1433;Initial Catalog=CLOUD-db;Persist Security Info=False;User ID=admin-youyou;Password=C'esttropcool87;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";

        // Define a static SqlConnection object
        public static SqlConnection con = new SqlConnection(con_string);

        // Define properties for the productsTable class
        public int ClaimID { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string status { get; set; }

        // Method to insert a product into the database
        public int CreateClaim(ClaimTable p)
        {
            try
            {
                // Define the SQL query to insert a new product
                string sql = "INSERT INTO claimTable (hoursWorked, hourlyRate, totalAmount, status) VALUES (@HoursWorked, @HourlyRate, @TotalAmount, @status)";
                SqlCommand cmd = new SqlCommand(sql, con);

                // Add parameters to the SqlCommand
                cmd.Parameters.AddWithValue("@HoursWorked", p.HoursWorked);
                cmd.Parameters.AddWithValue("@HourlyRate", p.HourlyRate);
                cmd.Parameters.AddWithValue("@TotalAmount", p.TotalAmount);
                cmd.Parameters.AddWithValue("@status", p.status);

                // Open the SqlConnection
                con.Open();

                // Execute the SqlCommand to insert the product
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the SqlConnection
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public List<ClaimTable> GetAllClaims()
        {
            // Initialize a list to store products
            List<ClaimTable> claims = new List<ClaimTable>();

            // Create a new instance of SqlConnection using the connection string
            using (SqlConnection con = new SqlConnection(con_string))
            {
                // Define the SQL query to select all products
                string sql = "SELECT * FROM claimTable";
                SqlCommand cmd = new SqlCommand(sql, con);

                // Open the SqlConnection
                con.Open();

                // Execute the SqlCommand and read the results
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    // Create a new productsTable object for each row
                    ClaimTable claim = new ClaimTable();
                    claim.ClaimID = (int)rdr["claimID"];
                    claim.HoursWorked = (decimal)rdr["hoursWorked"];
                    claim.HourlyRate = (decimal)rdr["hourlyRate"];
                    claim.TotalAmount = (decimal)rdr["totalAmount"];
                    claim.status = rdr["status"].ToString();

                    // Add the product to the list
                    claims.Add(claim);
                }
            }

            return claims;
        }

        // Static method to update a product's IsActive status in the database
        public void UpdateStatus(int claimID, string status)
        {
            // Create a new instance of SqlConnection using the connection string
            using (SqlConnection con = new SqlConnection(con_string))
            {
                // Define the SQL query to update the product's IsActive status
                string sql = "UPDATE claimTable SET status = @status WHERE claimID = @claimID";
                SqlCommand cmd = new SqlCommand(sql, con);

                // Add parameters to the SqlCommand
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@claimID", claimID);

                // Open the SqlConnection
                con.Open();

                // Execute the SqlCommand to update the product
                cmd.ExecuteNonQuery();
            }
        }

    }
}
