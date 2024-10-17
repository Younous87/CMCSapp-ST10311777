using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;

namespace CMCSapp_ST10311777.Models
{
    public class ClaimTable
    {
		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

        // Define a static connection string for the SQL database
        internal static string con_string =
            "Server=tcp:cloudev-sql-server.database.windows.net,1433;Initial Catalog=CLOUD-db;Persist Security Info=False;User ID=admin-youyou;Password=C'esttropcool87;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";

        // Define a static SqlConnection object
        public SqlConnection con = new SqlConnection(con_string);

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		public ClaimTable()
        {
        }

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Define properties for the productsTable class
		public int ClaimID { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string status { get; set; }
        public DateTime dateTime { get; set; }

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Method to insert a claim into the database
		public int CreateClaim(ClaimTable p)
        {

            try
            {
                string sql = "INSERT INTO claimTable (hoursWorked, hourlyRate, totalAmount, status, claimDate) VALUES (@HoursWorked, @HourlyRate, @TotalAmount, @status, @DateTime)";
                SqlCommand cmd = new SqlCommand(sql, con);

                // Add parameters to the SqlCommand
                cmd.Parameters.AddWithValue("@HoursWorked", p.HoursWorked);
                cmd.Parameters.AddWithValue("@HourlyRate", p.HourlyRate);
                cmd.Parameters.AddWithValue("@TotalAmount", p.TotalAmount);
                cmd.Parameters.AddWithValue("@status", p.status);
                cmd.Parameters.AddWithValue("@DateTime", p.dateTime);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected;
            }
            catch (SqlException sqlEx)
            {
                throw new ApplicationException("Database error occurred while creating the claim: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
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

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Method to get all claims from the database
		public List<ClaimTable> GetAllClaims()
        {
            List<ClaimTable> claims = new List<ClaimTable>();

			try
            {
                using (SqlConnection con = new SqlConnection(con_string))
                //using (SqlConnection con = GetConnection())
                {
                    string sql = "SELECT * FROM claimTable";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ClaimTable claim = new ClaimTable
                        {
                            ClaimID = (int)rdr["claimID"],
                            HoursWorked = (decimal)rdr["hoursWorked"],
                            HourlyRate = (decimal)rdr["hourlyRate"],
                            TotalAmount = (decimal)rdr["totalAmount"],
                            dateTime = (DateTime)rdr["claimDate"],
                            status = rdr["status"].ToString()
                        };

                        claims.Add(claim);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new ApplicationException("Database error occurred while retrieving claims: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving claims: " + ex.Message);
            }

            return claims;
        }

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Method to update claim status
		public void UpdateStatus(int claimID, string status)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(con_string))
                //using (SqlConnection con = GetConnection()) 
                {
                    string sql = "UPDATE claimTable SET status = @status WHERE claimID = @claimID";
                    SqlCommand cmd = new SqlCommand(sql, con);

                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@claimID", claimID);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlEx)
            {
                throw new ApplicationException("Database error occurred while updating the claim status: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the claim status: " + ex.Message);
            }
        }

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		public ClaimTable GetClaimById(int claimID)
        {
	        ClaimTable claim = null;
            using (SqlConnection con = new SqlConnection(con_string))
            //using (SqlConnection con = GetConnection())
            {
		        string sql = "SELECT * FROM claimTable WHERE claimID = @ClaimID";
		        SqlCommand cmd = new SqlCommand(sql, con);
		        cmd.Parameters.AddWithValue("@ClaimID", claimID);

		        con.Open();
		        SqlDataReader rdr = cmd.ExecuteReader();

		        if (rdr.Read())
		        {
			        // If a record is found, create a ClaimTable object
			        claim = new ClaimTable();
			        claim.ClaimID = (int)rdr["claimID"];
			        claim.HoursWorked = (decimal)rdr["hoursWorked"];
			        claim.HourlyRate = (decimal)rdr["hourlyRate"];
			        claim.TotalAmount = (decimal)rdr["totalAmount"];
			        claim.dateTime = (DateTime)rdr["claimDate"];
			        claim.status = rdr["status"].ToString();
		        }
	        }

	        return claim; // Return null if no claim is found
        }

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//
	}
}//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°...ooo000 END OF FILE 000ooo...°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//
