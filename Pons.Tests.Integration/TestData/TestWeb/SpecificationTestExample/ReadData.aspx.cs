using System.Data.SqlClient;
using System.Web.UI;

namespace WebGUIApplication1.SpecificationTestExample
{
    public partial class ReadData : Page
    {
        public string PersistentValue { get; set; }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            var connectionString = Request["connString"];
            var id = Request["id"];

            System.Diagnostics.Trace.WriteLine("fetching " + id + " from " + connectionString);

            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            using(conn)
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT B FROM TestTable WHERE A=@id";
                SqlParameter arg = cmd.CreateParameter();
                arg.ParameterName = "id";
                arg.Value = id;
                cmd.Parameters.Add(arg);
                string testData = (string) cmd.ExecuteScalar();
                readData.Value = testData;
            }
        }
    }
}
