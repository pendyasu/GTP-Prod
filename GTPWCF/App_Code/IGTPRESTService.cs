using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;



// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGTPRESTService" in both code and config file together.
[ServiceContract]
public interface IGTPRESTService
{
    //[OperationContract]
    //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Transaction/{TransactionID}")]
    //_FTPTransaction GetTransaction(int TransactionID);
    //[OperationContract]
    //[WebGet(UriTemplate="GetMarket",BodyStyle = WebMessageBodyStyle.Wrapped,ResponseFormat=R)]

    [OperationContract]
    [WebGet(UriTemplate = "GetProject")]
    List<string> GetProject();
    //upload.mygws.com/GTPWCF-DEV/GTPRESTService.svc/tblFTPTransaction?id_client_transaction=0&username=tuando&ip_address=192.168.1.15&host_name=TuanPC&market_name=AustinTX&campaign=13D1&Project=ATTBM&totalinputbytes=1000   
  
    
    [OperationContract]
    [WebInvoke(Method = "PUT", BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "tblFTPTransaction?TransactionGID={TransactionGID}&username={username}&ip_address={ip_address}&" +
    "host_name={host_name}&market_name={market_name}&campaign={campaign}&Project={Project}&totalinputbytes={totalinputbytes}")]

    string tblFTPTrasaction(Guid TransactionGID,
                           string username,
                           string ip_address,
                           string host_name,
                           string market_name,
                           string campaign,
                           string Project,
                           double totalinputbytes);


    
    [OperationContract]
    [WebInvoke(Method = "PUT", BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "tblFTPFile?TransactionGID={TransactionGID}&file_name={file_name}&bytestrnsfrd={bytestrnsfrd}")]    
    string tblFTPFile(
    Guid TransactionGID,    
    string file_name,
    int bytestrnsfrd);


    [OperationContract]
    [WebInvoke(Method = "PUT", UriTemplate = "UpdatetblFTPFile?TransactionGID={TransactionGID}&FileName={FileName}&status={status}")]    
    string UpdatetbFTPFile(Guid TransactionGID, string FileName, int status);

    [OperationContract]
    [WebInvoke(Method = "PUT", UriTemplate = "UpdatetblFTPFileUploadTime?TransactionGID={TransactionGID}&FileName={FileName}&uploadtime={uploadtime}")]
    string UpdatetbFTPFileUploadTime(Guid TransactionGID, string FileName, int uploadtime);

        
    [OperationContract]
    [WebInvoke(Method = "PUT", UriTemplate = "completeTransaction?TransactionGID={TransactionGID}&Status={Status}")]
    string completeTransaction(Guid TransactionGID, string Status);


    [WebGet(UriTemplate = "tblMarket?status={status}", ResponseFormat = WebMessageFormat.Json)]
    [OperationContract]
    List<String> GetMarket(int status);



}
