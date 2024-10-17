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
                string sql = "INSERT INTO documentTable (documentURL, claimID, uploadDate, documentName) VALUES (@DocumentURL, @ClaimID, @DateTime, @DocumentName)";
                SqlCommand cmd = new SqlCommand(sql, con);

                // Add parameters to the SqlCommand
                cmd.Parameters.AddWithValue("@DocumentURL", d.DocumentURL);
                cmd.Parameters.AddWithValue("@ClaimID", d.ClaimID);
                cmd.Parameters.AddWithValue("@DateTime", d.dateTime);
                cmd.Parameters.AddWithValue("@DocumentName", d.DocumentName);

                // Open the SqlConnection
                con.Open();

                // Execute the SqlCommand
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected;
            }
            catch (SqlException sqlEx)
            {
                // Log the error
                throw new ApplicationException("Database error occurred while uploading the document: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                // Log general exceptions
                throw new ApplicationException("An error occurred while uploading the document: " + ex.Message);
            }
            finally
            {
                // Ensure the connection is closed properly
                if (con != null && con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Method to get all documents from the database
		public List<DocumentTable> GetAllDocuments()
        {
            List<DocumentTable> documents = new List<DocumentTable>();

            try
            {
                using (SqlConnection con = new SqlConnection(con_string))
                {
                    string sql = "SELECT * FROM documentTable ORDER BY claimID";
                    SqlCommand cmd = new SqlCommand(sql, con);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        DocumentTable document = new DocumentTable
                        {
                            ClaimID = (int)rdr["claimID"],
                            dateTime = (DateTime)rdr["uploadDate"],
                            DocumentName = (string)rdr["documentName"]
                        };

                        documents.Add(document);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new ApplicationException("Database error occurred while retrieving documents: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving documents: " + ex.Message);
            }

            return documents;
        }

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//
	}
}//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°...ooo000 END OF FILE 000ooo...°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//
