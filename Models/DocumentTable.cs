using System.Data.SqlClient;

namespace CMCSapp_ST10311777.Models
{
	public class DocumentTable
	{
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





		// Method to insert a product into the database
		public int NewDocument(DocumentTable d)
		{
			try
			{
				// Define the SQL query to insert a new product
				string sql = "INSERT INTO documentTable (documentURL, claimID, uploadDate, documentName) VALUES (@DocumentURL, @ClaimID, @DateTime, @DocumentName)";
				SqlCommand cmd = new SqlCommand(sql, con);

				// Add parameters to the SqlCommand
				cmd.Parameters.AddWithValue("@DocumentURL", d.DocumentURL);
				cmd.Parameters.AddWithValue("@ClaimID", d.ClaimID);
				cmd.Parameters.AddWithValue("@DateTime", d.dateTime);
				cmd.Parameters.AddWithValue("@DocumentName", d.DocumentName);

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

		public List<DocumentTable> GetAllDocuments()
		{
			// Initialize a list to store documents
			List<DocumentTable> documents = new List<DocumentTable>();

			// Create a new instance of SqlConnection using the connection string
			using (SqlConnection con = new SqlConnection(con_string))
			{
				// Define the SQL query to select and order all documents by claimID
				string sql = "SELECT * FROM documentTable ORDER BY claimID";

				SqlCommand cmd = new SqlCommand(sql, con);

				// Open the SqlConnection
				con.Open();

				// Execute the SqlCommand and read the results
				SqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{
					// Create a new DocumentTable object for each row
					DocumentTable document = new DocumentTable();
					document.ClaimID = (int)rdr["claimID"];
					document.dateTime = (DateTime)rdr["uploadDate"];
					document.DocumentName = (string)rdr["documentName"];

					// Add the document to the list
					documents.Add(document);
				}
			}

			return documents;
		}

		public List<DocumentTable> GetDocumentsByClaimId(int claimId)
		{
			// Initialize a list to store documents
			List<DocumentTable> documents = new List<DocumentTable>();

			// Create a new instance of SqlConnection using the connection string
			using (SqlConnection con = new SqlConnection(con_string))
			{
				// Define the SQL query to select documents with the specific claimId
				string sql = "SELECT * FROM documentTable WHERE claimID = @ClaimID";
				SqlCommand cmd = new SqlCommand(sql, con);
				cmd.Parameters.AddWithValue("@ClaimID", claimId);

				// Open the SqlConnection
				con.Open();

				// Execute the SqlCommand and read the results
				SqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{
					// Create a new DocumentTable object for each row
					DocumentTable document = new DocumentTable
					{
						ClaimID = (int)rdr["claimID"],
						dateTime = (DateTime)rdr["uploadDate"]
					};

					// Add the document to the list
					documents.Add(document);
				}
			}

			return documents;
		}


	}
}
