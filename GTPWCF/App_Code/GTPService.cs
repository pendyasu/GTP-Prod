using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using REMISModel;
using UserDBModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Web.Security;
using System.IO;


// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GTPService" in code, svc and config file together.
public class GTPService : IGTPService
{

    private const int ActionInsert = 0;
    private const int ActionUpdate = 1;
    private string connectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["REMIS"]);
    private string UserDBconnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ApplicationServices"]);

    public List<_FileGroupConfig> getFileGroupContract()
    {
        List<_FileGroupConfig> output = new List<_FileGroupConfig>();
        SqlConnection con = new SqlConnection(connectionString);
        try
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "select * from tblFileGroupConfig";
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    _FileGroupConfig tmp = new _FileGroupConfig();
                    tmp.file_group_name = Convert.ToString(dr["file_group_name"]);
                    tmp.device_id = Convert.ToString(dr["device_id"]);
                    try
                    {
                        if (tmp.file_group_name != "FileTimestampThresh")
                        {
                            tmp.NumToTriggerMissingAlert = Convert.ToInt32(dr["NumToTriggerMissingAlert"]);
                        }
                        else
                        {
                            tmp.NumToTriggerMissingAlert = -1;
                        }

                    }
                    catch
                    {
                        tmp.NumToTriggerMissingAlert = -1;
                    }

                    output.Add(tmp);
                }
            }

        }
        catch (Exception ex)
        {
            return output;
        }
        finally
        {
            con.Close();
        }

        return output;
    }


    public List<string> GetClientForTeam(string TeamName)
    {
        List<String> clientList = new List<string>();
        SqlConnection con = new SqlConnection(connectionString);
        try
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM tblGTPTeamProjects WHERE isShowOnlyClientProject = 1 AND username='" + TeamName + "'";
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    clientList.Add(Convert.ToString(dr["ClientName"]));
                }
            }

        }
        catch (Exception ex)
        {
            return clientList;
        }
        finally
        {
            con.Close();
        }
        return clientList;

    }

    public List<_TeamProjects> GetTeamProjects()
    {
        List<_TeamProjects> teamProjects = new List<_TeamProjects>();

        SqlConnection con = new SqlConnection(connectionString);
        try
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM tblGTPTeamProjects WHERE isShowOnlyClientProject = 1";
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    _TeamProjects tp = new _TeamProjects();
                    tp.UserName = Convert.ToString(dr["username"]);
                    tp.ClientName = Convert.ToString(dr["ClientName"]);
                    teamProjects.Add(tp);
                }
            }

        }
        catch (Exception ex)
        {
            return teamProjects;
        }
        finally
        {
            con.Close();
        }
        return teamProjects;
    }


    public string ShadowPassword(int action, _ShadowPassword shadowPassword)
    {
        SqlConnection con = new SqlConnection(connectionString);
        try
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            int isActive = 1;
            if (!shadowPassword.isActive)
            {
                isActive = 0;
            }
            if (action == 0)
            {
                cmd.CommandText = "INSERT INTO tblShadowPassword(username, password, type, isActive, date_created) "
                + " VALUES ('" + shadowPassword.username + "','" + shadowPassword.password + "'," + shadowPassword.type + "," + isActive + ",'" + shadowPassword.date_created + "')";
            }
            else
                cmd.CommandText = "UPDATE tblShadowPassword SET username = '" + shadowPassword.username + "', password ='"
                    + shadowPassword.password + "', type = " + shadowPassword.type + ", isActive =" + isActive;

            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
        finally
        {
            con.Close();
        }
        return "1";
    }

    public List<_ShadowPassword> getShadowPassword(int? status)
    {
        List<_ShadowPassword> passwords = new List<_ShadowPassword>();
        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM tblShadowPassword WHERE isActive = " + status.ToString();
        SqlDataReader dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            _ShadowPassword shadowPassword = new _ShadowPassword();
            shadowPassword.isActive = Convert.ToBoolean(status);
            shadowPassword.username = Convert.ToString(dr["username"]);
            shadowPassword.password = Convert.ToString(dr["password"]);
            shadowPassword.type = Convert.ToInt32(dr["type"]);
            passwords.Add(shadowPassword);
        }
        con.Close();
        return passwords;
    }

    public List<_Project> GetProject()
    {
        List<_Project> output = new List<_Project>();
        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;

        cmd.CommandText = "SELECT * FROM tblProject WHERE project_status =1 ";
        SqlDataReader dr;

        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            _Project p = new _Project();
            p.project_name = Convert.ToString(dr["project_name"]);
            p.project_value = Convert.ToString(dr["project_value"]);
            output.Add(p);
        }
        con.Close();
        return output;
    }

    public List<_Client> GetClient()
    {
        List<_Client> output = new List<_Client>();

        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;

        cmd.CommandText = "SELECT * FROM tblClientSettings WHERE isActive =1 ";
        SqlDataReader dr;

        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            _Client c = new _Client();
            c.ClientName = Convert.ToString(dr["ClientName"]);
            c.client_market_value = Convert.ToInt32(dr["client_market_value"]);
            c.Campaigns = Convert.ToString(dr["Campaigns"]);
            output.Add(c);
        }
        con.Close();
        return output;
    }


    public bool ValidateUser(string username, string password)
    {
        bool isValidated = false;
        if (Membership.ValidateUser(username, password))
            isValidated = true;

        return isValidated;

    }


    public bool CheckIsUserinRole(string username, string roleName)
    {
        bool output = false;
        string[] ListofUser = Roles.FindUsersInRole(roleName, username);

        for (int i = 0; i <= ListofUser.Length - 1; i++)
        {
            if (ListofUser[i] == username)
                output = true;
        }

        return output;
    }


    public bool tblFTPFile(
        int id_server_transaction,
        int id_client_transaction,
        string file_name,
        DateTime date_created,
        bool is_finished,
        int file_status,
        string file_name_only,
        int bytestrnsfrd)
    {

        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;


        cmd.CommandText = "SELECT * FROM tblFTPFile where file_name='" + file_name.Replace("'", "''") + "' AND (id_server_transaction = " + id_server_transaction + ")";

        SqlDataReader dr;
        dr = cmd.ExecuteReader();

        if (!dr.HasRows)
        {
            cmd.Dispose();
            dr.Close();
            //2011-10-15-20-00-53-0003-0025-0002-0003-A
            DateTime? Date_Collected = null;
            if (file_name_only.Length > 10)
            {
                string datepart = file_name_only.Substring(0, 10);
                DateTime tmp;
                if (DateTime.TryParse(datepart, out tmp))
                {
                    Date_Collected = DateTime.Parse(datepart);
                }
            }
            String FileNameNoExtension = file_name_only.Substring(0, file_name_only.LastIndexOf("."));

            //SqlConnection con = new SqlConnection(connectionString);
            //con.Open();
            cmd = new SqlCommand();
            cmd.Connection = con;
            if (Date_Collected == null)
                cmd.CommandText = "INSERT INTO tblFTPFile(id_server_transaction,id_client_transaction,file_name,is_finished,file_status,file_name_only,bytestrnsfrd, FileNameNoExtension) VALUES (" + id_server_transaction + "," + id_client_transaction + ",'" + file_name.Replace("'", "''") + "',0,0,'" + file_name_only.Replace("'", "''") + "'," + bytestrnsfrd + ",'" + FileNameNoExtension + "')";
            else
                cmd.CommandText = "INSERT INTO tblFTPFile(id_server_transaction,id_client_transaction,file_name,is_finished,file_status,file_name_only,Date_Collected, bytestrnsfrd, FileNameNoExtension) VALUES (" + id_server_transaction + "," + id_client_transaction + ",'" + file_name.Replace("'", "''") + "',0,0,'" + file_name_only.Replace("'", "''") + "','" + Convert.ToString(Date_Collected) + "'," + bytestrnsfrd + ",'" + FileNameNoExtension + "')";
            cmd.ExecuteNonQuery();
        }


        con.Close();
        return true;
    }

    public bool tblDBStatus(int Action, string status_notify, int status_flag, DateTime MaintenanceStart, DateTime MaintenanceEnd, string CreatedBy)
    {

        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;

        cmd.CommandText = "INSERT INTO tblDBStatus(status_notify,status_flag,MaintenanceStart,MaintenanceEnd,CreatedBy) "
                        + " VALUES ('" + status_notify.Replace("'", "''") + "," + status_flag + "," + MaintenanceStart + "," + MaintenanceEnd + ",'" + CreatedBy + "')";

        cmd.ExecuteNonQuery();
        con.Close();

        return true;

    }

    public List<_DBStatus> GetDBStatus(int status_flag)
    {
        List<_DBStatus> output = new List<_DBStatus>();
        string connectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["REMIS"]);
        SqlConnection con = new SqlConnection(connectionString);

        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;

        cmd.CommandText = "SELECT * FROM tblDBStatus WHERE status_flag=" + status_flag;
        SqlDataReader dr;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            while (dr.Read())
            {
                _DBStatus db = new _DBStatus();
                db.id_DBStatus = Convert.ToInt32(dr["id_DBstatus"]);
                db.status_notify = Convert.ToString(dr["status_notify"]);
                db.status_flag = Convert.ToInt32(dr["status_flag"]);
                db.DateCreated = Convert.ToDateTime(dr["DateCreated"]);
                try
                {
                    db.MaintenanceStart = Convert.ToDateTime(dr["MaintenanceStart"]);
                }
                catch
                {
                    db.MaintenanceStart = null;
                }

                try
                {
                    db.MaintenanceEnd = Convert.ToDateTime(dr["MaintenanceEnd"]);
                }
                catch
                {
                    db.MaintenanceEnd = null;
                }
                output.Add(db);
            }
        }
        con.Close();
        return output;
    }

    public bool UpdateFileScannerStatus(int id_server_transaction, string FileName, int isScanner)
    {
        //ADO.NET
        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM tblFTPFile where file_name='" + FileName.Replace("'", "''") + "' AND (id_server_transaction = " + id_server_transaction + ")";

        SqlDataReader dr;
        dr = cmd.ExecuteReader();
        int id_server_file = -1;
        if (dr.HasRows)
        {
            while (dr.Read())
            {
                id_server_file = Convert.ToInt32(dr["id_server_file"]);
            }
        }
        dr.Close();
        dr.Dispose();
        if (id_server_file != -1)
        {
            SqlCommand cmd1 = new SqlCommand();
            cmd1.Connection = con;
            cmd1.CommandText = "UPDATE tblFTPFile set isScanner = " + isScanner + " WHERE id_server_file = " + id_server_file;
            cmd1.ExecuteNonQuery();
            cmd1.Dispose();
        }

        cmd.Dispose();
        con.Close();
        return true;
    }

    public bool UpdatetbFTPFile(int id_server_transaction, string FileName, int status)
    {

        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;

        if ((status == 1) || (status == 9))
        {
            cmd.CommandText = "SELECT * FROM tblFTPFile where file_name='" + FileName.Replace("'", "''") + "' AND (id_server_transaction = " + id_server_transaction + ")";

            SqlDataReader dr;
            dr = cmd.ExecuteReader();

            if (!dr.HasRows)
            {
                String file_name_only = FileName.Substring(FileName.LastIndexOf(@"\") + 1, FileName.Length - FileName.LastIndexOf(@"\") - 1);
                DateTime? Date_Collected = null;
                if (file_name_only.Length > 10)
                {
                    string datepart = file_name_only.Substring(0, 10);
                    DateTime tmp;
                    if (DateTime.TryParse(datepart, out tmp))
                    {
                        Date_Collected = DateTime.Parse(datepart);
                    }
                }

                dr.Close();
                dr.Dispose();

                if (Date_Collected == null)
                    cmd.CommandText = "INSERT INTO tblFTPFile(id_server_transaction,file_name,file_name_only) VALUES (" + id_server_transaction + ",'" + FileName.Replace("'", "''") + "','" + file_name_only.Replace("'", "''") + "')";
                else
                    cmd.CommandText = "INSERT INTO tblFTPFile(id_server_transaction,file_name,file_name_only,Date_Collected) VALUES (" + id_server_transaction + ",'" + FileName.Replace("'", "''") + "','" + file_name_only.Replace("'", "''") + "','" + Convert.ToString(Date_Collected) + "')";

                cmd.ExecuteNonQuery();
            }
            else
            {

                dr.Close();
                dr.Dispose();
            }

            cmd.CommandText = "UPDATE tblFTPFile SET date_finished=getdate(), is_finished=1, file_status = 1 WHERE id_server_transaction=" + id_server_transaction + " AND file_name ='" + FileName.Replace("'", "''") + "'";
            if (status == 9)
            {
                cmd.CommandText = "UPDATE tblFTPFile SET date_finished=getdate(), is_finished=1, file_status = 1, status_flag =9 WHERE id_server_transaction=" + id_server_transaction + " AND file_name ='" + FileName.Replace("'", "''") + "'";
            }
        }
        else
        {
            cmd.CommandText = "UPDATE tblFTPFile SET file_status =" + status + " WHERE id_server_transaction=" + id_server_transaction + " AND file_name ='" + FileName.Replace("'", "''") + "'";
        }
        cmd.ExecuteNonQuery();

        if ((status == 1) || (status == 9))
        {
            //Check if all file has been uploaded to server and update transaction
            int count_pending = 0;
            cmd.CommandText = "SELECT * FROM tblFTPFile WHERE file_status <= 0 AND id_server_transaction=" + id_server_transaction;
            count_pending = Convert.ToInt32(cmd.ExecuteScalar());
            if (count_pending == 0)
            {
                //Update transaction status as completed
                cmd.CommandText = "UPDATE tblFTPTransaction SET date_end = getdate(), isCompleted = 1, transaction_status=1 WHERE id_server_transaction = " + id_server_transaction;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            //Update total_file_uploaded

            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT count(*) FROM tblFTPFile WHERE (file_status =1 OR file_status>=3) AND id_server_transaction = " + id_server_transaction;
            int total_uploaded = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.Dispose();
            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "UPDATE tblFTPTransaction SET total_file_uploaded = " + total_uploaded + " WHERE id_server_transaction = " + id_server_transaction;
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
            cmd.CommandText = "SELECT * FROM tblFTPFile where (file_status =1 Or file_status>=3) AND (id_server_transaction = " + id_server_transaction + ")";

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

            cmd.CommandText = "UPDATE tblFTPTransaction SET totalbytesuploaded = " + totalbytesuploaded.ToString() + " WHERE id_server_transaction = " + id_server_transaction;
            cmd.ExecuteNonQuery();
            cmd.Dispose();

        }



        con.Close();
        return true;
    }

    public bool UpdatetbFTPFileUploadTime(int id_server_transaction, string FileName, int uploadtime)
    {
        try
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "UPDATE tblFTPFile SET uploadtime =" + uploadtime + " WHERE id_server_transaction=" + id_server_transaction + " AND file_name ='" + FileName.Replace("'", "''") + "'";
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch
        {
            return false;
        }
        return true;
    }

    private string GetMarketName(int id_market)
    {
        string market_name = "";

        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;

        cmd.CommandText = "SELECT * FROM tblMarket where id_market=" + id_market;
        SqlDataReader dr;

        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            market_name = Convert.ToString(dr["name_market"]);

        }

        return market_name.Replace(",", "");
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



    public int tblFTPTrasaction(int Action,
        int id_server_transaction,
        int id_client_transaction,
        bool transaction_type,
        DateTime date_started,
        string username,
        bool isCompleted,
        short number_of_run,
        short number_of_app_fail,
        int id_market,
        string ftp_url,
        string ip_address,
        string host_name,
        string market_name,
        short transaction_status,
        string DBName,
        int total_file_uploaded,
        string campaign,
        string Project,
        double totalinputbytes
        )
    {


        int id_server_transaction_sub = -1;

        try
        {
            if ((id_market == 419) || (id_market == 643) || (id_market == 1901))
            {
                int tmp_id_market = GetMarketID(market_name, Project);
                if (tmp_id_market != 0)
                    id_market = tmp_id_market;
            }
        }
        catch
        {

        }




        string connectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["REMIS"]);
        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;


        cmd.CommandText = "INSERT INTO tblFTPTransaction(id_client_transaction,transaction_type,username,isCompleted,number_of_run,number_of_app_fail,"
         + " id_market,ftp_url,ip_address,host_name,market_name,transaction_status,DBName,total_file_uploaded,campaign,Project,totalinputbytes) VALUES "
         + " (" + id_client_transaction + ",0,'" + username + "',0,1,0," + id_market.ToString() + ",'"
         + ftp_url + "','" + ip_address + "','" + host_name.Replace("'", "''") + "','" + market_name.Replace("'", "''") + "',0,'" + DBName.Replace("'", "''") + "'," + id_market.ToString() + ",'" + campaign + "','" + Project.Replace("'", "''") + "'," + totalinputbytes + " ); SELECT @@IDENTITY ";


        object tmpInt = cmd.ExecuteScalar();
        if (tmpInt != null)
        {
            id_server_transaction_sub = Convert.ToInt32(tmpInt);
            id_server_transaction = id_server_transaction_sub;
        }
        else
            id_server_transaction = -1;


        //Update id_server_transaction = id_server_transaction_sub
        cmd.Dispose();
        //cmd = new SqlCommand();
        //cmd.Connection = con;
        //cmd.CommandText = "UPDATE tblFTPTransaction SET id_server_transaction = " + id_server_transaction_sub + " WHERE id_server_transaction_sub=" + id_server_transaction_sub;
        //cmd.ExecuteNonQuery();
        //cmd.Dispose();

        con.Close();
        return id_server_transaction;
    }


    public List<_Campaign> GetCampaign()
    {
        List<_Campaign> output = new List<_Campaign>();

        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM tblCampaign WHERE isACtive=1";

        SqlDataReader dr;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            while (dr.Read())
            {
                _Campaign c = new _Campaign();
                c.id_Campaign = Convert.ToInt32(dr["id_campaign"]);
                c.campaign_name = Convert.ToString(dr["campaign_name"]);
                c.isActive = Convert.ToBoolean(dr["isActive"]);
                output.Add(c);
            }
        }

        con.Close();

        return output;
    }

    public bool deletetblFTPFile(int id_server_transaction)
    {

        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "DELETE FROM tblFTPFile WHERE id_server_transaction =" + id_server_transaction.ToString();

        try
        {
            cmd.ExecuteNonQuery();

            cmd.Dispose();
            //Update total_file_uploaded

            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT count(*) FROM tblFTPFile WHERE file_status =1 AND id_server_transaction = " + id_server_transaction;
            int total_uploaded = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.Dispose();
            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "UPDATE tblFTPTransaction SET total_file_uploaded = " + total_uploaded + " WHERE id_server_transaction = " + id_server_transaction;
            cmd.ExecuteNonQuery();
            cmd.Dispose();


            con.Close();
            return true;
        }
        catch
        {
            con.Close();
            return false;

        }
    }

    public bool deleteFile(int id_server_transaction, string file_name)
    {


        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "DELETE FROM tblFTPFile WHERE id_server_transaction =" + id_server_transaction.ToString() + " AND file_name='" + file_name.Replace("'", "''") + "'";

        try
        {
            cmd.ExecuteNonQuery();

            cmd.Dispose();
            //Update total_file_uploaded

            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT count(*) FROM tblFTPFile WHERE (file_status =1 OR file_status>=3) AND id_server_transaction = " + id_server_transaction;
            int total_uploaded = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.Dispose();
            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "UPDATE tblFTPTransaction SET total_file_uploaded = " + total_uploaded + " WHERE id_server_transaction = " + id_server_transaction;
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();

            return true;
        }
        catch
        {
            con.Close();
            return false;
        }
    }


    public List<_MarketName> GetMarket(int status)
    {
        List<_MarketName> output = new List<_MarketName>();


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
                _MarketName c = new _MarketName();
                c.name_market = Convert.ToString(dr["name_market"]);
                c.market_status = Convert.ToInt32(dr["market_status"]);
                c.id_market = Convert.ToInt32(dr["id_market"]);
                output.Add(c);
            }
        }
        con.Close();
        return output;
    }


    public int updateTotalFailed(int id_server_transaction, int fail_count, int network_lost_count)
    {

        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM tblFTPTransaction WHERE id_server_transaction = " + id_server_transaction.ToString();
        int number_of_app_fail = 0;
        int number_of_run = 0;
        SqlDataReader dr;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            while (dr.Read())
            {
                number_of_app_fail = Convert.ToInt32(dr["number_of_app_fail"]) + fail_count;
                number_of_run = Convert.ToInt32(dr["number_of_run"]) + network_lost_count;
            }
        }

        dr.Close();
        dr.Dispose();

        cmd.CommandText = "UPDATE tblFTPTransaction SET number_of_app_fail=" + number_of_app_fail.ToString()
            + ",number_of_run =" + number_of_run.ToString() + " WHERE id_server_transaction = " + id_server_transaction.ToString();
        cmd.ExecuteNonQuery();
        con.Close();
        return 0;
    }

    public int countFile(int id_server_transaction, string type)
    {
        int total_file = -1;

        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;

        cmd.CommandText = "SELECT COUNT(*) FROM tblFTPFile WHERE id_server_transaction = " + id_server_transaction.ToString();

        if (type == "all")
            cmd.CommandText = "SELECT COUNT(*) FROM tblFTPFile WHERE id_server_transaction = " + id_server_transaction.ToString();
        else if (type == "finished")
            cmd.CommandText = "SELECT COUNT(*) FROM tblFTPFile WHERE (file_status=1 Or file_status>=3) AND id_server_transaction = " + id_server_transaction.ToString();
        else if (type == "failed")
            cmd.CommandText = "SELECT COUNT(*) FROM tblFTPFile WHERE file_status=-1 AND id_server_transaction = " + id_server_transaction.ToString();
        else if (type == "pending")
            cmd.CommandText = "SELECT COUNT(*) FROM tblFTPFile WHERE file_status=0 AND id_server_transaction = " + id_server_transaction.ToString();
        else if (type == "deleted")
            cmd.CommandText = "SELECT COUNT(*) FROM tblFTPFile WHERE file_status=2 AND id_server_transaction = " + id_server_transaction.ToString();



        total_file = Convert.ToInt32(cmd.ExecuteScalar());

        con.Close();
        return total_file;
    }

    public bool updateTotalUploaded(int id_server_transaction, int total_file_uploaded)
    {
        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "UPDATE tblFTPTransaction SET total_file_uploaded =" + total_file_uploaded + " WHERE id_server_transaction = " + id_server_transaction.ToString();
        cmd.ExecuteNonQuery();
        con.Close();
        return true;
    }


    public bool deleteTransaction(int id_server_transaction)
    {

        try
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "UPDATE tblFTPTransaction SET transaction_status=2 WHERE id_server_transaction = " + id_server_transaction.ToString();
            cmd.ExecuteNonQuery();
            con.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool AddMarket(string name_market)
    {
        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "INSERT INTO tblMarket(name_market,market_status) VALUES ('" + name_market + "',1)";
        cmd.ExecuteNonQuery();
        con.Close();
        return true;
    }


    public _GTPVersion getVersion(string version, bool isCurrent)
    {
        _GTPVersion GTPversion = new _GTPVersion();
        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        if (version != "")
            cmd.CommandText = "SELECT * FROM tblVersion WHERE version='" + version + "'";
        else
            cmd.CommandText = "SELECT * FROM tblVersion WHERE isCurrent=1";
        SqlDataReader dr;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            while (dr.Read())
            {
                GTPversion.id_version = Convert.ToInt32(dr["id_version"]);
                GTPversion.version = Convert.ToString(dr["version"]);
                GTPversion.total_installed = Convert.ToInt32(dr["total_installed"]);
                GTPversion.team_installed = Convert.ToString(dr["team_installed"]);
                GTPversion.new_features = Convert.ToString(dr["new_features"]);
                GTPversion.release_date = Convert.ToDateTime(dr["release_date"]);
                GTPversion.isCurrent = Convert.ToBoolean(dr["isCurrent"]);
                try
                {
                    GTPversion.doAllowUpload = Convert.ToBoolean(dr["doAllowUpload"]);
                }
                catch
                {
                    GTPversion.doAllowUpload = true;
                }

            }
        }
        con.Close();
        return GTPversion;
    }

    public List<_AspNetUser> GetUserAccount(int hasFTPAccess)
    {
        List<_AspNetUser> output = new List<_AspNetUser>();
        SqlConnection con = new SqlConnection(UserDBconnectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;

        if (hasFTPAccess >= 0)
            cmd.CommandText = "select * from aspnet_Users where hasFTPAccess = " + hasFTPAccess.ToString() + " Order by username ";
        else
            cmd.CommandText = "SELECT * FROM aspnet_Users Order by username ";


        SqlDataReader dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            _AspNetUser user = new _AspNetUser();
            user.username = Convert.ToString(dr["username"]);
            user.LastActivityDate = Convert.ToDateTime(dr["LastActivityDate"]);
            user.isFTPAccess = Convert.ToBoolean(dr["hasFTPAccess"]);
            output.Add(user);
        }

        return output;
    }

    public int GetFileStatus(int id_server_transaction, string file_name)
    {
        int file_status = 0;

        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT file_status FROM tblFTPFile WHERE id_server_transaction=" + id_server_transaction.ToString() + " AND file_name='" + file_name + "'";
        file_status = Convert.ToInt32(cmd.ExecuteScalar());
        con.Close();
        return file_status;
    }


    public bool ExecuteQuery(string query)
    {

        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = query;

        cmd.ExecuteNonQuery();
        cmd.Dispose();
        con.Close();
        return true;
    }

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



    public bool isFileExist(string FileName)
    {
        bool isExist = false;

        String fileNameNoExt = FileName.Substring(0, FileName.LastIndexOf("."));

        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM tblFTPFile WHERE (file_status=1 OR file_status >=3) AND FileNameNoExtension='" + fileNameNoExt + "'";
        SqlDataReader dr;
        dr = cmd.ExecuteReader();

        if (dr.HasRows)
        {
            isExist = true;
        }
        return isExist;
    }

    public bool completeTransaction(int id_server_transaction)
    {
        try
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "UPDATE tblFTPTransaction SET date_end = getdate(), isCompleted = 1, transaction_status=1 WHERE id_server_transaction = " + id_server_transaction;
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            //Update total upload file size
            cmd = new SqlCommand();
            cmd.Connection = con;
            double totalbytesuploaded = 0;
            SqlDataReader dr;
            cmd.CommandText = "SELECT * FROM tblFTPFile where (file_status =1 Or file_status>=3) AND (id_server_transaction = " + id_server_transaction + ")";

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

            cmd.CommandText = "UPDATE tblFTPTransaction SET totalbytesuploaded = " + totalbytesuploaded.ToString() + " WHERE id_server_transaction = " + id_server_transaction;
            cmd.ExecuteNonQuery();
            cmd.Dispose();


            con.Close();
            return true;
        }
        catch
        {
            return false;
        }

    }

    public bool  AddFTPFileFailed(int idServerTransaction, string fileName, string fileNameWithoutExt, string extension)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "INSERT INTO tblFTPFileFailed VALUES (" + idServerTransaction + ",'"+ fileName +"','" + fileNameWithoutExt +"','" + DateTime.Now + "','" + extension +"')";
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            return true;
        }
        catch 
        {
            return false;
        }
    }
}
