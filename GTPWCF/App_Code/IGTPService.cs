using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGTPService" in both code and config file together.
[DataContract]
public class _FileGroupConfig
{
    [DataMember]
    public String file_group_name;
    [DataMember]
    public String device_id;
    [DataMember]
    public int NumToTriggerMissingAlert;
}

[DataContract]
public class _ShadowPassword
{
    [DataMember]
    public int id_shadow;
    [DataMember]
    public string username;
    [DataMember]
    public string password;
    [DataMember]
    public int type;
    [DataMember]
    public bool isActive;
    [DataMember]
    public DateTime date_created;
}

[DataContract]
public class _Project
{
    [DataMember]
    public string project_name;
    [DataMember]
    public string project_value;
}

public class _Client
{
    [DataMember]
    public string ClientName;
    [DataMember]
    public int client_market_value;
    [DataMember]
    public string Campaigns;
}

public class _TeamProjects
{
    [DataMember]
    public string UserName;
    [DataMember]
    public string ClientName;

}

[DataContract]
public class _FTPTransaction
{
    [DataMember]
    public int id_server_transaction;
    [DataMember]
    public DateTime? date_started;
    [DataMember]
    public DateTime? date_end;
    [DataMember]
    public bool isCompleted;
    [DataMember]
    public int number_of_run;
    [DataMember]
    public int number_off_app_fail;
    [DataMember]
    public int transaction_status;
    [DataMember]
    public int total_file_uploaded;
    [DataMember]
    public string market_name;
    [DataMember]
    public string Campaign;
    [DataMember]
    public string Project;
    [DataMember]
    public int totalinputbytes;
    [DataMember]
    public int totalbytesuploaded;
}

[DataContract]
public class _AspNetUser
{
    [DataMember]
    public string username;
    [DataMember]
    public DateTime LastActivityDate;
    [DataMember]
    public bool isFTPAccess;
}


[DataContract]
public class _DBStatus
{
    [DataMember]
    public int id_DBStatus;
    [DataMember]
    public string status_notify;
    [DataMember]
    public int status_flag;
    [DataMember]
    public DateTime? DateCreated;
    [DataMember]
    public DateTime? MaintenanceStart;
    [DataMember]
    public DateTime? MaintenanceEnd;
    [DataMember]
    public string CreatedBy;
}

[DataContract]
public class _GTPVersion
{
    [DataMember]
    public int id_version;
    [DataMember]
    public string version;
    [DataMember]
    public int total_installed;
    [DataMember]
    public string team_installed;
    [DataMember]
    public string new_features;
    [DataMember]
    public DateTime release_date;    
    [DataMember]
    public bool isCurrent;
    [DataMember]
    public bool doAllowUpload;
}

[DataContract]
public class _Campaign
{
    [DataMember]
    public int id_Campaign;
    [DataMember]
    public string campaign_name;
    [DataMember]
    public bool isActive;
}

[DataContract]
public class _MarketName
{
    [DataMember]
    public string name_market;
    [DataMember]
    public int market_status;
    [DataMember]
    public int id_market;
}

[ServiceContract]
public interface IGTPService
{

    [OperationContract]
    List<_FileGroupConfig> getFileGroupContract();

    [OperationContract]
    String ShadowPassword(int action, _ShadowPassword shadowPassword);

    [OperationContract]
    List<_ShadowPassword> getShadowPassword(int? status);

	[OperationContract]
	bool ValidateUser(string username,string password);
    
    [OperationContract]
    bool tblFTPFile(
    int id_server_transaction,
    int id_client_transaction,
    string file_name,
    DateTime date_created,    
    bool is_finished,
    int file_status,
    string file_name_only,
    int bytestrnsfrd);

    //[OperationContract]
    //bool DeletetblFTPFile(int id_server_transaction, string FileName);

    [OperationContract]
    bool UpdatetbFTPFile(int id_server_transaction, string FileName, int status);
    
    [OperationContract]
    bool UpdateFileScannerStatus(int id_server_transaction, string FileName, int isScanner);

    [OperationContract]
    bool UpdatetbFTPFileUploadTime(int id_server_transaction, string FileName, int uploadtime);


    [OperationContract]
    int tblFTPTrasaction(int Action,     
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
        double totalinputbytes);





    [OperationContract]
    bool tblDBStatus(int Action, string status_notify, int status_flag, DateTime MaintenanceStart, DateTime MaintenanceEnd, string CreatedBy);

    [OperationContract]
    List<_DBStatus> GetDBStatus(int status_flag);

    [OperationContract]
    bool CheckIsUserinRole(string username, string roleName);

    [OperationContract]
    List<_Campaign> GetCampaign();

    [OperationContract]
    bool deletetblFTPFile(int id_server_transaction);

    [OperationContract]
    bool deleteFile(int id_server_transaction, string file_name);

    [OperationContract]
    List<_MarketName> GetMarket(int status);

    [OperationContract]
    int updateTotalFailed(int id_server_transaction, int fail_count, int network_lost_count);

    [OperationContract]
    int countFile(int id_server_transaction,string type);

    [OperationContract]
    bool updateTotalUploaded(int id_server_transaction, int total_file_uploaded);

    [OperationContract]
    bool deleteTransaction(int id_server_transaction);


    [OperationContract]
    bool AddMarket(string name_market);

    [OperationContract]
    bool completeTransaction(int id_server_transaction);

    [OperationContract]
    _GTPVersion getVersion(string version, bool isCurrent);


    [OperationContract]
    List<_AspNetUser> GetUserAccount(int isFTPAccess);

    [OperationContract]
    int GetFileStatus(int id_server_transaction, string file_name);

    [OperationContract]
    bool ExecuteQuery(string query);

    [OperationContract]
    _FTPTransaction GetTransaction(int id_server_transaction);

    [OperationContract]
    bool isFileExist(string FileName);

    [OperationContract]
    List<_Project> GetProject();

    [OperationContract]
    List<_Client> GetClient();

    [OperationContract]
    List<string> GetClientForTeam(String TeamName);

    [OperationContract]
    List<_TeamProjects> GetTeamProjects();

    [OperationContract]
    bool AddFTPFileFailed(int idServerTransaction, string fileName,string fileNameWithoutExt, string extension);
}
