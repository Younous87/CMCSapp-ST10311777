using System.Data;
using System.Data.SqlClient;

namespace CMCSapp_ST10311777.Models
{
	public class DocumentTable
	{
		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Define a static connection string for the SQL database
		internal static string con_string =
			"Server=tcp:cloudev-sql-server.database.windows.net,1433;Initial Catalog=CLOUD-db;Persist Security Info=False;User ID=admin-youyou;Password=C'esttropcool87;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";

		// Define a static SqlConnection object
		public static SqlConnection con = new SqlConnection(con_string);

		// Define properties for the productsTable class
		public int DocumentID { get; set; }
		public string DocumentURL { get; set; }
		public string DocumentName { get; set; }
		public int ClaimID { get; set; }
		public DateTime dateTime { get; set; }

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Method to insert a new document into the database
		public int NewDocument(DocumentTable d)
		{
			try
			{
				// SQL query to insert a new document record into the documentTable
				string sql = "INSERT INTO documentTable (documentURL, claimID, uploadDate, documentName) VALUES (@DocumentURL, @ClaimID, @DateTime, @DocumentName)";
				SqlCommand cmd = new SqlCommand(sql, con);

				// Add parameters to the SqlCommand to prevent SQL injection
				cmd.Parameters.AddWithValue("@DocumentURL", d.DocumentURL); 
				cmd.Parameters.AddWithValue("@ClaimID", d.ClaimID); 
				cmd.Parameters.AddWithValue("@DateTime", d.dateTime); 
				cmd.Parameters.AddWithValue("@DocumentName", d.DocumentName); 

				// Open the SqlConnection to the database
				con.Open();

				// Execute the SqlCommand and return the number of affected rows
				int rowsAffected = cmd.ExecuteNonQuery();
				return rowsAffected; // Return the count of rows affected by the insert operation
			}
			catch (SqlException sqlEx)
			{
				// Log and rethrow database-related errors
				throw new ApplicationException("Database error occurred while uploading the document: " + sqlEx.Message);
			}
			catch (Exception ex)
			{
				// Log and rethrow general exceptions
				throw new ApplicationException("An error occurred while uploading the document: " + ex.Message);
			}
			finally
			{
				// Ensure the database connection is closed properly
				if (con != null && con.State == ConnectionState.Open)
				{
					con.Close(); // Close the connection if it is open
				}
			}
		}

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Method to get all documents from the database
		public List<DocumentTable> GetAllDocuments()
		{
			// Initialize a list to store retrieved documents
			List<DocumentTable> documents = new List<DocumentTable>();

			try
			{
				// Create a new SqlConnection using the provided connection string
				using (SqlConnection con = new SqlConnection(con_string))
				{
					// SQL query to select all documents ordered by claimID
					string sql = "SELECT * FROM documentTable ORDER BY claimID";
					SqlCommand cmd = new SqlCommand(sql, con);

					// Open the SqlConnection to the database
					con.Open();
					// Execute the command and obtain a SqlDataReader for processing the results
					SqlDataReader rdr = cmd.ExecuteReader();
					// Read each record from the SqlDataReader
					while (rdr.Read())
					{
						// Create a new DocumentTable object and populate its properties from the data reader
						DocumentTable document = new DocumentTable
						{
							ClaimID = (int)rdr["claimID"], 
							dateTime = (DateTime)rdr["uploadDate"], 
							DocumentName = (string)rdr["documentName"] 
						};

						// Add the populated document object to the list
						documents.Add(document);
					}
				}
			}
			catch (SqlException sqlEx)
			{
				// Log and rethrow database-related errors
				throw new ApplicationException("Database error occurred while retrieving documents: " + sqlEx.Message);
			}
			catch (Exception ex)
			{
				// Log and rethrow general exceptions
				throw new ApplicationException("An error occurred while retrieving documents: " + ex.Message);
			}

			// Return the list of retrieved documents
			return documents;
		}

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//
	}
}//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°...ooo000 END OF FILE 000ooo...°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//
