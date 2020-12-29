using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Security;


// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GTPRESTService" in code, svc and config file together.
public class GTPRESTService : IGTPRESTService
{
    private const int ActionInsert = 0;
    private const int ActionUpdate = 1;
    private string connectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["REMIS"]);
    private string UserDBconnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ApplicationServices"]);

    public _FTPTransaction GetTransaction(int id_server_transaction)
    {
        _FTPTransaction output = new _FTPTransaction();
        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM tblFTPTransaction WHERE id_server_transaction=" + id_server_transaction.ToString();
        SqlDataReader dr;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            while (dr.Read())
            {
                output.id_server_transaction = id_server_transaction;
                output.date_started = Convert.ToDateTime(dr["date_started"]);
                try
                {
                    output.date_end = Convert.ToDateTime(dr["date_started"]);
                }
                catch
                {
                    output.date_end = null;
                }
                output.isCompleted = Convert.ToBoolean(dr["isCompleted"]);
                output.number_of_run = Convert.ToInt32(dr["number_of_run"]);
                output.number_off_app_fail = Convert.ToInt32(dr["number_of_app_fail"]);
                output.total_file_uploaded = Convert.ToInt32(dr["total_file_uploaded"]);
                output.transaction_status = Convert.ToInt32(dr["transaction_status"]);
                output.market_name = Convert.ToString(dr["market_name"]);
                output.Campaign = Convert.ToString(dr["Campaign"]);
                output.Project = Convert.ToString(dr["Project"]);

            }
            return output;
        }
        else
            return null;
    }



    public List<string> GetProject()
    {
        List<string> output = new List<string>();
        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;

        cmd.CommandText = "SELECT * FROM tblClientSettings WHERE isActive =1 ";
        SqlDataReader dr;

        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            output.Add(Convert.ToString(dr["ClientName"]));
        }
        con.Close();
        return output;
    }


    public string tblFTPTrasaction(Guid TransactionGID,  
                           string username,                     
                           string ip_address,
                           string host_name,
                           string market_name,              
                           string campaign,
                           string Project,
                           double totalinputbytes)

    {

        string CommandText = "";

        try
        {            
            if (username == null)
                username = "";
            if (ip_address == null)
                ip_address = "";
            if (host_name == null)
                host_name = "";
            if (market_name == null)
                market_name = "";
            if (campaign == null)
                campaign = "";
            if (Project == null)
                Project = "";
            if (totalinputbytes == null)
                totalinputbytes = -1;

            int id_server_transaction = -1;

            string connectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["REMIS"]);
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
          
            cmd.CommandText = "INSERT INTO tblFTPTransaction(TransactionGID,id_client_transaction,transaction_type,username,isCompleted,number_of_run,number_of_app_fail,"
            + " ftp_url,ip_address,host_name,market_name,transaction_status,total_file_uploaded,campaign,Project,totalinputbytes) VALUES "
            + " ('" + TransactionGID + "',-1,0,'" + username + "',0,1,0,'"
            + "ftp://upload.mygws.com" + "','" + ip_address + "','" + host_name + "','" + market_name + "',0,0,'" + campaign + "','" + Project.Replace("'", "''") + "'," + totalinputbytes + " ); SELECT @@IDENTITY ";


            CommandText = cmd.CommandText;
            object tmpInt = cmd.ExecuteScalar();
            if (tmpInt != null)
            {
                id_server_transaction = Convert.ToInt32(tmpInt);
            }
            else
                id_server_transaction = -1;

            //Update market name
            cmd.CommandText = "UPDATE tblFTPTransaction SET FTPFolder='" + id_server_transaction.ToString() + "-" + market_name + "' WHERE id_server_transaction =" + id_server_transaction;
            cmd.ExecuteNonQuery();

            con.Close();
            return id_server_transaction.ToString();
        }
        catch (Exception ex)
        {
            string tmpStr = ex.ToString();

            return  CommandText + " Error Log : " + ex.ToString() + "Username:" + username.ToString() + "IP:" + ip_address.ToString() + "Host:" + host_name.ToString() + "Market:" + market_name.ToString() + " Campaign:" + campaign.ToString() + "Project:" + Project.ToString() + "Total" + totalinputbytes.ToString();
        }
        
    }



    private int GetMarketID(string market_name, string Project)
    {
        int id_market = 0;

        try
        {

            int status_market = 0;

            if (Project == "AT&T BM")
            {
                status_market = 2;
            }
            else if (Project == "AT&T E911")
            {
                status_market = 3;
            }
            else if (Project == "AT&T MRAB")
            {
                status_market = 7;
            }
            else if (Project == "BELL BM")
            {
                status_market = 4;
            }
            else if (Project == "TIGO BM")
            {
                status_market = 5;
            }
            else if (Project == "UAE BM")
            {
                status_market = 6;
            }
            else if (Project == "Netherlands")
            {
                status_market = 8;
            }

            market_name = market_name.Substring(0, market_name.LastIndexOf(" ")) + ", " + market_name.Substring(market_name.LastIndexOf(" ") + 1, market_name.Length - 1 - market_name.LastIndexOf(" "));
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            cmd.CommandText = "SELECT * FROM tblMarket where name_market='" + market_name + "' AND market_status=" + status_market.ToString();
            SqlDataReader dr;

            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                id_market = Convert.ToInt32(dr["id_market"]);

            }
            cmd.Dispose();
            con.Close();

        }
        catch
        {

        }


        return id_market;
    }



    public string tblFTPFile(
            Guid TransactionGID,
            string file_name,
            int bytestrnsfrd)
    {
        //2011-10-15-20-00-53-0003-0025-0002-0003-A
        String CommandText = "";

        try
        {
            DateTime? Date_Collected = null;
            string file_name_only = "";
            file_name_only = file_name.Substring(file_name.LastIndexOf(@"\") + 1, file_name.Length - file_name.LastIndexOf(@"\") - 1);

            if (file_name_only.Length > 10)
            {
                string datepart = file_name_only.Substring(0, 10);
                DateTime tmp;
                if (DateTime.TryParse(datepart, out tmp))
                {
                    Date_Collected = DateTime.Parse(datepart);
                }
            }

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT id_server_transaction FROM tblFTPTransaction WHERE TransactionGID ='" + TransactionGID.ToString() + "'" ;
            cmd.Connection = con;
            SqlDataReader dr;
            int id_server_transaction =-1;

            dr = cmd.ExecuteReader();
            if (dr.HasRows)
                while (dr.Read())
                {
                    id_server_transaction = Convert.ToInt32(dr["id_server_transaction"]);
                }


            dr.Close();
            cmd.Dispose();

            cmd = new SqlCommand();            
            cmd.Connection = con;
            if (Date_Collected == null)
                cmd.CommandText = "INSERT INTO tblFTPFile(id_server_transaction,id_client_transaction,file_name,is_finished,file_status,file_name_only,bytestrnsfrd,TransactionGID) VALUES (" + id_server_transaction + ",-1,'" + file_name.Replace("'", "''") + "',0,0,'" + file_name_only.Replace("'", "''") + "'," + bytestrnsfrd + ",'" + TransactionGID.ToString() +"')";
            else
                cmd.CommandText = "INSERT INTO tblFTPFile(id_server_transaction,id_client_transaction,file_name,is_finished,file_status,file_name_only,Date_Collected,bytestrnsfrd,TransactionGID) VALUES (" + id_server_transaction + ",-1,'" + file_name.Replace("'", "''") + "',0,0,'" + file_name_only.Replace("'", "''") + "','" + Convert.ToString(Date_Collected) + "'," + bytestrnsfrd + ",'" + TransactionGID.ToString() + "')";

            CommandText = cmd.CommandText;

            cmd.ExecuteNonQuery();
            con.Close();
            return "OK";
        }
        catch (Exception ex)
        {
            return  ex.ToString();
        }
    }


    public string UpdatetbFTPFile(Guid TransactionGID, string FileName, int status)
    {
        try
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            if (status == 1)
            {


                cmd.CommandText = "UPDATE tblFTPFile SET date_finished=getdate(), is_finished=1, file_status =" + status + " WHERE TransactionGID='" + TransactionGID + "' AND file_name ='" + FileName + "'";
            }
            else
            {
                cmd.CommandText = "UPDATE tblFTPFile SET file_status =" + status + " WHERE TransactionGID='" + TransactionGID + "' AND file_name ='" + FileName + "'";
            }
            cmd.ExecuteNonQuery();

            if (status == 1)
            {
                //Check if all file has been uploaded to server and update transaction
                int count_pending = 0;
                cmd.CommandText = "SELECT * FROM tblFTPFile WHERE file_status <= 0 AND TransactionGID=" + TransactionGID;
                count_pending = Convert.ToInt32(cmd.ExecuteScalar());
                if (count_pending == 0)
                {
                    //Update transaction status as completed
                    cmd.CommandText = "UPDATE tblFTPTransaction SET date_end = getdate(), isCompleted = 1, transaction_status=1 WHERE TransactionGID = '" + TransactionGID + "'";
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                //Update total_file_uploaded

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT count(*) FROM tblFTPFile WHERE (file_status =1 OR file_status>=3) AND TransactionGID = '" + TransactionGID + "'";
                int total_uploaded = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.Dispose();
                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE tblFTPTransaction SET total_file_uploaded = " + total_uploaded + " WHERE TransactionGID = '" + TransactionGID + "'";
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }



            if ((status == 1) || (status >= 3))
            {
                //Update total upload file size
                double totalbytesuploaded = 0;
                SqlDataReader dr;
                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT * FROM tblFTPFile where (file_status =1 Or file_status>=3) AND (iTransactionGID = '" + TransactionGID + "')";

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    try
                    {
                        totalbytesuploaded += Convert.ToDouble(dr["bytestrnsfrd"]);
                    }
                    catch
                    {

                    }
                }

                dr.Close();
                dr.Dispose();
                cmd.Dispose();

                cmd.CommandText = "UPDATE tblFTPTransaction SET totalbytesuploaded = " + totalbytesuploaded.ToString() + " WHERE TransactionGID = '" + TransactionGID + "'";
                cmd.ExecuteNonQuery();
                cmd.Dispose();

            }
            con.Close();
            return "OK";
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }

        
        
    }

    public string UpdatetbFTPFileUploadTime(Guid TransactionGID, string FileName, int uploadtime)
    {
        try
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "UPDATE tblFTPFile SET uploadtime =" + uploadtime + " WHERE TransactionGID='" + TransactionGID + "' AND file_name ='" + FileName + "'";
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
        return "OK";
    }


    public List<String> GetMarket(int status)
    {
        List<String> output = new List<String>();
        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM tblMarket WHERE market_status=" + status.ToString() + " ORDER BY name_market ";

        SqlDataReader dr;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            while (dr.Read())
            {                
                output.Add(Convert.ToString(dr["name_market"]));
            }
        }
        con.Close();
        return output;
    }


    public string completeTransaction(Guid TransactionGID, string Status)
    {
        try
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            if (Status=="1")
                cmd.CommandText = "UPDATE tblFTPTransaction SET date_end = getdate(), isCompleted = 1, transaction_status=1 WHERE TransactionGID = '" + TransactionGID + "'";
            else
                cmd.CommandText = "UPDATE tblFTPTransaction SET  transaction_status=" + Status + " WHERE TransactionGID = '" + TransactionGID + "'";


            cmd.ExecuteNonQuery();
            cmd.Dispose();

            ////Update total upload file size

            
            cmd = new SqlCommand();
            cmd.Connection = con;
            double totalbytesuploaded = 0;
            SqlDataReader dr;
            cmd.CommandText = "SELECT * FROM tblFTPFile where (file_status =1 Or file_status>=3) AND (TransactionGID = '" + TransactionGID + "')";

            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                try
                {
                    totalbytesuploaded += Convert.ToDouble(dr["bytestrnsfrd"]);
                }
                catch
                {

                }
            }

            dr.Close();
            dr.Dispose();
            cmd.Dispose();
            cmd = new SqlCommand();
            cmd.Connection = con;

            cmd.CommandText = "UPDATE tblFTPTransaction SET totalbytesuploaded = " + totalbytesuploaded.ToString() + " WHERE TransactionGID = '" + TransactionGID + "'";
            cmd.ExecuteNonQuery();
            cmd.Dispose();


            con.Close();
            return "OK";
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }

    }







}
