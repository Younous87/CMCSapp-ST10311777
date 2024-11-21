using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
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

		//Default constructor
		public ClaimTable()
		{
		}

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Define properties for the productsTable class
		public int claimID { get; set; }
        public decimal hoursWorked { get; set; }
        public decimal hourlyRate { get; set; }
        public decimal totalAmount { get; set; }
        public string status { get; set; }
        public DateTime claimDate { get; set; }

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Method to insert a claim into the database
		public int CreateClaim(ClaimTable p)
		{
			try
			{
				// SQL query to insert a new claim into the claimTable
				string sql = "INSERT INTO claimTable (hoursWorked, hourlyRate, totalAmount, status, claimDate) VALUES (@hoursWorked, @hourlyRate, @totalAmount, @status, @DateTime)";
				SqlCommand cmd = new SqlCommand(sql, con);

				// Add parameters to the SqlCommand to prevent SQL injection
				cmd.Parameters.AddWithValue("@hoursWorked", p.hoursWorked);
				cmd.Parameters.AddWithValue("@hourlyRate", p.hourlyRate);
				cmd.Parameters.AddWithValue("@totalAmount", p.totalAmount);
				cmd.Parameters.AddWithValue("@status", p.status);
				cmd.Parameters.AddWithValue("@DateTime", p.claimDate);

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
		public List<ClaimTable> GetAllClaims()
		{
			// List to hold all claims
			List<ClaimTable> claims = new List<ClaimTable>();

			try
			{
				using (SqlConnection con = this.con)
				{
					// SQL query to select all claims from the claimTable
					string sql = "SELECT * FROM claimTable";
					SqlCommand cmd = new SqlCommand(sql, con);
					con.Open(); // Open the connection

					SqlDataReader rdr = cmd.ExecuteReader(); // Execute the command and read the data
					while (rdr.Read()) // Iterate through the results
					{
						// Create a ClaimTable object for each record found
						ClaimTable claim = new ClaimTable
						{
							claimID = (int)rdr["claimID"],
							hoursWorked = (decimal)rdr["hoursWorked"],
							hourlyRate = (decimal)rdr["hourlyRate"],
							totalAmount = (decimal)rdr["totalAmount"],
							claimDate = (DateTime)rdr["claimDate"],
							status = rdr["status"].ToString()
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

		// Method to update claim status
		public void UpdateStatus(int claimID, string status)
		{
			try
			{
				using (SqlConnection con = this.con)
				{
					// SQL query to update the status of a specific claim identified by claimID
					string sql = "UPDATE claimTable SET status = @status WHERE claimID = @claimID";
					SqlCommand cmd = new SqlCommand(sql, con);

					// Add parameters to the SqlCommand
					cmd.Parameters.AddWithValue("@status", status);
					cmd.Parameters.AddWithValue("@claimID", claimID);

					con.Open(); // Open the connection
					cmd.ExecuteNonQuery(); // Execute the update command
				}
			}
			catch (SqlException sqlEx)
			{
				// Handle SQL exceptions
				throw new ApplicationException("Database error occurred while updating the claim status: " + sqlEx.Message);
			}
			catch (Exception ex)
			{
				// Handle any other exceptions
				throw new ApplicationException("An error occurred while updating the claim status: " + ex.Message);
			}
		}

		////°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Method to get a claim by its ID
		public ClaimTable GetClaimById(int claimID)
		{
			ClaimTable claim = null; // Initialize claim to null
			using (SqlConnection con = this.con) // Using statement for automatic resource management
			{
				// SQL query to select a claim by its claimID
				string sql = "SELECT * FROM claimTable WHERE claimID = @claimID";
				SqlCommand cmd = new SqlCommand(sql, con);
				cmd.Parameters.AddWithValue("@claimID", claimID);

				con.Open(); // Open the connection
				SqlDataReader rdr = cmd.ExecuteReader(); // Execute the query and read the data

				if (rdr.Read()) // Check if a record is found
				{
					// If a record is found, create a ClaimTable object
					claim = new ClaimTable();
					claim.claimID = (int)rdr["claimID"];
					claim.hoursWorked = (decimal)rdr["hoursWorked"];
					claim.hourlyRate = (decimal)rdr["hourlyRate"];
					claim.totalAmount = (decimal)rdr["totalAmount"];
					claim.claimDate = (DateTime)rdr["claimDate"];
					claim.status = rdr["status"].ToString();
				}
			}

			return claim; // Return the claim object or null if not found
		}


		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//
	}
}//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°...ooo000 END OF FILE 000ooo...°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//
