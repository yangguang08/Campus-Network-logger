Campus-Network-logger
=====================

Campus Network logger

Core code of network logger

  Uri uri = new Uri("Server IP Address");//Example:127.25.35.105/host/logger  <br/>
  WebRequest objWebRequest = WebRequest.Create(uri);  <br/>
  objWebRequest.Credentials = CredentialCache.DefaultCredentials;  <br/>
  objWebRequest.Method = "POST";  <br/>
  objWebRequest.ContentType = "application/x-www-form-urlencoded";  <br/>
  objWebRequest.ContentType = "text/html";  <br/>
  Stream dataStream = objWebRequest.GetRequestStream();  <br/>
  //需要传递哪些参数，抓包获取，对应即可  <br/>
  string data = String.Format("action=XX&username={0}&password={1}&……&nas_ip=172.XX.XX.XX&……",txtStuID,txtStuKEY);  <br/>
  UTF8Encoding f8 = new UTF8Encoding();  <br/>
  byte[] byteArray = f8.GetBytes(data);  <br/>
  dataStream.Write(byteArray, 0, byteArray.Length);  <br/>
  dataStream.Close();  <br/>
  WebResponse objResponse = objWebRequest.GetResponse();  <br/>
  lblMSG.Text = ((HttpWebResponse)objResponse).StatusDescription;  <br/>
  <br/>
  Stream rpData = objResponse.GetResponseStream();  <br/>
  StreamReader r = new StreamReader(rpData);  <br/>
  string msg = r.ReadToEnd();  <br/>
    <br/>
    <br/>
Core code of network WiFi  <br/>
  <br/>
  Open wifi  <br/>
  string strCommand1 = string.Format("netsh wlan set hostednetwork mode=allow ssid={0} key={1}", txtName.Text.Trim(), txtPassword.Text.Trim());  <br/>
  string strCommand2 = string.Format("netsh wlan start hostednetwork");  <br/>
  string result1 = CommandDo.Execute(strCommand1, 10000);  <br/>
    <br/>
  close wifi  <br/>
  string strCommand1 = string.Format("netsh wlan stop hostednetwork");  <br/>
  string result1 = CommandDo.Execute(strCommand1, 10000);  <br/>
    <br/>
  Interop.NETCONLib.dll file is used,for which network card should be choosed.
  
